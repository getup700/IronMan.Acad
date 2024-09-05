using Autodesk.AutoCAD.Runtime;
using IronMan.Acad.Demo.BasicApi;
using System.Windows;

[assembly: CommandClass(typeof(HelloCommand))]
namespace IronMan.Acad.Demo.BasicApi;

public class HelloCommand
{
    [CommandMethod(nameof(WelcomeCommand), CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void WelcomeCommand()
    {
        MessageBox.Show("Hello CAD");
    }


}
