using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Demo
{
    public static class DimensionConversionUtil
    {
        public static Point3d EnhanceDimension(Point2d point)
        {
            return new Point3d(point.X, point.Y, 0);
        }

        public static Point2d ReductionDimension(Point3d point)
        {
            return new Point2d(point.X, point.Y);
        }

    }
}
