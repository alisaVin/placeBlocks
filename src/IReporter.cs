using System;

namespace placing_block.src
{
    public interface IReporter
    {
        void ReportExeption(Exception ex);
        void WriteText(String txt);
        void ReportCurrentFile(string strName);
        void ClearText();
    }
}
