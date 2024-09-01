using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Demo.BasicApi;
using IronMan.CAD.Demo.Command;
using IronMan.CAD.Demo.Extensions;
using Stark.Extensions.CAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(AnnotatedAreaCommand))]
namespace IronMan.CAD.Demo.Command
{
    internal class AnnotatedAreaCommand : CommandBase
    {

        [CommandMethod(nameof(AnnotatedAreaByLayer))]
        public void AnnotatedAreaByLayer()
        {
            var prompt = new PromptEntityOptions("\n选择一个线条以获取其图层");
            var result = Editor.GetEntity(prompt);
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            Database.NewTransaction(trans =>
            {
                var entity = (Entity)trans.GetObject(result.ObjectId, OpenMode.ForRead);
                if (entity == null)
                {
                    Editor.WriteMessage("\n选择的对象无效");
                    return;
                }

                var layerName = entity.Layer;

                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var modelSpace = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                var units = Database.Insunits;

                foreach (var id in modelSpace)
                {
                    var currentEntity = (Entity)trans.GetObject(id, OpenMode.ForWrite);
                    if (currentEntity.LayerId == entity.LayerId)
                    {
                        if (currentEntity is Polyline polyline && polyline.Closed)
                        {
                            var text = new MText();
                            text.TextHeight = 10;
                            var area = polyline.Area;
                            text.Contents = $"{area:F2}{units}";
                            text.Location = GeometryUtil.CalculateCentroid(polyline);
                            modelSpace.AppendEntity(text);
                            trans.AddNewlyCreatedDBObject(text, true);
                        }
                    }
                }

            });

        }

        [CommandMethod(nameof(AnnotatedAreaByPoint))]
        public void AnnotatedAreaByPoint()
        {
            var prompt = new PromptPointOptions("\n请选择封闭区域内的一个点");
            var result = Editor.GetPoint(prompt);
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            var clickPoint = result.Value;
            Database.NewTransaction(trans =>
            {
                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var record = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (var objId in record)
                {
                    var entity = (Entity)trans.GetObject(objId, OpenMode.ForWrite);
                    if (entity is Polyline polyline)
                    {
                        if (GeometryUtil.IsPointInsidePolygon(clickPoint, polyline))
                        {
                            var area = polyline.Area;
                            var units = Database.Insunits;
                            var text = new MText();
                            text.Contents = $"{area:F2}{units}";
                            text.TextHeight = 10;
                            text.Location = clickPoint;
                            record.AppendEntity(text);
                            trans.AddNewlyCreatedDBObject(text, true);
                            clickPoint = clickPoint.Add(new Vector3d(0, 10, 0));
                        }
                    }
                }


            });
        }

    }


}
