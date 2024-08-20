using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Demo.BasicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

[assembly: CommandClass(typeof(HelloCommand))]
namespace IronMan.CAD.Demo.BasicApi;

public class HelloCommand
{
    [CommandMethod(nameof(WelcomeCommand), CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void WelcomeCommand()
    {
        MessageBox.Show("Hello CAD");
    }


}
