using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using IronMan.CAD.Demo.Algorithm;
using IronMan.CAD.Demo.BasicApi;
using IronMan.CAD.Demo.Command.Random;
using IronMan.CAD.Demo.Extensions;
using IronMan.CAD.Demo.新文件夹;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(AutoPointsCommand))]
namespace IronMan.CAD.Demo.Command.Random
{
    internal class AutoPointsCommand:CommandBase
    {
        [CommandMethod(nameof(RandomLines))]
        public void RandomLines()
        {
            var options = new PromptIntegerOptions("生成线的数量");
            options.LowerLimit = 3;
            var prompt = Editor.GetInteger(options);
            if (prompt.Status !=PromptStatus.OK)
            {
                return;
            }
            var lines = RandomObject.RandomLines(prompt.Value);
            Database.NewTransaction(trans =>
            {
                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead);
                var blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace],OpenMode.ForWrite);
                foreach (var line in lines)
                {
                    blockTableRecord.AppendEntity(line);
                    trans.AddNewlyCreatedDBObject(line, true);
                }
            });

        }

       
    }
}
