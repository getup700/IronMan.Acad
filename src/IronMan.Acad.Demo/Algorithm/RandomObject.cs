using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace IronMan.Acad.Demo.Algorithm
{
    internal static class RandomObject
    {
        internal static Random random = new Random();
        public static List<Line> RandomLines(int count, int minAxisVaue = 0, int maxAxisVaue = 1000)
        {
            if (count == 0) return null;
            var result = new List<Line>();
            for (int i = 0; i < count; i++)
            {
                result.Add(RandomLine(minAxisVaue, maxAxisVaue));
            }
            return result;
        }

        public static Line RandomLine(int minAxisValue = 0, int maxAxisValue = 1000)
        {
            var x = random.Next(minAxisValue, maxAxisValue);
            var y = random.Next(minAxisValue, maxAxisValue);
            var x1 = random.Next(minAxisValue, maxAxisValue);
            var y1 = random.Next(minAxisValue, maxAxisValue);

            return new Line(new Point3d(x, y, 0), new Point3d(x1, y1, 0));
        }

        public static List<Point2d> RandomPoints(int count, int minAxisValue = 0, int maxAxisValue = 1000)
        {
            if (count == 0) return null;
            var result = new List<Point2d>();
            for (int i = 0; i < count; i++)
            {
                var x = random.Next(minAxisValue, maxAxisValue);
                var y = random.Next(minAxisValue, maxAxisValue);
                result.Add(new Point2d(x, y));
            }
            return result;
        }

        public static Point2d RandomPoint(int maxAxisValue = 1000)
        {
            var x = random.Next(0, maxAxisValue);
            var y = random.Next(0, maxAxisValue);
            return new Point2d(x, y);
        }
    }
}
