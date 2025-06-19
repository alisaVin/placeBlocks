using System;
using System.Windows.Forms;

namespace ViewModels
{
    public static class Invoker
    {
        public static void Invoke(Action act, Control ctrl)
        {
            if (ctrl == null)
                return;
            ctrl.Invoke(act);
        }
    }
}
