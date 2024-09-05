using Autodesk.AutoCAD.Customization;

namespace IronMan.Acad.Demo.Extensions
{
    public class MecroData
    {
        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Internal command method name
        /// </summary>
        public string CommandName { get; set; }

        public string ImagePath { get; set; }

        public MacroGroup ParentMacroGroup { get; set; }

        public string Tag { get; set; }

        public string GroupName { get; set; }
    }
}
