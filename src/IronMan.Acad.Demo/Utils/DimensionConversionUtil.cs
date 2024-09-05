using Autodesk.AutoCAD.Geometry;

namespace IronMan.Acad.Demo.Utils
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
