namespace placing_block.src
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
            this.errorProvCoord = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProvBlock = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvCoord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvBlock)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pfad zur Koordinatenliste (.xlsx)";
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
            this.coordPath.Size = new System.Drawing.Size(451, 20);
            this.coordPath.TabIndex = 3;
            // 
            // blockPath
            // 
            this.blockPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blockPath.Location = new System.Drawing.Point(31, 80);
            this.blockPath.Name = "blockPath";
            this.blockPath.Size = new System.Drawing.Size(451, 20);
            this.blockPath.TabIndex = 4;
            // 
            // insertBtn
            // 
            this.insertBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.insertBtn.Location = new System.Drawing.Point(407, 115);
            this.insertBtn.Name = "insertBtn";
            this.insertBtn.Size = new System.Drawing.Size(75, 23);
            this.insertBtn.TabIndex = 5;
            this.insertBtn.Text = "Einfügen";
            this.insertBtn.UseVisualStyleBackColor = true;
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.Location = new System.Drawing.Point(28, 177);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(475, 206);
            this.richTextBox.TabIndex = 6;
            this.richTextBox.Text = "";
            // 
            // selCoordBtn
            // 
            this.selCoordBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selCoordBtn.Location = new System.Drawing.Point(488, 39);
            this.selCoordBtn.Name = "selCoordBtn";
            this.selCoordBtn.Size = new System.Drawing.Size(25, 23);
            this.selCoordBtn.TabIndex = 7;
            this.selCoordBtn.Text = "...";
            this.selCoordBtn.UseVisualStyleBackColor = true;
            this.selCoordBtn.Click += new System.EventHandler(this.selCoordBtn_Click);
            // 
            // selBlockBtn
            // 
            this.selBlockBtn.Location = new System.Drawing.Point(488, 78);
            this.selBlockBtn.Name = "selBlockBtn";
            this.selBlockBtn.Size = new System.Drawing.Size(25, 23);
            this.selBlockBtn.TabIndex = 8;
            this.selBlockBtn.Text = "...";
            this.selBlockBtn.UseVisualStyleBackColor = true;
            this.selBlockBtn.Click += new System.EventHandler(this.selBlockBtn_Click);
            // 
            // canselBtn
            // 
            this.canselBtn.Enabled = false;
            this.canselBtn.Location = new System.Drawing.Point(326, 115);
            this.canselBtn.Name = "canselBtn";
            this.canselBtn.Size = new System.Drawing.Size(75, 23);
            this.canselBtn.TabIndex = 9;
            this.canselBtn.Text = "Abbrechen";
            this.canselBtn.UseVisualStyleBackColor = true;
            this.canselBtn.Click += new System.EventHandler(this.canselBtn_Click);
            // 
            // errorProvCoord
            // 
            this.errorProvCoord.ContainerControl = this;
            // 
            // errorProvBlock
            // 
            this.errorProvBlock.ContainerControl = this;
            // 
            // FormDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 411);
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
            this.Text = "FormDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDialog_FormClosing);
            this.Load += new System.EventHandler(this.FormDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvCoord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvBlock)).EndInit();
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
        private System.Windows.Forms.ErrorProvider errorProvCoord;
        private System.Windows.Forms.ErrorProvider errorProvBlock;
    }
}