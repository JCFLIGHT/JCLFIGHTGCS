namespace Terminal
{
    partial class TerminalControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TXT_terminal = new System.Windows.Forms.RichTextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.port_label = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // TXT_terminal
            // 
            this.TXT_terminal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_terminal.BackColor = System.Drawing.Color.Black;
            this.TXT_terminal.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXT_terminal.ForeColor = System.Drawing.Color.White;
            this.TXT_terminal.Location = new System.Drawing.Point(0, 27);
            this.TXT_terminal.Name = "TXT_terminal";
            this.TXT_terminal.Size = new System.Drawing.Size(752, 442);
            this.TXT_terminal.TabIndex = 1;
            this.TXT_terminal.Text = "";
            this.TXT_terminal.Click += new System.EventHandler(this.TXT_terminal_Click);
            this.TXT_terminal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TXT_terminal_KeyDown);
            this.TXT_terminal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_terminal_KeyPress);
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // port_label
            // 
            this.port_label.AutoSize = true;
            this.port_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.port_label.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.port_label.Location = new System.Drawing.Point(4, 5);
            this.port_label.Name = "port_label";
            this.port_label.Size = new System.Drawing.Size(70, 15);
            this.port_label.TabIndex = 11;
            this.port_label.Text = "Porta COM:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(188, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 21);
            this.button1.TabIndex = 10;
            this.button1.Text = "Conectar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(80, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(102, 21);
            this.comboBox1.TabIndex = 9;
            // 
            // TerminalControl
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.port_label);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.TXT_terminal);
            this.Name = "TerminalControl";
            this.Size = new System.Drawing.Size(752, 469);
            this.Load += new System.EventHandler(this.Terminal_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox TXT_terminal;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label port_label;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}
