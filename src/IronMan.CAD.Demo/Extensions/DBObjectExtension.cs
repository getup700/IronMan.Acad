using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Demo.Extensions
{
    public static class DBObjectExtension
    {
        public static string Print(this DBObject dBObject)
        {
            var builder = new StringBuilder();
            var type = dBObject.GetType();
            builder.AppendLine($"------------Start:{DateTime.Now}------------");
            builder.AppendLine($"type:{type}");
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                ?.OrderBy(x => x.Name);
            foreach (var property in properties)
            {
                try
                {
                    var result = property.GetValue(dBObject);
                    builder.AppendLine($"{property.Name}:{result}");
                }
                catch (Exception e)
                {
                    builder.AppendLine($"error:{property.Name},{e.Message}");
                }
            }
            builder.AppendLine($"------------End:{DateTime.Now}------------");
            builder.AppendLine();
            return builder.ToString();
        }
    }
}
