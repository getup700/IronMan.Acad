using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(InputCommand))]
namespace IronMan.CAD.Commands;

internal class TransactionCommand
{
    public static Document Document = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
    public static Editor Editor = Document.Editor;
    public static Database Database = Document.Database;

    //CAD扫描当前程序集内所有的类
    //静态or实例方法都可以
    [CommandMethod(nameof(CreateLine), CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void CreateLine()
    {
        Debug.WriteLine("Hello Debug");
        var firstResult = Editor.GetPoint("input first point please.");
        if (firstResult.Status == PromptStatus.OK)
        {
            var secontResult = Editor.GetPoint("input second point please.");
            if (secontResult.Status == PromptStatus.OK)
            {
                //OpenClose事务性能更高，但不允许事务嵌套
                //var openClose = Database.TransactionManager.StartOpenCloseTransaction();
                using var transaction = Database.TransactionManager.StartTransaction();
                using var line = new Line(firstResult.Value, secontResult.Value);

                //开启事务时，transaction已经将事务存在database中，所以不需要再从transaction获取图元
                var currentTransaction = Database.TransactionManager.TopTransaction;

                //从transaction获取图元
                var tabelFromTransaction = transaction.GetObject(Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                //从database获取图元
                var tableFromDatabase = Database.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;

                Editor.WriteMessage($"{tabelFromTransaction == tableFromDatabase}");
                var record = tableFromDatabase[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite) as BlockTableRecord;
                record.AppendEntity(line);
                //把创建好的对象添加到事务，以方便后续对其操作
                //如果不是新建则无需考虑
                transaction.AddNewlyCreatedDBObject(line, true);
                transaction.Commit();

                //CAD大部分对象需要手动销毁，如果等到GC销毁则可能导致内存泄漏

            }
        }

    }

    [CommandMethod(nameof(CreateLineToLayer), CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void CreateLineToLayer()
    {
        Debug.WriteLine("Hello Debug");
        var pointResult1 = Editor.GetPoint("input first point please.");
        if (pointResult1.Status == PromptStatus.OK)
        {
            var options = new PromptPointOptions("\n input second point please.");
            options.BasePoint = pointResult1.Value;
            options.UseBasePoint = true;
            var pointResult2 = Editor.GetPoint(options);

            if (pointResult2.Status == PromptStatus.OK)
            {
                using var transaction = Database.TransactionManager.StartTransaction();
                using var line = new Line(pointResult1.Value, pointResult2.Value);

                var layerTable = (LayerTable)Database.LayerTableId.GetObject(OpenMode.ForRead);
                if (!layerTable.Has("line"))
                {
                    using var layer = new LayerTableRecord();
                    layer.Name = "line";
                    layer.Color = Color.FromColorIndex(ColorMethod.ByAci, 2);

                    layerTable.UpgradeOpen();
                    layerTable.Add(layer);
                    transaction.AddNewlyCreatedDBObject(layer, true);
                    layerTable.DowngradeOpen();
                }

                line.LayerId = layerTable["line"];
                var table = (BlockTable)Database.BlockTableId.GetObject(OpenMode.ForRead);
                var record = (BlockTableRecord)table[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite);

                record.AppendEntity(line);
                transaction.AddNewlyCreatedDBObject(line, true);
                transaction.Commit();


            }
        }

    }
}
