using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Commands;
using IronMan.CAD.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(SelectCommand))]
namespace IronMan.CAD.Commands;

internal class SelectCommand
{
    public static Document Document = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
    public static Editor Editor = Document.Editor;
    public static Database Database = Document.Database;

    [CommandMethod(nameof(PickObject), CommandFlags.Modal)]
    public void PickObject()
    {
        var options = new PromptEntityOptions("\n 选择实体");
        //允许选中锁定图元
        options.AllowObjectOnLockedLayer = true;

        var result = Editor.GetEntity(options);
        if (result.Status == PromptStatus.OK)
        {
            var selectedId = result.ObjectId;
            using var transaction = Document.TransactionManager.StartOpenCloseTransaction();
            var dbObject = transaction.GetObject(selectedId, OpenMode.ForRead);
            Editor.WriteMessage(dbObject.DbToString());
        }
    }

    [CommandMethod(nameof(PickNestedObject), CommandFlags.Modal)]
    public void PickNestedObject()
    {
        var options = new PromptNestedEntityOptions("\n 选择嵌套实体");
        //允许以非交互形式选择点
        //或者说，获取某点上的图元，如果没有则返回
        options.UseNonInteractivePickPoint = true;
        options.NonInteractivePickPoint = Point3d.Origin;

        var result = Editor.GetNestedEntity(options);
        if (result.Status == PromptStatus.OK)
        {
            var selectedId = result.ObjectId;
            using var transaction = Document.TransactionManager.StartOpenCloseTransaction();
            var dbObject = transaction.GetObject(selectedId, OpenMode.ForRead);
            Editor.WriteMessage(dbObject.DbToString());
        }
    }

    [CommandMethod(nameof(PickObjects), CommandFlags.Modal)]
    public void PickObjects()
    {
        var options = new PromptSelectionOptions();
        //只选择一次
        options.SingleOnly = true;
        //选择完自动空格确认
        options.SinglePickInSpace = true;


        var result = Editor.GetSelection(options);
        if (result.Status == PromptStatus.OK)
        {
            var selectedId = result.Value;
            using var transaction = Document.TransactionManager.StartOpenCloseTransaction();
            //var dbObject = transaction.GetObject(selectedId, OpenMode.ForRead);
            Editor.WriteMessage($"你选择了{selectedId.Count.ToString()}个图元");
        }
    }

    [CommandMethod(nameof(PickObjectsWithFilter), CommandFlags.Modal | CommandFlags.NoUndoMarker)]
    public void PickObjectsWithFilter()
    {
        var filter = new SelectionFilter([
            //new TypedValue((int)DxfCode.Start,"Line"),
            //new TypedValue((int)DxfCode.Start,"text"),
            //new TypedValue((int)DxfCode.Start,"mtext"),
            //new TypedValue((int)DxfCode.Start,"lwpolyline"),

            //过滤图层*Test*上的实体
            //new TypedValue((int)DxfCode.LayerName,"Test"),

            //通过实体颜色过滤，而不是图层的颜色
            //new TypedValue((int)DxfCode.Color,2),

            ////通过算术运算符过滤
            //new TypedValue((int)DxfCode.Start,"circle"),
            //new TypedValue((int)DxfCode.Operator,">="),
            ////过滤半径大于等于300的圆
            //new TypedValue(40,100),

            ////通过逻辑运算符过滤,总是成对出现的。默认的是and
            //new TypedValue((int)DxfCode.Operator,"<OR"),
            //new TypedValue((int)DxfCode.Start,"circle"),
            //new TypedValue((int)DxfCode.Start,"line"),
            //new TypedValue((int)DxfCode.Start,"Arc"),
            //new TypedValue((int)DxfCode.Operator,"OR>"),

            //通过通配符过滤
            //new TypedValue((int)DxfCode.Start,"line,circle,arc,text"),
            new TypedValue((int)DxfCode.Text,"*行*"),

        ]);

        var options = new PromptSelectionOptions()
        {
            SingleOnly = true,
            SinglePickInSpace = true,
        };

        var result = Editor.GetSelection(options, filter);

        if (result.Status == PromptStatus.OK)
        {
            var selectedObjects = result.Value;
            Editor.SetImpliedSelection(selectedObjects);
            Editor.WriteMessage(selectedObjects.Count.ToString());

        }
        else
        {
            Editor.WriteMessage(result.Status.ToString());
        }
    }

    [CommandMethod(nameof(PickObjectWithLayout), CommandFlags.Modal | CommandFlags.NoBlockEditor | CommandFlags.NoUndoMarker)]
    public void PickObjectWithLayout()
    {
        var filter = new SelectionFilter([
            new TypedValue((int)DxfCode.LayoutName,LayoutManager.Current.CurrentLayout),
            ]);
        var result = Editor.SelectAll(filter);
        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage($"\n {result.Value.Count}");

        }
    }

}
