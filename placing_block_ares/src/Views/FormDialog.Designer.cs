namespace Views
{
    partial class FormDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bw = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.coordPath = new System.Windows.Forms.TextBox();
            this.blockPath = new System.Windows.Forms.TextBox();
            this.insertBtn = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.selCoordBtn = new System.Windows.Forms.Button();
            this.selBlockBtn = new System.Windows.Forms.Button();
            this.canselBtn = new System.Windows.Forms.Button();
            this.errorProv = new System.Windows.Forms.ErrorProvider(this.components);
            this.etageLabel = new System.Windows.Forms.Label();
            this.etageInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.blockName = new System.Windows.Forms.TextBox();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.errorProv)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pfad zur Koordinatentabelle (.xlsx)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Pfad zur Blockdatei (.dwg)";
            // 
            // coordPath
            // 
            this.coordPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.coordPath.Location = new System.Drawing.Point(31, 41);
            this.coordPath.Name = "coordPath";
            this.coordPath.Size = new System.Drawing.Size(420, 20);
            this.coordPath.TabIndex = 3;
            // 
            // blockPath
            // 
            this.blockPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blockPath.Location = new System.Drawing.Point(31, 80);
            this.blockPath.Name = "blockPath";
            this.blockPath.Size = new System.Drawing.Size(420, 20);
            this.blockPath.TabIndex = 4;
            // 
            // insertBtn
            // 
            this.insertBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.insertBtn.Location = new System.Drawing.Point(376, 188);
            this.insertBtn.Name = "insertBtn";
            this.insertBtn.Size = new System.Drawing.Size(75, 23);
            this.insertBtn.TabIndex = 5;
            this.insertBtn.Text = "Einfügen";
            this.insertBtn.UseVisualStyleBackColor = true;
            this.insertBtn.Click += new System.EventHandler(this.insertBtn_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.Location = new System.Drawing.Point(31, 255);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(482, 166);
            this.richTextBox.TabIndex = 6;
            this.richTextBox.Text = "";
            // 
            // selCoordBtn
            // 
            this.selCoordBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selCoordBtn.Location = new System.Drawing.Point(476, 39);
            this.selCoordBtn.Name = "selCoordBtn";
            this.selCoordBtn.Size = new System.Drawing.Size(37, 23);
            this.selCoordBtn.TabIndex = 7;
            this.selCoordBtn.Text = "...";
            this.selCoordBtn.UseVisualStyleBackColor = true;
            this.selCoordBtn.Click += new System.EventHandler(this.selCoordBtn_Click);
            // 
            // selBlockBtn
            // 
            this.selBlockBtn.Location = new System.Drawing.Point(476, 78);
            this.selBlockBtn.Name = "selBlockBtn";
            this.selBlockBtn.Size = new System.Drawing.Size(37, 23);
            this.selBlockBtn.TabIndex = 8;
            this.selBlockBtn.Text = "...";
            this.selBlockBtn.UseVisualStyleBackColor = true;
            this.selBlockBtn.Click += new System.EventHandler(this.selBlockBtn_Click);
            // 
            // canselBtn
            // 
            this.canselBtn.Enabled = false;
            this.canselBtn.Location = new System.Drawing.Point(295, 188);
            this.canselBtn.Name = "canselBtn";
            this.canselBtn.Size = new System.Drawing.Size(75, 23);
            this.canselBtn.TabIndex = 9;
            this.canselBtn.Text = "Abbrechen";
            this.canselBtn.UseVisualStyleBackColor = true;
            this.canselBtn.Click += new System.EventHandler(this.canselBtn_Click);
            // 
            // errorProv
            // 
            this.errorProv.ContainerControl = this;
            // 
            // etageLabel
            // 
            this.etageLabel.AutoSize = true;
            this.etageLabel.Location = new System.Drawing.Point(28, 143);
            this.etageLabel.Name = "etageLabel";
            this.etageLabel.Size = new System.Drawing.Size(57, 13);
            this.etageLabel.TabIndex = 10;
            this.etageLabel.Text = "Geschoss ";
            // 
            // etageInput
            // 
            this.etageInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.etageInput.Location = new System.Drawing.Point(31, 159);
            this.etageInput.Name = "etageInput";
            this.etageInput.Size = new System.Drawing.Size(420, 20);
            this.etageInput.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Blockname";
            // 
            // blockName
            // 
            this.blockName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blockName.Location = new System.Drawing.Point(31, 119);
            this.blockName.Name = "blockName";
            this.blockName.Size = new System.Drawing.Size(420, 20);
            this.blockName.TabIndex = 13;
            // 
            // _progressBar
            // 
            this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressBar.Location = new System.Drawing.Point(31, 227);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(482, 15);
            this._progressBar.TabIndex = 14;
            this._progressBar.Visible = false;
            // 
            // FormDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 449);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this.blockName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.etageInput);
            this.Controls.Add(this.etageLabel);
            this.Controls.Add(this.canselBtn);
            this.Controls.Add(this.selBlockBtn);
            this.Controls.Add(this.selCoordBtn);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.insertBtn);
            this.Controls.Add(this.blockPath);
            this.Controls.Add(this.coordPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormDialog";
            this.Padding = new System.Windows.Forms.Padding(25);
            this.Text = "Neue Blöcke in DWG einfügen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDialog_FormClosing);
            this.Load += new System.EventHandler(this.FormDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker bw;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox coordPath;
        private System.Windows.Forms.TextBox blockPath;
        private System.Windows.Forms.Button insertBtn;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Button selCoordBtn;
        private System.Windows.Forms.Button selBlockBtn;
        private System.Windows.Forms.Button canselBtn;
        private System.Windows.Forms.ErrorProvider errorProv;
        private System.Windows.Forms.Label etageLabel;
        private System.Windows.Forms.TextBox etageInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox blockName;
        private System.Windows.Forms.ProgressBar _progressBar;
    }
}