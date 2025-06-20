using Commands;
using placing_block.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ViewModels;


namespace Views
{
    public partial class FormDialog : Form
    {
        Reporter _reporter;
        ACADCommands _cmd;
        public FormDialog()
        {
            InitializeComponent();
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            _cmd = new ACADCommands();
            _reporter = new Reporter(richTextBox, this);

            if (Path.IsPathRooted(Settings.Default.LastCoordinates))
                coordPath.Text = Settings.Default.LastCoordinates;
            else
                coordPath.Text = string.Empty;

            if (Path.IsPathRooted(Settings.Default.LastBlock))
                blockPath.Text = Settings.Default.LastBlock;
            else
                blockPath.Text = string.Empty;
        }

        private void selCoordBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialogExcel = new OpenFileDialog())
            {
                openFileDialogExcel.InitialDirectory = "c:\\";
                openFileDialogExcel.Filter = "All files (*.*)|*.*|Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv";
                openFileDialogExcel.FilterIndex = 2;
                openFileDialogExcel.RestoreDirectory = true;

                if (openFileDialogExcel.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(openFileDialogExcel.FileName))
                        Settings.Default.LastCoordinates = openFileDialogExcel.FileName;

                    Settings.Default.Save();
                    coordPath.Text = openFileDialogExcel.FileName;
                    var fileStream = openFileDialogExcel.OpenFile();
                }
            }
        }

        private void selBlockBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialogDwg = new OpenFileDialog())
            {
                openFileDialogDwg.InitialDirectory = "c:\\";
                openFileDialogDwg.Filter = "All files (*.*)|*.*|DWG files (*.dwg)|*.dwg";
                openFileDialogDwg.FilterIndex = 2;
                openFileDialogDwg.RestoreDirectory = true;

                if (openFileDialogDwg.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(openFileDialogDwg.FileName))
                        Settings.Default.LastBlock = openFileDialogDwg.FileName;

                    Settings.Default.Save();
                    blockPath.Text = openFileDialogDwg.FileName;
                    var fileStream = openFileDialogDwg.OpenFile();
                }
            }
        }

        private void insertBtn_Click(object sender, EventArgs e)
        {
            _reporter.ClearText();
            errorProv.Clear();

            if (string.IsNullOrEmpty(coordPath.Text))
                errorProv.SetError(this.coordPath, "Geben Sie den Pfad zur Excel-Datei");

            if (string.IsNullOrEmpty(blockPath.Text))
                errorProv.SetError(this.blockPath, "Geben Sie den Pfad zur DWG-Datei");

            if (string.IsNullOrEmpty(blockName.Text))
                errorProv.SetError(this.blockName, "Geben Sie den Blocknamen");

            if (string.IsNullOrEmpty(etageInput.Text))
                errorProv.SetError(this.etageInput, "Geben Sie die Bezeichnung des Geschosses z. B. EG");

            insertBtn.Enabled = false;
            canselBtn.Enabled = true;
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;
            bw.RunWorkerAsync(this);
        }

        private void canselBtn_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
                bw.CancelAsync();
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string coordRoot = coordPath.Text;
            string blockRoot = blockPath.Text;
            string blName = blockName.Text;
            string etage = etageInput.Text;
            _cmd.PlaceBlocks(coordRoot, blockRoot, blName, etage, sender, e);
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _reporter?.ClearText();
                _reporter?.ReportExeption(e.Error);
                return;
            }

            if (e.Cancelled == true)
            {
                _reporter?.ClearText();
                _reporter?.WriteText("Der Prozess wurde abgebrochen.");
                insertBtn.Enabled = true;
                canselBtn.Enabled = false;
                progressBar.Visible = false;
                return;
            }
            _reporter?.ClearText();
            _reporter?.WriteText("Der Prozess wurde erfolgreich abgeschlossen.");
            Thread.Sleep(100);
            insertBtn.Enabled = true;
            canselBtn.Enabled = false;
            progressBar.Visible = false;
        }

        private void FormDialog_Load(object sender, EventArgs e)
        {
            Settings.Default.WindowWidth = this.Width;
            Settings.Default.WindowHeight = this.Height;
            Settings.Default.WindowLocation = this.Location;
            Settings.Default.LastCoordinates = this.coordPath.Text;
            Settings.Default.LastBlock = blockPath.Text;
            Settings.Default.Save();
        }

        private void FormDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowWidth = this.Width;
            Settings.Default.WindowHeight = this.Height;
            Settings.Default.WindowLocation = this.Location;
        }
    }
}
