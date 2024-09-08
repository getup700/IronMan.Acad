using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using IronMan.Abstract.Acad;
using IronMan.Acad.Demo.Command;
using IronMan.Acad.Demo.Models.Fireworks;
using IronMan.Acad.Demo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(ShellCommand))]
namespace IronMan.Acad.Demo.Command
{
    internal class ShellCommand : CommandBase
    {
        public static TransientManager Tm = TransientManager.CurrentTransientManager;

        [CommandMethod(nameof(Fire0))]
        public void Fire0()
        {
            var prompt = Editor.GetPoint("");
            while (prompt.Status == PromptStatus.OK)
            {
                var bomb = new ClusterBomb0(prompt.Value);
                bomb.Fire();
                prompt = Editor.GetPoint("");
            }
        }

        [CommandMethod(nameof(Fire))]
        public async void Fire()
        {
            var pointOptionResult = Editor.GetPoint("");
            if (pointOptionResult.Status == PromptStatus.OK)
            {
                //            var son = new Shell(pointOptionResult.Value, new Vector3d(1, 1, 0), 5,
                //Color.FromRgb(255, 0, 0), 1);
                //            var son2 = new Shell(pointOptionResult.Value, new Vector3d(-1, -1, 0), 5,
                //Color.FromRgb(255, 0, 0), 1);
                //            var shells = new List<Shell>();
                //            shells.Add(son);
                //            shells.Add(son2);
                //            var tasks = shells.Select(x => x.Fire()).ToArray();
                //            await Task.WhenAll(tasks);

                //await new ClusterBomb().FireShells(pointOptionResult.Value);
                var clusterBomb = new ClusterBomb(pointOptionResult.Value);
                await clusterBomb.FireShells();

            }
        }


        [CommandMethod(nameof(FireShell))]
        public async void FireShell()
        {
            var pointOptionResult = Editor.GetPoint("");
            if (pointOptionResult.Status == PromptStatus.OK)
            {
                var son = new Shell(pointOptionResult.Value, new Vector3d(1, 1, 0), 5,
                    Color.FromRgb(255, 0, 0), 1);
                ThreadUtil.Print(nameof(FireShell));
                await son.Fire();
            }
        }

        [CommandMethod(nameof(MainAsync))]
        public async void MainAsync()
        {
            ThreadUtil.Print("Start");
            var result = await Task.Run(HeavyJob);
            Editor.WriteMessage($"\nresult:{result}");
            ThreadUtil.Print("End");
        }

        private int HeavyJob()
        {
            ThreadUtil.Print("HeavyJob");
            Thread.Sleep(3000);
            return 3000;
        }

    }
}
