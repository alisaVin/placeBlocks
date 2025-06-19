using System;

namespace ViewModels
{
    public interface IReporter
    {
        void ReportExeption(Exception ex);
        void WriteText(String txt);
        void ReportCurrentFile(string strName);
        void ClearText();
    }
}
