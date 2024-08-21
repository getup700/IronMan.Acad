using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Demo.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace IronMan.CAD.Demo.Algorithm
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

            var sortedByPolarAnglePoints = new List<Point3d>();

            RecursiveByPolarAngle(ref sortedByPolarAnglePoints, startPoint, sortedPoints);
            var result = new List<Line>();
            foreach (var point in sortedByPolarAnglePoints)
            {
                var index = sortedByPolarAnglePoints.IndexOf(point);
                if (index != sortedByPolarAnglePoints.Count - 1)
                {
                    result.Add(new Line(point, sortedByPolarAnglePoints[index + 1]));

                }
                else
                {
                    result.Add(new Line(point, sortedByPolarAnglePoints[0]));
                }
            }
            return result;

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
