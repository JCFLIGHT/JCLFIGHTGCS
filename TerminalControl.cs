using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Terminal
{
    public partial class TerminalControl : UserControl
    {
        public string data { get; set; }
        bool threadrun;
        bool conected = false;
        private StringBuilder cmd = new StringBuilder();

        public TerminalControl()
        {
            InitializeComponent();
            serialPort1.DataReceived += serialPort1_DataReceived;
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                return;
            }
            try
            {
                try
                {
                    int dataLength = serialPort1.BytesToRead;
                    byte[] dataRecevied = new byte[dataLength];
                    int nbytes = serialPort1.Read(dataRecevied, 0, dataLength);
                    if (nbytes == 0)
                    {
                        return;
                    }
                    this.BeginInvoke((Action)(() =>
                    {
                        data = System.Text.Encoding.Default.GetString(dataRecevied);
                        TXT_terminal.AppendText(data);
                        data = BitConverter.ToString(dataRecevied);
                    }));
                }
                catch
                {
                }
            }
            catch (Exception)
            {
                if (!threadrun)
                {
                    return;
                }
                TXT_terminal.AppendText("Erro ao ler a porta serial\r\n");
            }
        }

        private void Terminal_Load(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }

                serialPort1.ReadBufferSize = 1024 * 1024;

                var t11 = new Thread(delegate ()
                {
                    threadrun = true;

                    var start = DateTime.Now;

                    while ((DateTime.Now - start).TotalMilliseconds < 2000)
                    {
                        try
                        {
                            if (serialPort1.BytesToRead > 0)
                            {
                                serialPort1_DataReceived(null, null);
                            }
                        }
                        catch
                        {
                            return;
                        }
                    }

                    try
                    {
                        serialPort1.Write("\n\n\n");
                    }
                    catch
                    {
                        return;
                    }

                    while (threadrun)
                    {
                        try
                        {
                            Thread.Sleep(10);
                            if (!serialPort1.IsOpen)
                            {
                                break;
                            }
                            if (serialPort1.BytesToRead > 0)
                            {
                                serialPort1_DataReceived(null, null);
                            }
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                    }
                    catch
                    {
                    }

                    if (threadrun == false)
                    {
                        serialPort1.Close();
                    }
                });
                t11.IsBackground = true;
                t11.Name = "Terminal";
                t11.Start();

                TXT_terminal.AppendText("Aguardando comunicação serial com a JCLFLIGHT...\r\n");
                TXT_terminal.AppendText("\n");
            }
            catch (Exception)
            {
                return;
            }

            TXT_terminal.Focus();
        }

        private void TXT_terminal_Click(object sender, EventArgs e)
        {
            TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

            TXT_terminal.ScrollToCaret();

            TXT_terminal.Refresh();
        }

        private void TXT_terminal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                e.Handled = true;
            }
        }

        private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            Thread.Sleep(400);
        }

        private void TXT_terminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (serialPort1.IsOpen)
                {
                    try
                    {
                        var temp = cmd.ToString();

                        serialPort1.Write(Encoding.ASCII.GetBytes(cmd.ToString()), 0, cmd.Length);
                    }
                    catch
                    {
                    }
                }
                cmd = new StringBuilder();
            }
            else
            {
                cmd.Append(e.KeyChar);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (conected)
            {
                serialPort1.Close();
                serialPort1.DtrEnable = false;
                TXT_terminal.AppendText("\n");
                TXT_terminal.AppendText("Comunicação serial fechada\r\n");
                conected = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!conected)
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.Open();
                    serialPort1.DtrEnable = true;
                    TXT_terminal.AppendText("Comunicação serial aberta\r\n");
                    TXT_terminal.AppendText("\n");
                    conected = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("A conexão falhou!Acho que a Porta Serial está aberta em outro programa,verifique se ela não está aberta na tela principal do GCS.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/JCFLIGHT/JCFLIGHT/wiki/CLI");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/JCFLIGHT/JCFLIGHT/blob/master/Docs/Settings.md");
        }
    }
}