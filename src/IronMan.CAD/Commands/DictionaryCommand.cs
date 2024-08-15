using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(Dictionary))]
namespace IronMan.CAD.Commands
{
    internal class DictionaryCommand : CommandBase
    {
        [CommandMethod(nameof(Add), CommandFlags.Modal | CommandFlags.NoBlockEditor | CommandFlags.NoUndoMarker)]
        public void Add()
        {
            var namedObjectDictionary = Database.NamedObjectsDictionaryId;
            using var ts = Document.TransactionManager.StartTransaction();
            var nod = namedObjectDictionary.GetObject(OpenMode.ForWrite) as DBDictionary;
            var newDIc = new DBDictionary();
            nod.SetAt("IronMan_myDic", newDIc);
            ts.AddNewlyCreatedDBObject(newDIc, true);
            ts.Commit();

            Editor.WriteMessage("\n 添加成功");
        }
    }
}
