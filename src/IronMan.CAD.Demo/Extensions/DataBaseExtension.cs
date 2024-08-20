using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Demo.Extensions
{
    public static class DataBaseExtension
    {
        /// <summary>
        /// 获取当前图层
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static LayerTableRecord GetCurrentLayer(this Database db)
        {
            LayerTableRecord ltr = (LayerTableRecord)db.Clayer.GetObject(OpenMode.ForRead);
            return ltr;
        }

        /// <summary>
        /// 开启一个事务
        /// </summary>
        /// <param name="db"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Transaction NewTransaction(this Database db, Action<Transaction> action)
        {
            var transaction = db.TransactionManager.StartTransaction();
            if (transaction != null)
            {
                action(transaction);
            }
            transaction.Commit();
            return transaction;
        }

    }
}
