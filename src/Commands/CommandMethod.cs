using Autodesk.AutoCAD.Runtime;
using Views;

namespace Commands
{
    public class CommandMethods
    {
        [CommandMethod("PLACEBLOCK", CommandFlags.Modal)]
        public void Demo()
        {
            FormDialog formDlg = new FormDialog { TopMost = false };
            //formDlg.Show();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(formDlg); //open the modeless dialog
        }

        #region Register commands
        [CommandMethod("RegApp")]
        public void RegisterAppOnDemand()
        {
            DemandLoading.RegisterForDemandLoading();
        }

        [CommandMethod("UnregApp")]
        public void UnregisterApp()
        {
            DemandLoading.UnregisterForDemandLoading();
        }
        #endregion
    }
}
