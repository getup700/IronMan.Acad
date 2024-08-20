using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Demo.BasicApi;
using IronMan.CAD.Demo.新文件夹;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(CalculateAreaCommand))]
namespace IronMan.CAD.Demo.新文件夹
{
    internal class CalculateAreaCommand : CommandBase
    {
        [CommandMethod(nameof(CalculateAreaCommand))]
        public void CalculateArea()
        {
            
        }
    }
}
