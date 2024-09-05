using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Stark.Extensions.Acad;
using System;
using System.Collections.Generic;
using System.Linq;
using Exception = System.Exception;

namespace IronMan.Acad.Demo.Algorithm
{
    internal class ConverHullAlgorithm
    {
        public List<Line> Graham(ICollection<Point3d> points)
        {
            if (points == null || points.Count < 3)
            {
                throw new Exception("Conver hull algorithm requires at least three points");
            }
            //起点为y轴最小，x轴最小的点
            var startPoint = points.OrderBy(p => p.Y).OrderBy(p => p.X).First();

            //对所有点排序：按起点与其他点连线与x轴的极角(arctan)正向排序
            var sortedPoints = points
                .Where(p => p != startPoint)
                .OrderBy(p => Math.Atan2(p.Y - startPoint.Y, p.X - startPoint.X))
                .ToList();
            sortedPoints.Insert(0, startPoint);
            var resultPoints = new List<Point3d>();
            var origin = sortedPoints[1];
            for (int i = 0; i < sortedPoints.Count; i++)
            {
                if (i < 2)
                {
                    resultPoints.Add(sortedPoints[i]);
                    origin = resultPoints[i];
                    continue;
                }
                else if (i > sortedPoints.Count - 2)
                {
                    resultPoints.Add(sortedPoints[i - 2]);
                    break;
                }
                else
                {
                    var side = GeometryUtil.DetermineSide(origin, sortedPoints[i], sortedPoints[i + 1]);
                    //i+1在i的右侧,放弃i点,i+1为下一个判断的原点
                    if (side > 0)
                    {
                        resultPoints.Add(sortedPoints[i + 1]);
                        origin = sortedPoints[i + 1];
                        continue;
                    }
                    else
                    {
                        i++;
                        continue;
                    }
                }
            }

            var resultLines = new List<Line>();
            for (int i = 0; i < resultPoints.Count - 1; i++)
            {
                var line = new Line(resultPoints[i], resultPoints[i + 1]);
                resultLines.Add(line);
            }

            return resultLines;

        }

        private void RecursiveByPolarAngle(ref List<Point3d> result, Point3d startPoint, List<Point3d> points)
        {
            if (points == null || points.Count == 0)
            {
                throw new Exception();
            }
            result.Add(startPoint);

            var sortedPoints = points
                .Where(p => p != startPoint)
                .OrderBy(p => Math.Atan2(p.Y - startPoint.Y, p.X - startPoint.X))
                .ToList();
            var nextPoint = sortedPoints.First();
            sortedPoints.Remove(nextPoint);
            if (sortedPoints.Count > 1)
            {
                RecursiveByPolarAngle(ref result, nextPoint, sortedPoints);
            }
        }

        private Point3d MinPolarAngle(Point3d startPoint, ICollection<Point3d> points)
        {
            if (points == null || points.Count == 0)
            {
                throw new Exception();
            }
            var result = points
                .Where(p => p != startPoint)
                .OrderBy(p => Math.Atan2(p.Y - startPoint.Y, p.X - startPoint.X))
                .First();
            return result;
        }
    }
}
