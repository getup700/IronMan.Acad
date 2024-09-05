using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using IronMan.Acad.Demo.Algorithm;
using IronMan.Acad.Demo.BasicApi;
using IronMan.Acad.Demo.Command;
using IronMan.Acad.Demo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

[assembly: CommandClass(typeof(ConverHullAlgoCommand))]
namespace IronMan.Acad.Demo.Command
{
    internal class ConverHullAlgoCommand : CommandBase
    {
        [CommandMethod(nameof(GrahamPoints))]
        public void GrahamPoints()
        {
            using var trans = Database.TransactionManager.StartTransaction();
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
            var points = lines.SelectMany(line => new[] { line.StartPoint, line.EndPoint })
                .Distinct()
                .ToList();
            var startPoint = points.OrderBy(p => p.Y).ThenBy(p => p.X).First();

            var sortedPoints = points
                .Where(p => p != startPoint)
                .OrderBy(p => Math.Atan2(p.Y - startPoint.Y, p.X - startPoint.X))
                .ToList();
            // 事务提交以释放模型空间的锁定
            trans.Commit();
            sortedPoints.Insert(0, startPoint);

            // 逐个显示圆
            foreach (var point in sortedPoints)
            {
                using (Transaction tr = Database.TransactionManager.StartTransaction())
                {
                    var blockTable1 = (BlockTable)tr.GetObject(Database.BlockTableId, OpenMode.ForRead);
                    var modelSpace1 = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    var circle = new Circle(point, Vector3d.ZAxis, 10);

                    modelSpace1.AppendEntity(circle);
                    tr.AddNewlyCreatedDBObject(circle, true);
                    var dbText = new DBText
                    {
                        Position = point.Add(new Vector3d(20, 0, 0)),
                        Height = 30,
                        TextString = $"{sortedPoints.IndexOf(point)}",
                        Color = Color.FromRgb(255, 0, 0)
                    };
                    modelSpace1.AppendEntity(dbText);
                    tr.AddNewlyCreatedDBObject(dbText, true);

                    tr.Commit(); // 提交事务以在AutoCAD中显示圆
                }
                Editor.UpdateScreen();

                // 可选：在每个圆显示后添加一个延迟
                Thread.Sleep(500); // 延迟 500 毫秒
            }


            var stack = new Stack<Point3d>();
            stack.Push(sortedPoints[0]);
            stack.Push(sortedPoints[1]);


            var source = sortedPoints[1];
            var algo = new ConverHullAlgorithm();
            for (int i = 2; i < sortedPoints.Count; i++)
            {
                stack.Push(sortedPoints[i]);

                var next = new Point3d();
                if (i == sortedPoints.Count - 1)
                {
                    next = sortedPoints[0];
                }
                else
                {
                    next = sortedPoints[i + 1];
                }

                var before = stack.ElementAt(stack.Count - 2);

                var vector1 = stack.Peek() - before;
                var vector2 = next - stack.Peek();

                //vector1在vector2的左侧，则side<0
                var side = vector2.CrossProduct(vector1).Z;
                if (side > 0)
                {
                    stack.Pop();
                    stack.Push(next);
                    i++;
                    continue;
                }
                else if (side < 0)
                {

                    continue;
                }
            }

            var list = stack.ToList();
            list.Reverse();
            for (int i = 0; i < list.Count - 1; i++)
            {
                Database.NewTransaction(trans =>
                {
                    var blockTable1 = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                    var modelSpace1 = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    var line = new Line(list[i], list[i + 1]);
                    line.Color = Color.FromRgb(255, 0, 0);
                    modelSpace1.AppendEntity(line);
                    trans.AddNewlyCreatedDBObject(line, true);
                    Editor.UpdateScreen();
                    Thread.Sleep(500);
                });
            }
            Database.NewTransaction(trans =>
            {
                var blockTable1 = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var modelSpace1 = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                var line = new Line(list.Last(), list.First());
                line.Color = Color.FromRgb(255, 0, 0);
                modelSpace1.AppendEntity(line);
                trans.AddNewlyCreatedDBObject(line, true);
                Editor.UpdateScreen();
                Thread.Sleep(500);
            });
        }

    }
}
