using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Demo.Utils
{
    internal static class GeometryUtil
    {
        /// <summary>
        /// 判断vector1在vector2的左侧还是右侧
        /// </summary>
        /// <param name="source"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>1为左侧，-1为右侧，0为共线</returns>
        public static int DetermineSide(Point3d source, Point3d point1, Point3d point2)
        {
            var vector1 = point1 - source;
            var vector2 = point2 - source;

            var crossProduct = vector1.CrossProduct(vector2);
            if (crossProduct.Z > 0)
            {
                return 1;
            }
            else if (crossProduct.Z < 0)
            {
                return -1;
            }
            return 0;
        }

       
    }
}
