using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using IronMan.Acad.Demo.Algorithm;
using IronMan.Acad.Demo.BasicApi;
using IronMan.Acad.Demo.Command.Randoms;
using IronMan.Acad.Demo.Extensions;

[assembly: CommandClass(typeof(AutoPointsCommand))]
namespace IronMan.Acad.Demo.Command.Randoms
{
    internal class AutoPointsCommand : CommandBase
    {
        [CommandMethod(nameof(RandomLines))]
        public void RandomLines()
        {
            var options = new PromptIntegerOptions("生成线的数量");
            options.LowerLimit = 3;
            var prompt = Editor.GetInteger(options);
            if (prompt.Status != PromptStatus.OK)
            {
                return;
            }
            var lines = RandomObject.RandomLines(prompt.Value);
            Database.NewTransaction(trans =>
            {
                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                foreach (var line in lines)
                {
                    blockTableRecord.AppendEntity(line);
                    trans.AddNewlyCreatedDBObject(line, true);
                }
            });

        }


    }
}
