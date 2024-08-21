using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Demo.Algorithm;
using IronMan.CAD.Demo.BasicApi;
using IronMan.CAD.Demo.Command;
using IronMan.CAD.Demo.Extensions;
using IronMan.CAD.Demo.新文件夹;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

[assembly: CommandClass(typeof(ConverHullAlgoCommand))]
namespace IronMan.CAD.Demo.Command
{
    internal class ConverHullAlgoCommand : CommandBase
    {
        [CommandMethod(nameof(GrahamPoints))]
        public void GrahamPoints()
        {
            Database.NewTransaction(trans =>
            {
                var blockTable = trans.GetObject(Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                var modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var lines = new List<Line>();
                foreach (ObjectId objId in modelSpace)
                {
                    var entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;

                    if (entity is Line line)
                    {
                        lines.Add(line);
                    }
                }
                var points = lines.SelectMany(line => new[] { line.StartPoint, line.EndPoint }).ToList();
                var converHull = new ConverHullAlgorithm();
                var resultLines = converHull.Graham(points);
                foreach (Line line in resultLines)
                {
                    line.Color = Color.FromRgb(255, 0, 0);
                    modelSpace.AppendEntity(line);
                    trans.AddNewlyCreatedDBObject(line,true);
                }
            });
        }

    }
}
