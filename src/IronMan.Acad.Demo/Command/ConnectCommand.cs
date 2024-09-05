using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using IronMan.Acad.Demo.Command;
using System;
using System.Runtime.InteropServices;

[assembly: CommandClass(typeof(ConnectCommand))]
namespace IronMan.Acad.Demo.Command
{
    public class ConnectCommand
    {
        [CommandMethod(nameof(ConnectToAcad))]
        public static void ConnectToAcad()
        {
            var version = Autodesk.AutoCAD.ApplicationServices.Core.Application.Version;
            AcadApplication acAppComObj = null;
            const string strProgId = "AutoCAD.Application.24.2";

            // Get a running instance of AutoCAD
            try
            {
                acAppComObj = (AcadApplication)Marshal.GetActiveObject(strProgId);
            }
            catch // An error occurs if no instance is running
            {
                try
                {
                    // Create a new instance of AutoCAD
                    acAppComObj = (AcadApplication)Activator.CreateInstance(Type.GetTypeFromProgID(strProgId), true);
                }
                catch
                {
                    // If an instance of AutoCAD is not created then message and exit
                    System.Windows.Forms.MessageBox.Show("Instance of 'AutoCAD.Application'" +
                                                         " could not be created.");

                    return;
                }
            }

            // Display the application and return the name and version
            acAppComObj.Visible = true;
            System.Windows.Forms.MessageBox.Show("Now running " + acAppComObj.Name +
                                                 " version " + acAppComObj.Version);

            // Get the active document
            AcadDocument acDocComObj;
            acDocComObj = acAppComObj.ActiveDocument;

            // Optionally, load your assembly and start your command or if your assembly
            // is demandloaded, simply start the command of your in-process assembly.
            acDocComObj.SendCommand("(command " + (char)34 + "NETLOAD" + (char)34 + " " +
                                    (char)34 + "c:/myapps/mycommands.dll" + (char)34 + ") ");

            acDocComObj.SendCommand("MyCommand ");
        }

    }
}
