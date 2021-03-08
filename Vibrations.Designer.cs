namespace JCFLIGHTGCS
{
    partial class Vibrations
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vibrations));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_clip0 = new System.Windows.Forms.TextBox();
            this.txt_clip1 = new System.Windows.Forms.TextBox();
            this.txt_clip2 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.VibBarZ = new JCFLIGHTGCS.VerticalProgressBar2();
            this.VibBarY = new JCFLIGHTGCS.VerticalProgressBar2();
            this.VibBarX = new JCFLIGHTGCS.VerticalProgressBar2();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Roll";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(112, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Pitch";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(203, 227);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Yaw";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(79, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 39);
            this.label4.TabIndex = 6;
            this.label4.Text = "Vibração";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(283, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 24);
            this.label5.TabIndex = 7;
            this.label5.Text = "Clip";
            // 
            // txt_clip0
            // 
            this.txt_clip0.Location = new System.Drawing.Point(296, 106);
            this.txt_clip0.Name = "txt_clip0";
            this.txt_clip0.Size = new System.Drawing.Size(47, 20);
            this.txt_clip0.TabIndex = 13;
            // 
            // txt_clip1
            // 
            this.txt_clip1.Location = new System.Drawing.Point(296, 141);
            this.txt_clip1.Name = "txt_clip1";
            this.txt_clip1.Size = new System.Drawing.Size(47, 20);
            this.txt_clip1.TabIndex = 14;
            // 
            // txt_clip2
            // 
            this.txt_clip2.Location = new System.Drawing.Point(296, 176);
            this.txt_clip2.Name = "txt_clip2";
            this.txt_clip2.Size = new System.Drawing.Size(47, 20);
            this.txt_clip2.TabIndex = 15;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(270, 109);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "1º";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(270, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "2º";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(270, 179);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "3º";
            // 
            // VibBarZ
            // 
            this.VibBarZ.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.VibBarZ.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.VibBarZ.DisplayScale = 1F;
            this.VibBarZ.DrawLabel = true;
            this.VibBarZ.Label = null;
            this.VibBarZ.Location = new System.Drawing.Point(194, 51);
            this.VibBarZ.Maximum = 90;
            this.VibBarZ.maxline = 60;
            this.VibBarZ.Minimum = 0;
            this.VibBarZ.minline = 30;
            this.VibBarZ.Name = "VibBarZ";
            this.VibBarZ.Size = new System.Drawing.Size(64, 173);
            this.VibBarZ.TabIndex = 2;
            this.VibBarZ.Value = 10;
            this.VibBarZ.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // VibBarY
            // 
            this.VibBarY.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.VibBarY.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.VibBarY.DisplayScale = 1F;
            this.VibBarY.DrawLabel = true;
            this.VibBarY.Label = null;
            this.VibBarY.Location = new System.Drawing.Point(104, 51);
            this.VibBarY.Maximum = 90;
            this.VibBarY.maxline = 60;
            this.VibBarY.Minimum = 0;
            this.VibBarY.minline = 30;
            this.VibBarY.Name = "VibBarY";
            this.VibBarY.Size = new System.Drawing.Size(64, 173);
            this.VibBarY.TabIndex = 1;
            this.VibBarY.Value = 10;
            this.VibBarY.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // VibBarX
            // 
            this.VibBarX.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.VibBarX.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.VibBarX.DisplayScale = 1F;
            this.VibBarX.DrawLabel = true;
            this.VibBarX.Label = null;
            this.VibBarX.Location = new System.Drawing.Point(12, 51);
            this.VibBarX.Maximum = 90;
            this.VibBarX.maxline = 60;
            this.VibBarX.Minimum = 0;
            this.VibBarX.minline = 30;
            this.VibBarX.Name = "VibBarX";
            this.VibBarX.Size = new System.Drawing.Size(64, 173);
            this.VibBarX.TabIndex = 0;
            this.VibBarX.Value = 10;
            this.VibBarX.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // Vibrations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(355, 274);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txt_clip2);
            this.Controls.Add(this.txt_clip1);
            this.Controls.Add(this.txt_clip0);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VibBarZ);
            this.Controls.Add(this.VibBarY);
            this.Controls.Add(this.VibBarX);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Vibrations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vibração";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VerticalProgressBar2 VibBarX;
        private VerticalProgressBar2 VibBarY;
        private VerticalProgressBar2 VibBarZ;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_clip0;
        private System.Windows.Forms.TextBox txt_clip1;
        private System.Windows.Forms.TextBox txt_clip2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}