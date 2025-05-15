using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace placing_block.src
{
    public partial class FormDialog : Form
    {
        IReporter _reporter;
        Commands _cmd = new Commands();

        public FormDialog()
        {
            InitializeComponent();
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

            if (Path.IsPathRooted(Properties.Settings.Default.LastCoordinates))
                coordPath.Text = Properties.Settings.Default.LastCoordinates;
            else
                coordPath.Text = string.Empty;

            if (Path.IsPathRooted(Properties.Settings.Default.LastBlock))
                blockPath.Text = Properties.Settings.Default.LastBlock;
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
                        Properties.Settings.Default.LastCoordinates = openFileDialogExcel.FileName;

                    Properties.Settings.Default.Save();
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
                        Properties.Settings.Default.LastBlock = openFileDialogDwg.FileName;

                    Properties.Settings.Default.Save();
                    blockPath.Text = openFileDialogDwg.FileName;
                    var fileStream = openFileDialogDwg.OpenFile();
                }
            }
        }

        private void insertBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(coordPath.Text))
            {
                errorProvCoord.SetError(this.coordPath, "The diractory path is required");
            }
            else if (string.IsNullOrEmpty(blockPath.Text))
            {
                errorProvBlock.SetError(this.blockPath, "The block file path is required");
            }
            else
            {
                insertBtn.Enabled = false;
                canselBtn.Enabled = true;
                errorProvBlock.Clear();
                errorProvCoord.Clear();
                bw.RunWorkerAsync(this);
            }
        }

        private void canselBtn_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
                bw.CancelAsync();
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
                _reporter?.WriteText("Das Prozess wurde abgebrochen.");
                insertBtn.Enabled = true;
                canselBtn.Enabled = false;
                return;
            }
            _reporter?.ClearText();
            _reporter?.WriteText("Das Prozess wurde erfolgreich abgeschlossen.");
            Thread.Sleep(100);
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string coordRoot = coordPath.Text;
            string blockRoot = blockPath.Text;
            _cmd.PlaceBlocks(blockRoot, coordRoot, sender, e);
        }


        private void FormDialog_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.WindowLocation = this.Location;
            Properties.Settings.Default.LastCoordinates = this.coordPath.Text;
            Properties.Settings.Default.LastBlock = blockPath.Text;
            Properties.Settings.Default.Save();
        }

        private void FormDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.WindowLocation = this.Location;
        }


    }
}
