using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using IronMan.Acad.Demo.BasicApi;

[assembly: CommandClass(typeof(LayerTableRecordCommand))]
namespace IronMan.Acad.Demo.BasicApi;

internal class LayerTableRecordCommand : CommandBase
{
    [CommandMethod(nameof(GetLayerTableRecord), CommandFlags.Modal | CommandFlags.NoBlockEditor | CommandFlags.NoUndoMarker)]
    public void GetLayerTableRecord()
    {
        using var ts = Database.TransactionManager.StartOpenCloseTransaction();

        //Database中存有各种表
        var table = (LayerTable)ts.GetObject(Database.LayerTableId, OpenMode.ForRead);
        foreach (var item in table)
        {
            var record = (LayerTableRecord)ts.GetObject(item, OpenMode.ForRead);
            Editor.WriteMessage($"\n {record.Name}");
        }
        ts.Abort();

    }

    [CommandMethod(nameof(CreateLayerTableRecord), CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void CreateLayerTableRecord()
    {
        try
        {
            using var ts = Database.TransactionManager.StartTransaction();
            var table = (LayerTable)ts.GetObject(Database.LayerTableId, OpenMode.ForWrite);
            if (table.Has("test2"))
            {
                ts.Abort();
                return;
            }
            using var record = new LayerTableRecord();
            record.Name = "test2";
            record.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
            var recordId = table.Add(record);
            ts.AddNewlyCreatedDBObject(record, true);
            Database.Clayer = recordId;
            ts.Commit();
        }
        catch (System.Exception e)
        {
            Editor.WriteMessage(e.Message);
        }

    }

    [CommandMethod(nameof(DeleteLayerTableRecord), CommandFlags.Modal | CommandFlags.NoBlockEditor)]
    public void DeleteLayerTableRecord()
    {
        try
        {
            using var ts = Database.TransactionManager.StartTransaction();
            //LayerTable没有改
            var table = (LayerTable)ts.GetObject(Database.LayerTableId, OpenMode.ForRead);
            if (!table.Has("test2"))
            {
                ts.Abort();
                return;
            }
            var recordId = table["test2"];
            if (Database.Clayer == recordId)
            {
                throw new System.Exception("can not delete current layer");
            }
            //LayerTableRecord修改了
            var layer = (LayerTableRecord)ts.GetObject(recordId, OpenMode.ForWrite);
            if (layer.IsUsed)
            {
                throw new System.Exception("layer is using");
            }
            layer.Erase();
            if (layer.IsErased)
            {
                Editor.WriteMessage("layer is erased");
            }

            ts.Commit();
        }
        catch (System.Exception e)
        {
            Editor.WriteMessage(e.Message);
        }

    }

}
