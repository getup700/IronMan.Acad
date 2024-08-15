using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

[assembly:CommandClass(typeof(MyCommand))]
namespace IronMan.CAD.Commands;

public class MyCommand
{
    [CommandMethod(nameof(WelcomeCommand),CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void WelcomeCommand()
    {
        MessageBox.Show("Hello CAD");
    }


}
