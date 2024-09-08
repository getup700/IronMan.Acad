using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.Acad.Demo.Models.Fireworks
{
    internal class ClusterBomb : IDisposable
    {
        public List<Shell> shells = [];
        Random random = new Random();
        public ClusterBomb(Point3d point)
        {
            var shellCount = random.Next(50, 60);
            var speed = random.Next(50, 70);
            var decayRate = random.NextDouble();
            for (int i = 0; i < shellCount; i++)
            {
                var orientation = new Vector3d(random.NextDouble() - 0.5,
                    random.NextDouble() - 0.5,
                    random.NextDouble() - 0.5);
                shells.Add(new Shell(point, orientation, speed, Color.FromRgb(255, 0, 0), decayRate));
            }
        }
        //public ClusterBomb()
        //{

        //}
        //public async Task FireShells(Point3d point)
        //{
        //    var shellCount = random.Next(5, 10);
        //    var speed = random.Next(50, 70);
        //    var decayRate = random.NextDouble() * 0.1;
        //    for (int i = 0; i < shellCount; i++)
        //    {
        //        var orientation = new Vector3d(random.NextDouble() - 0.5,
        //            random.NextDouble() - 0.5,
        //            random.NextDouble() - 0.5);
        //        shells.Add(new Shell(point, orientation, speed, Color.FromRgb(255, 0, 0), decayRate));
        //    }
        //    var tasks = shells.Select(x => x.Fire()).ToArray();
        //    await Task.WhenAll(tasks);
        //}

        public async Task FireShells()
        {
            if (shells == null)
            {
                return;
            }
            var tasks = shells.Select(x => x.Fire()).ToArray();
            //var tasks = new List<Task>();
            //foreach (var item in shells)
            //{
            //    tasks.Add(item.Fire());
            //}
            await Task.WhenAll(tasks);
            //Dispose();
        }

        public void Dispose()
        {
            if (shells != null)
            {
                foreach (var item in shells)
                {
                    item?.Dispose();
                }
            }
            shells?.Clear();
        }
    }
}
