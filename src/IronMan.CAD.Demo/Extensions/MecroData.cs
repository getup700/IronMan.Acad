using Autodesk.AutoCAD.Customization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Demo.Extensions
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
