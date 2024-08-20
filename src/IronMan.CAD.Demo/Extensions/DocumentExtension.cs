using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Demo.Extensions
{
    internal static class DocumentExtension
    {
        public static ObjectId GetOrCreateLayer(this Document document, string name)
        {
            var layerTable = (LayerTable)document.Database.LayerTableId.GetObject(OpenMode.ForWrite);
            if (!layerTable.Has(name))
            {
                using var record = new LayerTableRecord();
                record.Name = name;
                record.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                layerTable.Add(record);
                document.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(record, true);

            }
            return layerTable[name];
        }
    }
}
