using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IronMan.Acad.Demo.Models.Fireworks
{
    /// <summary>
    /// 烟花
    /// </summary>
    internal class ClusterBomb0 : IDisposable
    {
        Timer timer;
        List<Shell0> shells = [];
        int k = 0;
        Random random = new Random();
        Point3d point;
        public ClusterBomb0(Point3d point)
        {
            timer = new()
            {
                Interval = 30
            };
            this.point = point;
            timer.Tick += SingleFireworkFly;
            var n = random.Next(150, 200);
            var speed = random.Next(60, 100);
            var decayRate = random.NextDouble() * 0.05 + 0.95;
            for (int i = 0; i < n; i++)
            {
                var orientation = new Vector3d(random.NextDouble() - 0.5,
                    random.NextDouble() - 0.5,
                    random.NextDouble() - 0.5);
                var newShell = new Shell0(point, orientation, speed, Color.FromRgb(255, 0, 0), decayRate);

                this.shells.Add(newShell);
            }
        }

        public void Fire()
        {
            timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FireNext()
        {
            foreach (var item in shells)
            {
                if (item.IsBurning)
                {
                    item.Fire();
                }
            }
        }

        /// <summary>
        /// 重复执行，生成下一阶段的火星，删除熄灭的火花
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SingleFireworkFly(object sender, EventArgs e)
        {
            k++;
            FireNext();

            //待删除图元
            var deleteShells = new List<Shell0>();
            for (int i = shells.Count - 1; i >= 0; i--)
            {
                var shell = shells[i];
                if (!shell.IsBurning)
                {
                    deleteShells.Add(shell);
                    shells.RemoveAt(i);
                }
            }
            //foreach (var shell in deleteShells)
            //{
            //    shell.Dispose();
            //}
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.CurrentDocument.Editor.WriteMessage($"\nscreen update {k}");
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.CurrentDocument.Editor.UpdateScreen();
            //Application.DoEvents();
            if (this.shells.Count == 0)
            {
                this.Dispose();
            }
        }

        public void Dispose()
        {
            timer.Tick -= SingleFireworkFly;
            shells.Clear();
            timer.Stop();
            timer.Dispose();
        }


    }
}
