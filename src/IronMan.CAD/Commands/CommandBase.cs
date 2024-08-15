using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Commands;

internal class CommandBase
{
    public static Document Document = Application.DocumentManager.MdiActiveDocument;
    public static Editor Editor = Document.Editor;
    public static Database Database = Document.Database;
}
