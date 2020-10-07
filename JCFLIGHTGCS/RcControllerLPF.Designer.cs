namespace JCFLIGHTGCS
{
    partial class RcControllerLPF
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RcControllerLPF));
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel5.Location = new System.Drawing.Point(12, 85);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(209, 38);
            this.metroLabel5.TabIndex = 11;
            this.metroLabel5.Text = "Azul:Valor Original sem Filtro.\r\nAmarelo:Valor Filtrado.";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.metroLabel2.Location = new System.Drawing.Point(12, 9);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(786, 76);
            this.metroLabel2.TabIndex = 10;
            this.metroLabel2.Text = resources.GetString("metroLabel2.Text");
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.Location = new System.Drawing.Point(12, 149);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(146, 19);
            this.metroLabel1.TabIndex = 9;
            this.metroLabel1.Text = "Exemplo com 40 Hz:";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel3.Location = new System.Drawing.Point(12, 440);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(146, 19);
            this.metroLabel3.TabIndex = 13;
            this.metroLabel3.Text = "Exemplo com 50 Hz:";
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel4.Location = new System.Drawing.Point(12, 735);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(146, 19);
            this.metroLabel4.TabIndex = 15;
            this.metroLabel4.Text = "Exemplo com 80 Hz:";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::JCFLIGHTGCS.Properties.Resources.RC_LPF_80HZ;
            this.pictureBox3.Location = new System.Drawing.Point(12, 757);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(1242, 239);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 14;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::JCFLIGHTGCS.Properties.Resources.RC_LPF_50HZ;
            this.pictureBox2.Location = new System.Drawing.Point(12, 462);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1242, 239);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::JCFLIGHTGCS.Properties.Resources.RC_LPF_40HZ;
            this.pictureBox1.Location = new System.Drawing.Point(12, 171);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1242, 239);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // RcControllerLPF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1289, 561);
            this.Controls.Add(this.metroLabel4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.metroLabel5);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RcControllerLPF";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attitude RC LPF";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private System.Windows.Forms.PictureBox pictureBox3;
    }
}