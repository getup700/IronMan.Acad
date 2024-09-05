using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using IronMan.Acad.Demo.BasicApi;
using IronMan.Acad.Demo.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly: CommandClass(typeof(TableReaderCommand))]
namespace IronMan.Acad.Demo.BasicApi
{
    class TableReaderCommand : CommandBase
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
                //扩展数据
                var RegAppTable = (RegAppTable)trans.GetObject(Database.RegAppTableId, OpenMode.ForRead);
                var TextStyleTable = (TextStyleTable)trans.GetObject(Database.TextStyleTableId, OpenMode.ForRead);
                var UcsTable = (UcsTable)trans.GetObject(Database.UcsTableId, OpenMode.ForRead);
                var ViewTable = (ViewTable)trans.GetObject(Database.ViewTableId, OpenMode.ForRead);
                var ViewportTable = (ViewportTable)trans.GetObject(Database.ViewportTableId, OpenMode.ForRead);
            });
        }




        [CommandMethod(nameof(ReadDimStyleTable))]
        public void ReadDimStyleTable()
        {
            Database.NewTransaction(trans =>
            {
                var table = (DimStyleTable)trans.GetObject(Database.DimStyleTableId, OpenMode.ForRead);
                foreach (var item in table)
                {
                    var dimStyle = (DimStyleTableRecord)trans.GetObject(item, OpenMode.ForRead);
                    Editor.WriteMessage($"{dimStyle.Name}");
                }
            });
        }

        [CommandMethod(nameof(ReadLinetypeTable))]
        public void ReadLinetypeTable()
        {
            Database.NewTransaction(trans =>
            {
                var table = (LinetypeTable)trans.GetObject(Database.LinetypeTableId, OpenMode.ForRead);
                foreach (var item in table)
                {
                    var linetype = (LinetypeTableRecord)trans.GetObject(item, OpenMode.ForRead);
                    Editor.WriteMessage(linetype.Name);
                }
            });
        }

        [CommandMethod(nameof(ReadLayerTable))]
        public void ReadLayerTable()
        {
            Database.NewTransaction(trans =>
            {
                var layerTable = (LayerTable)trans.GetObject(Database.LayerTableId, OpenMode.ForRead);
                foreach (var item in layerTable)
                {
                    var layer = (LayerTableRecord)trans.GetObject(item, OpenMode.ForRead);
                    Editor.WriteMessage($"{layer.Name}");
                }
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
