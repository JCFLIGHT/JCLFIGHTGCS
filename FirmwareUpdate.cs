using ArduinoUploader;
using ArduinoUploader.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Windows.Forms;

namespace JCFLIGHTGCS
{
    public partial class FirmwareUpdate : Form, IDisposable
    {
        private Button btnPickFile;
        private TextBox textBoxFilePath;
        private ComboBox cmbxPort;
        private Button burnCodeOne;

        public SerialPort ComPort { get; set; }
        public List<string> PortNames { get; set; }
        public string SelectedPortName { get; set; }

        public FirmwareUpdate()
        {
            InitializeComponent();
            ComPort = new SerialPort();
            LoadAllAvailablePorts();
        }

        void LoadAllAvailablePorts()
        {
            try
            {
                PortNames = SerialPort.GetPortNames()?.ToList();
                cmbxPort.DataSource = PortNames;
                if (PortNames != null && PortNames.Count > 0)
                {
                    cmbxPort.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPickFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog
                {
                    InitialDirectory = @"C:\",
                    Title = "Browse Files",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "hex",
                    Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBoxFilePath.Text = openFileDialog1.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void burnCodeOne_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComPort != null && ComPort.IsOpen)
                {
                    MessageBox.Show("PARECE QUE A PORTA ESTÁ EM USO EM OUTRO PROGRAMA!");
                    return;
                }
                if (string.IsNullOrEmpty(textBoxFilePath.Text) || string.IsNullOrWhiteSpace(SelectedPortName))
                {
                    MessageBox.Show("SELECIONE O ARQUIVO HEXADECIMAL!");
                    return;
                }
                if (string.IsNullOrEmpty(SelectedPortName) || string.IsNullOrWhiteSpace(SelectedPortName))
                {
                    MessageBox.Show("SELECIONE A PORTA SERIAL!");
                    return;
                }

                var Uploader = new ArduinoSketchUploader(new ArduinoSketchUploaderOptions()
                {
                    FileName = @"" + textBoxFilePath.Text,
                    PortName = SelectedPortName,
                    ArduinoModel = ArduinoModel.Mega2560
                });

                try
                {
                    Uploader.UploadSketch();
                }
                catch { }

                MessageBox.Show("FIRMWARE ATUALIZADO!");

                if (ComPort != null && ComPort.IsOpen) ComPort.Close();

            }
            catch (Exception)
            {
            }
        }

        private void cmbxPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox cmb = (ComboBox)sender;
                int selectedIndex = cmb.SelectedIndex;
                var selectedValue = cmb.SelectedValue;
                dynamic selectedPort = cmb.SelectedItem;
                SelectedPortName = (string)selectedPort;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (ComPort != null && ComPort.IsOpen)
                {
                    ComPort.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirmwareUpdate));
            this.btnPickFile = new System.Windows.Forms.Button();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.cmbxPort = new System.Windows.Forms.ComboBox();
            this.burnCodeOne = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPickFile
            // 
            this.btnPickFile.Location = new System.Drawing.Point(395, 83);
            this.btnPickFile.Name = "btnPickFile";
            this.btnPickFile.Size = new System.Drawing.Size(33, 27);
            this.btnPickFile.TabIndex = 14;
            this.btnPickFile.Text = "...";
            this.btnPickFile.UseVisualStyleBackColor = true;
            this.btnPickFile.Click += new System.EventHandler(this.btnPickFile_Click);
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(201, 87);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.ReadOnly = true;
            this.textBoxFilePath.Size = new System.Drawing.Size(188, 20);
            this.textBoxFilePath.TabIndex = 13;
            this.textBoxFilePath.Text = "Local Do Arquivo Hexadecimal";
            // 
            // cmbxPort
            // 
            this.cmbxPort.BackColor = System.Drawing.SystemColors.Window;
            this.cmbxPort.ForeColor = System.Drawing.SystemColors.MenuText;
            this.cmbxPort.FormattingEnabled = true;
            this.cmbxPort.Location = new System.Drawing.Point(245, 35);
            this.cmbxPort.Name = "cmbxPort";
            this.cmbxPort.Size = new System.Drawing.Size(113, 21);
            this.cmbxPort.TabIndex = 12;
            this.cmbxPort.Text = "PORTA COM";
            this.cmbxPort.SelectedIndexChanged += new System.EventHandler(this.cmbxPort_SelectedIndexChanged);
            // 
            // burnCodeOne
            // 
            this.burnCodeOne.Location = new System.Drawing.Point(103, 144);
            this.burnCodeOne.Name = "burnCodeOne";
            this.burnCodeOne.Size = new System.Drawing.Size(399, 31);
            this.burnCodeOne.TabIndex = 11;
            this.burnCodeOne.Text = "ATUALIZAR FIRMWARE";
            this.burnCodeOne.UseVisualStyleBackColor = true;
            this.burnCodeOne.Click += new System.EventHandler(this.burnCodeOne_Click);
            // 
            // FirmwareUpdate
            // 
            this.BackgroundImage = global::JCFLIGHTGCS.Properties.Resources.HexaDec;
            this.ClientSize = new System.Drawing.Size(605, 210);
            this.Controls.Add(this.btnPickFile);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.cmbxPort);
            this.Controls.Add(this.burnCodeOne);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FirmwareUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ATUALIZADOR DE FIRMWARE DA JCFLIGHT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
