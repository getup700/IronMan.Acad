using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using IronMan.Abstract.CAD;
using IronMan.CAD.Demo.Command;
using IronMan.CAD.Demo.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(ReaderCommand))]
namespace IronMan.CAD.Demo.Command
{
    class ReaderCommand : CommandBase
    {
        [CommandMethod(nameof(ReadDoc))]
        public void ReadDoc()
        {
            Database.NewTransaction(trans =>
            {
                var BlockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var LayerTable = (LayerTable)trans.GetObject(Database.LayerTableId, OpenMode.ForRead);
                var LinetypeTable = (LinetypeTable)trans.GetObject(Database.LinetypeTableId, OpenMode.ForRead);
                var DimStyleTable = (DimStyleTable)trans.GetObject(Database.DimStyleTableId, OpenMode.ForRead);
                //扩展数据，
                var RegAppTable = (RegAppTable)trans.GetObject(Database.RegAppTableId, OpenMode.ForRead);
                var TextStyleTable = (TextStyleTable)trans.GetObject(Database.TextStyleTableId, OpenMode.ForRead);
                var UcsTable = (UcsTable)trans.GetObject(Database.UcsTableId, OpenMode.ForRead);
                var ViewTable = (ViewTable)trans.GetObject(Database.ViewTableId, OpenMode.ForRead);
                var ViewportTable = (ViewportTable)trans.GetObject(Database.ViewportTableId, OpenMode.ForRead);

            });


        }

        [CommandMethod(nameof(ReadBlockTable))]
        public void ReadBlockTable()
        {
            Database.NewTransaction(trans =>
            {
                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var modelSpace = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                var objs = trans.GetAllObjects();
                var dbObj = new List<DBObject>();
                var stringBuilder = new StringBuilder();
                foreach (var item in objs)
                {
                    //dbObj.Add(item);
                    if (item is BlockTable bTable)
                    {
                        foreach (var bt in bTable)
                        {
                            var value = trans.GetObject(bt, OpenMode.ForRead);
                            dbObj.Add(value);
                            stringBuilder.AppendLine(value.Print());
                        }
                    }
                    else if (item is BlockTableRecord btRecord)
                    {
                        foreach (var bt in btRecord)
                        {
                            var value = trans.GetObject(bt, OpenMode.ForRead);
                            dbObj.Add(value);
                            stringBuilder.AppendLine(value.Print());
                        }
                    }
                }
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "text1.txt"), stringBuilder.ToString());
                Editor.WriteMessage($"\n{dbObj.Count}");
            });
        }

        [CommandMethod(nameof(ReadLayoutDic))]
        public void ReadLayoutDic()
        {
            Database.NewTransaction(trans =>
            {
                var blockTable = (DBDictionary)trans.GetObject(Database.LayoutDictionaryId, OpenMode.ForRead);

            });
        }
    }
}
