using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Demo.BasicApi;

public class CommandBase
{
    public static Document Document = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
    public static Editor Editor = Document.Editor;
    public static Database Database = Document.Database;
    public static Window Window = Document.Window;
}
