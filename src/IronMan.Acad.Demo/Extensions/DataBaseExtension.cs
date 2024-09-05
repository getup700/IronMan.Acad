using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace IronMan.Acad.Demo.Extensions
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
