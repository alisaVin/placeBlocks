using System;
using System.Windows.Forms;

namespace placing_block.src
{
    public class Reporter : IReporter
    {
        RichTextBox _textBox;
        Control _ctrl;

        public Reporter(RichTextBox richTextBox, Control ctrl)
        {
            _textBox = richTextBox;
            _ctrl = ctrl;
        }

        public void ReportExeption(Exception ex)
        {
            Invoker.Invoke(() =>
            {
                _textBox.AppendText($"\nError message: {ex.Message}");
                _textBox.AppendText($"\nType: {ex.GetType().FullName}\n{ex.StackTrace}");
                _textBox.SelectionStart = _textBox.Text.Length;
                _textBox.ScrollToCaret();
            }, _ctrl);
        }

        public void ReportCurrentFile(string strName)
        {
            Invoker.Invoke(() =>
            {
                _textBox.AppendText($"\nFile Name: {strName}\n");
                _textBox.SelectionStart = _textBox.Text.Length;
                _textBox.ScrollToCaret();
            }, _ctrl);
        }

        public void WriteText(string txt)
        {
            Invoker.Invoke(() =>
            {
                _textBox.AppendText($"{txt}\n");
                _textBox.Refresh();
            }, _ctrl);
        }

        public void ClearText()
        {
            Invoker.Invoke(() => _textBox.Clear(), _ctrl);
        }
    }
}
