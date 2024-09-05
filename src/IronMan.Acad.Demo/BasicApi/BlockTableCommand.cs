using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using IronMan.Acad.Demo.BasicApi;
using IronMan.Acad.Demo.Extensions;
using System.Collections.Generic;
using System.Linq;

[assembly: CommandClass(typeof(BlockTableCommand))]
namespace IronMan.Acad.Demo.BasicApi;

internal class BlockTableCommand : CommandBase
{
    //模态时被使用，绘制图形时被使用，编辑块时禁止使用
    [CommandMethod(nameof(CreateBlock), CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void CreateBlock()
    {
        var result = Editor.GetPoint("\n 放置块");
        if (result.Status != Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
        {
            return;
        }
        using var ts = Document.TransactionManager.StartTransaction();
        var blockTable = (BlockTable)Database.BlockTableId.GetObject(OpenMode.ForWrite);
        //获取模型空间这个块
        var blockTableRecord = (BlockTableRecord)blockTable[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite);
        //准备放置的块
        using var blockReference = new BlockReference(result.Value, blockTableRecord.Id);
        blockReference.LayerId = Document.GetOrCreateLayer("my layer");
        blockTableRecord.AppendEntity(blockReference);
        ts.AddNewlyCreatedDBObject(blockReference, true);
        ts.Commit();
        Editor.WriteMessage("RE \n");

    }

    [CommandMethod(nameof(CreateBlockTableRecord), CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void CreateBlockTableRecord()
    {
        using var ts = Document.TransactionManager.StartTransaction();
        var blockTable = (BlockTable)Database.BlockTableId.GetObject(OpenMode.ForWrite);
        var modelSpace = (BlockTableRecord)blockTable[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite);
        if (!blockTable.Has("my block"))
        {
            using var record = new BlockTableRecord();
            record.Name = "my block";
            CreateLine(record);
            var id = blockTable.Add(record);
            ts.AddNewlyCreatedDBObject(record, true);

            using var blockReference = new BlockReference(Point3d.Origin, id);
            blockReference.LayerId = Document.GetOrCreateLayer("my layer");
            modelSpace.AppendEntity(blockReference);
            ts.AddNewlyCreatedDBObject(blockReference, true);
        }
    }

    private void CreateLine(BlockTableRecord record)
    {
        var list = new List<Point3d>();
        var upLeft = new Point3d(-500, 500, 0);
        var upRight = new Point3d(500, 500, 0);
        var downRight = new Point3d(500, -500, 0);
        var downLeft = new Point3d(-500, -500, 0);
        list.Add(upLeft);
        list.Add(upRight);
        list.Add(downLeft);
        list.Add(downRight);
        foreach (var item in list)
        {
            if (list.Last() == item)
            {
                record.AppendEntity(new Line(list.First(), item));
            }
            else
            {
                var index = list.IndexOf(item);
                record.AppendEntity(new Line(item, list.ElementAt(index + 1)));
            }
        }

    }
}
