using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Layout))]
namespace IronMan.Acad.Demo.BasicApi
{
    internal class LayoutCommand : CommandBase
    {
        [CommandMethod(nameof(GetLayout), CommandFlags.Modal | CommandFlags.NoBlockEditor | CommandFlags.NoUndoMarker)]
        public void GetLayout()
        {
            using var ts = Document.TransactionManager.StartTransaction();
            var nod = (DBDictionary)Database.NamedObjectsDictionaryId.GetObject(OpenMode.ForRead);
            foreach (var item in nod)
            {
                Editor.WriteMessage($"\n {item.Key}");
            }

            //获取所有布局
            var layouts = Database.LayoutDictionaryId.GetObject(OpenMode.ForWrite) as DBDictionary;
            foreach (var item in layouts)
            {
                var layout = item.Value.GetObject(OpenMode.ForRead) as Layout;
                Editor.WriteMessage($"\n {layout.LayoutName}");
            }
            ts.Abort();
        }

        [CommandMethod(nameof(CreateLayout), CommandFlags.Modal | CommandFlags.NoBlockEditor | CommandFlags.NoUndoMarker)]
        public void CreateLayout()
        {
            var name = "新的布局";
            using var ts = Document.TransactionManager.StartTransaction();
            var layouts = Database.LayoutDictionaryId.GetObject(OpenMode.ForWrite) as DBDictionary;
            if (layouts.Contains(name))
            {
                ts.Commit();
                return;
            }
            var table = Database.BlockTableId.GetObject(OpenMode.ForWrite) as BlockTable;
            var record = new BlockTableRecord()
            {
                //创建布局特定的名称格式
                Name = $"*Paper_Space{layouts.Count - 1}",
            };
            table.Add(record);
            ts.AddNewlyCreatedDBObject(record, true);
            using var layout = new Layout();
            layout.LayoutName = name;

            layout.AddToLayoutDictionary(Database, record.Id);
            ts.AddNewlyCreatedDBObject(layout, true);
            Editor.Regen();
            ts.Commit();
        }

        [CommandMethod(nameof(CreateLayoutByManager), CommandFlags.Modal | CommandFlags.NoBlockEditor | CommandFlags.NoUndoMarker)]
        public void CreateLayoutByManager()
        {
            var name = "新的布局1";
            using var ts = Document.TransactionManager.StartTransaction();
            if (!LayoutManager.Current.LayoutExists(name))
            {
                LayoutManager.Current.CreateLayout(name);
                LayoutManager.Current.CurrentLayout = name;
            }
            Editor.Regen();
            ts.Commit();
        }
    }
}
