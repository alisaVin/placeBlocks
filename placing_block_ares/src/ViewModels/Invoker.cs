using System;
using System.Windows.Forms;

namespace placing_block.src
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
