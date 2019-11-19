using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPoker_Server.Misc
{
    static class Threading
    {
        public static void InvokeIfRequired(Action action)
        {
            if (MainWindow.dispatcher.CheckAccess()) { action.Invoke(); }
            else MainWindow.dispatcher.Invoke(action);
        }

        public static T InvokeIfRequired<T>(Func<T> func)
        {
            if (MainWindow.dispatcher.CheckAccess()) {
                return func.Invoke(); }
            else return MainWindow.dispatcher.Invoke(func);
        }
    }
}
