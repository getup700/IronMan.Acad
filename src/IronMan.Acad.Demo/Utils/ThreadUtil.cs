using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IronMan.Acad.Demo.Utils
{
    class ThreadUtil
    {
        public static void Print(string message = null)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine(id);
            Application.DocumentManager.CurrentDocument.Editor.WriteMessage($"\n{message}-当前方法所在线程Id:{id}");
        }
    }
}
