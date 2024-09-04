using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using IronMan.Abstract.CAD;
using IronMan.CAD.Demo.Command;
using IronMan.CAD.Demo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: CommandClass(typeof(XDataCommand))]
namespace IronMan.CAD.Demo.Command
{
    class XDataCommand : CommandBase
    {
        [CommandMethod(nameof(AppendData))]
        public void AppendData()
        {
            var pEntityOptions = new PromptEntityOptions("选择挂载数据的对象");
            var pEntityResult = Editor.GetEntity(pEntityOptions);
            if (pEntityResult.Status != PromptStatus.OK)
            {
                return;
            }
            var dataName = "MyKey";
            Database.NewTransaction(trans =>
            {
                var entity = trans.GetObject(pEntityResult.ObjectId, OpenMode.ForWrite);
                var rATable = (RegAppTable)trans.GetObject(Database.RegAppTableId, OpenMode.ForWrite);
                if (!rATable.Has(dataName))
                {
                    var rATRecord = new RegAppTableRecord();
                    rATRecord.Name = dataName;
                    rATable.Add(rATRecord);
                    trans.AddNewlyCreatedDBObject(rATRecord, true);
                }
                else
                {
                    var table = rATable[dataName];
                    var record = (RegAppTableRecord)trans.GetObject(table, OpenMode.ForRead);
                    var id = record.ExtensionDictionary;
                    //var value = trans.GetObject(id, OpenMode.ForRead);
                    //foreach (var pair in db)
                    //{
                    //    Editor.WriteMessage($"\n{pair.Key}:{pair.Value}");
                    //}
                }

            });

            Database.NewTransaction(trans =>
            {
                var entity = trans.GetObject(pEntityResult.ObjectId, OpenMode.ForWrite);
                var rATable = (RegAppTable)trans.GetObject(Database.RegAppTableId, OpenMode.ForWrite);

                var resBuffer = new ResultBuffer
                {
                    new TypedValue((int)DxfCode.ExtendedDataRegAppName, dataName),
                    new TypedValue((int)DxfCode.ExtendedDataAsciiString, "This is a string"),
                    new TypedValue((int)DxfCode.ExtendedDataReal, 3.14),
                    new TypedValue((int)DxfCode.ExtendedDataInteger32, 123)
                };
                entity.XData = resBuffer;

            });

        }


        [CommandMethod(nameof(ReadData))]
        public void ReadData()
        {
            var pEntityOptions = new PromptEntityOptions("选择挂载数据的对象");
            var pEntityResult = Editor.GetEntity(pEntityOptions);
            if (pEntityResult.Status != PromptStatus.OK)
            {
                return;
            }
            Database.NewTransaction(trans =>
            {
                var entity = (Entity)trans.GetObject(pEntityResult.ObjectId, OpenMode.ForRead);
                var data = entity.XData;
                if (data != null)
                {
                    Editor.WriteMessage($"\n{data}");
                    foreach (var item in data)
                    {
                        Editor.WriteMessage($"\n{item.TypeCode}:{item.Value}");
                    }
                }
            });
        }

        [CommandMethod(nameof(DeleteData))]
        public void DeleteData()
        {
            var pEntityOptions = new PromptEntityOptions("选择挂载数据的对象");
            var pEntityResult = Editor.GetEntity(pEntityOptions);
            if (pEntityResult.Status != PromptStatus.OK)
            {
                return;
            }
            Database.NewTransaction(trans =>
            {
                var entity = (Entity)trans.GetObject(pEntityResult.ObjectId, OpenMode.ForRead);
                var data = entity.XData;
                if (data != null)
                {
                    Editor.WriteMessage($"\n{data}");
                    foreach (var item in data)
                    {
                        Editor.WriteMessage($"\n{item.TypeCode}:{item.Value}");
                    }
                }
            });
        }
    }
}
