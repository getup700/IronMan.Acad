using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Dictionary))]
namespace IronMan.Acad.Demo.BasicApi
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
