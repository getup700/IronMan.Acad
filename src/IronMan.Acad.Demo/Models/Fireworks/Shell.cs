using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using IronMan.Abstract.Acad;
using IronMan.Acad.Demo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.Acad.Demo.Models.Fireworks
{
    /// <summary>
    /// 火星，烟花中的一条
    /// </summary>
    internal class Shell : IDisposable
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

        public Shell(Point3d center, Vector3d orientation, double speed, Color color, double decayRate)
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
        /// 开火
        /// </summary>
        /// <returns></returns>
        public async Task Fire()
        {
            var orientation = this.orientation.GetNormal() * speed;
            var g = -Vector3d.YAxis * 10;
            ThreadUtil.Print("1");
            while (IsBurning)
            {
                actualCount++;
                //Application.DocumentManager.CurrentDocument.Editor.WriteMessage($"\n{actualCount}");
                speed *= decayRate;
                orientation += g;
                circle.Center += orientation;
                var circle1 = circle.Clone() as Circle;
                circles.Add(circle1);
                TransientManager.CurrentTransientManager.AddTransient(circle1, TransientDrawingMode.Main, 0, []);
                Application.DocumentManager.CurrentDocument.Editor.UpdateScreen();
                if (actualCount > designCount || speed < 0.1)
                {
                    IsBurning = false;
                }
                await Task.Delay(30);
                ThreadUtil.Print("2");
            }
            ThreadUtil.Print("3");
            Dispose();
        }

        public void Dispose()
        {
            Application.DocumentManager.CurrentDocument.Editor.WriteMessage($"\nDisposing...");
            color.Dispose();
            circle.Dispose();
            foreach (var item in circles)
            {
                TransientManager.CurrentTransientManager.EraseTransient(item, []);
                item?.Dispose();
            }
            circles.Clear();
        }
    }
}
