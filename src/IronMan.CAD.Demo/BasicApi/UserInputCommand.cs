using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using IronMan.Abstract.CAD.UI;
using IronMan.CAD.Demo.BasicApi;
using System;
using System.Diagnostics;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(UserInputCommand))]
namespace IronMan.CAD.Demo.BasicApi;

[Macro(nameof(UserInputCommand))]
public class UserInputCommand : CommandBase
{
    [Macro(nameof(InputPoint))]
    [CommandMethod(nameof(InputPoint), CommandFlags.Modal)]
    public void InputPoint()
    {
        var options = new PromptPointOptions("\n在绘制窗口选择点：");
        options.Keywords.Add("Create", "C", "创建(C)");
        options.Keywords.Add("Back", "B", "返回(B)");
        //把添加的keywords添加到提示中
        options.AppendKeywordsToMessage = true;

        var options1 = new PromptPointOptions("\n在创建窗口选择点[创建(C)/返回(B)]：", "Create Back");
        //用户必须输入一个值
        options.AllowNone = false;

        var result = Editor.GetPoint(options);

        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage(result.Value.ToString());
        }
        if (result.Status == PromptStatus.Keyword)
        {
            switch (result.StringResult)
            {
                case "Create":
                    //todo
                    Editor.WriteMessage("创建的关键字");
                    break;
                case "Back":
                    //todo
                    Editor.WriteMessage("回退的关键字");
                    break;
                default:
                    break;
            }
        }
    }

    [Macro(nameof(InputPoint))]
    [CommandMethod(nameof(InputAngle), CommandFlags.Modal)]
    public void InputAngle()
    {
        var options = new PromptAngleOptions("\n 请输入角度：");

        var result = Editor.GetAngle(options);
        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage(result.Value.ToString());
        }
    }

    [CommandMethod(nameof(InputCorner), CommandFlags.Modal)]
    public void InputCorner()
    {
        var options = new PromptCornerOptions("\n 选择对角", Autodesk.AutoCAD.Geometry.Point3d.Origin);
        options.UseDashedLine = true;

        var result = Editor.GetCorner(options);
        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage(result.Value.ToString());
        }
    }

    [CommandMethod(nameof(InputString), CommandFlags.Modal)]
    public void InputString()
    {
        var options = new PromptStringOptions("\n 请输入：");
        var result = Editor.GetString(options);
        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage($"您输入了：{result.StringResult}");
        }
    }

    [CommandMethod(nameof(InputInteger), CommandFlags.Modal)]
    public void InputInteger()
    {
        var options = new PromptIntegerOptions("\n 请输入RGB");
        options.UpperLimit = 255;
        options.LowerLimit = 0;

        var result = Editor.GetInteger(options);
        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage(result.Value.ToString());
        }
    }

    [CommandMethod(nameof(InputDouble), CommandFlags.Modal)]
    public void InputDouble()
    {
        var options = new PromptDoubleOptions("\n 请输入RGB");
        options.AllowNegative = false;

        var result = Editor.GetDouble(options);
        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage(result.Value.ToString());
        }
    }

    [CommandMethod(nameof(InputDistance), CommandFlags.Modal)]
    public void InputDistance()
    {
        var options = new PromptDistanceOptions("\n 请测量");
        options.AllowNegative = false;
        options.Only2d = true;

        var result = Editor.GetDistance(options);
        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage($"总长度：{result.Value.ToString()}");
        }
    }

    [CommandMethod(nameof(InputKeywords), CommandFlags.Modal)]
    public void InputKeywords()
    {
        var options = new PromptKeywordOptions("\n 请测量");
        options.AllowNone = true;
        options.AppendKeywordsToMessage = true;
        options.Keywords.Add("Red");
        options.Keywords.Add("Yellow");
        options.Keywords.Add("Green");
        options.Keywords.Default = "Green";


        var result = Editor.GetKeywords(options);
        if (result.Status == PromptStatus.OK)
        {
            Editor.WriteMessage(result.StringResult);
        }
    }

}
