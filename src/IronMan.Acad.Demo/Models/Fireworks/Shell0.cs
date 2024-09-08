using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.Acad.Demo.Models.Fireworks
{
    internal class Shell0 : IDisposable
    {
        /// <summary>
        /// 火星中其中的一个亮点
        /// </summary>
        public List<Circle> circles = new();

        /// <summary>
        /// 火星中心
        /// </summary>
        Circle circle;

        /// <summary>
        /// 运动方向
        /// </summary>
        Vector3d orientation;

        /// <summary>
        /// 运动速度
        /// </summary>
        double speed;

        /// <summary>
        /// 预期亮点数量
        /// </summary>
        int designCount;

        /// <summary>
        /// 实际光点数量
        /// </summary>
        int actualCount;

        /// <summary>
        /// 火星颜色，动态的
        /// </summary>
        Color color;

        /// <summary>
        /// 衰减速率
        /// </summary>
        double decayRate;

        /// <summary>
        /// 是否在燃烧
        /// </summary>
        public bool IsBurning { get; set; } = true;

        public Shell0(Point3d center, Vector3d orientation, double speed, Color color, double decayRate)
        {
            this.orientation = orientation;
            this.speed = speed;
            this.color = color;
            this.decayRate = decayRate;
            designCount = new Random().Next(40, 50);
            circle = new Circle(center, Vector3d.ZAxis, 1)
            {
                LineWeight = LineWeight.LineWeight015,
                Color = color
            };

        }

        /// <summary>
        /// 生成下一个火星
        /// </summary>
        public void Fire()
        {
            if (actualCount > designCount || speed < 0.1)
            {
                IsBurning = false;
                Extinguish();
                return;
            }
            var vt = orientation.GetNormal() * speed - Vector3d.YAxis * 10;
            circle.Center += vt;
            var newCircle = circle.Clone() as Circle;
            circles.Add(newCircle);
            TransientManager.CurrentTransientManager.AddTransient(newCircle, TransientDrawingMode.Main, 0, []);
            actualCount++;
            speed *= decayRate;

        }
        
        /// <summary>
        /// 熄灭火花
        /// </summary>
        public void Extinguish()
        {
            if (circles == null)
            {
                return;
            }
            foreach (var item in circles)
            {
                TransientManager.CurrentTransientManager.EraseTransient(item, []);
            }
        }

        public void Dispose()
        {
            circle.Dispose();
            if (circles != null)
            {
                foreach (var circle in circles)
                {
                    circle.Dispose();
                }
                circles.Clear();
            }
            color.Dispose();

        }
    }
}
