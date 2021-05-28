using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Terminal
{
    public partial class TerminalControl : UserControl
    {
        public static bool threadrun;
        private readonly List<string> cmdHistory = new List<string>();
        private readonly object thisLock = new object();
        private int history;
        private int inputStartPos;
        bool conected = false;

        public TerminalControl()
        {
            InitializeComponent();
            threadrun = false;
            serialPort1.DataReceived += serialPort1_DataReceived;
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                return;
            }

            serialPort1.ReadTimeout = 300;

            try
            {
                lock (thisLock)
                {
                    var buffer = new byte[256];
                    var a = 0;

                    while (serialPort1.IsOpen && serialPort1.BytesToRead > 0)
                    {
                        var indata = (byte)serialPort1.ReadByte();

                        buffer[a] = indata;

                        if (buffer[a] >= 0x20 && buffer[a] < 0x7f || buffer[a] == '\n' || buffer[a] == 0x1b)
                        {
                            a++;
                        }

                        if (indata == '\n')
                        {
                            break;
                        }

                        if (a == (buffer.Length - 1))
                        {
                            break;
                        }
                    }

                    addText(Encoding.ASCII.GetString(buffer, 0, a + 1));
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

        private void addText(string data)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                if (this.Disposing)
                {
                    return;
                }

                if (inputStartPos > TXT_terminal.Text.Length)
                {
                    inputStartPos = TXT_terminal.Text.Length - 1;
                }

                string currenttypedtext = TXT_terminal.Text.Substring(inputStartPos, TXT_terminal.Text.Length - inputStartPos);

                TXT_terminal.Text = TXT_terminal.Text.Remove(inputStartPos, TXT_terminal.Text.Length - inputStartPos);

                TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

                data = data.TrimEnd('\r');
                data = data.Replace("\0", "");
                data = data.Replace((char)0x1b + "[K", "");
                TXT_terminal.AppendText(data);

                if (data.Contains("\b"))
                {
                    TXT_terminal.Text = TXT_terminal.Text.Remove(TXT_terminal.Text.IndexOf('\b'));
                    TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
                }

                if (data.Contains((char)0x1b + "[K"))
                {
                    TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
                }
                inputStartPos = TXT_terminal.SelectionStart;

                TXT_terminal.AppendText(currenttypedtext);
            });
        }

        private void TXT_terminal_KeyDown(object sender, KeyEventArgs e)
        {
            TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
            lock (thisLock)
            {
                switch (e.KeyData)
                {
                    case Keys.Up:
                        if (history > 0)
                        {
                            TXT_terminal.Select(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            TXT_terminal.SelectedText = "";
                            TXT_terminal.AppendText(cmdHistory[--history]);
                        }
                        e.Handled = true;
                        break;
                    case Keys.Down:
                        if (history < cmdHistory.Count - 1)
                        {
                            TXT_terminal.Select(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            TXT_terminal.SelectedText = "";
                            TXT_terminal.AppendText(cmdHistory[++history]);
                        }
                        e.Handled = true;
                        break;
                    case Keys.Left:
                    case Keys.Back:
                        if (TXT_terminal.SelectionStart <= inputStartPos)
                            e.Handled = true;
                        break;
                }
            }
        }

        private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;

            try
            {
                if (serialPort1 != null && serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
            }
            catch
            {
            }
        }

        private void TXT_terminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (serialPort1.IsOpen)
                {
                    try
                    {
                        var cmd = "";
                        lock (thisLock)
                        {
                            cmd = TXT_terminal.Text.Substring(inputStartPos, TXT_terminal.Text.Length - inputStartPos - 1);

                            TXT_terminal.Select(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            TXT_terminal.SelectedText = "";
                            if (cmd.Length > 0 && (cmdHistory.Count == 0 || cmdHistory.Last() != cmd))
                            {
                                cmdHistory.Add(cmd);
                                history = cmdHistory.Count;
                            }
                        }

                        serialPort1.Write(Encoding.ASCII.GetBytes(cmd), 0, cmd.Length);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void waitandsleep(int time)
        {
            var start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < time)
            {
                try
                {
                    if (!serialPort1.IsOpen || serialPort1.BytesToRead > 0)
                    {
                        return;
                    }
                }
                catch
                {
                    threadrun = false;
                    return;
                }
            }
        }

        private void readandsleep(int time)
        {
            var start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < time)
            {
                try
                {
                    if (!serialPort1.IsOpen)
                        return;
                    if (serialPort1.BytesToRead > 0)
                    {
                        serialPort1_DataReceived(null, null);
                    }
                }
                catch
                {
                    threadrun = false;
                    return;
                }
            }
        }

        private void Terminal_Load(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                return;
            }

            try
            {
                if (serialPort1 != null && serialPort1.BaseStream != null && serialPort1.IsOpen) serialPort1.BaseStream.Close();

                if (serialPort1.IsOpen)
                {
                    threadrun = false;

                    serialPort1.Close();

                    Thread.Sleep(400);
                }

                serialPort1.ReadBufferSize = 1024 * 1024 * 4;

                serialPort1.PortName = comboBox1.Text;

                serialPort1.BaudRate = 115200;

                serialPort1.Open();

                serialPort1.DtrEnable = true;

                try
                {
                    serialPort1.DiscardInBuffer();
                }
                catch
                {
                }

                startreadthread();
            }
            catch (Exception)
            {
                TXT_terminal.AppendText("Não foi possivel abrir a porta\r\n");
                return;
            }
        }

        private void startreadthread()
        {

            var t11 = new Thread(delegate ()
            {
                threadrun = true;

                try
                {
                    serialPort1.Write("\r");
                }
                catch
                {
                }

                waitandsleep(10000);

                readandsleep(100);

                try
                {
                    if (serialPort1.IsOpen)
                    {
                        serialPort1.Write("\n\n\n");
                    }

                    if (serialPort1.IsOpen)
                    {
                        readandsleep(1000);
                    }

                    if (serialPort1.IsOpen)
                    {
                        serialPort1.Write("\r\r\r?\r");
                    }
                }
                catch (Exception)
                {
                    threadrun = false;
                    return;
                }

                while (threadrun)
                {
                    try
                    {
                        Thread.Sleep(10);

                        if (!threadrun)
                        {
                            break;
                        }
                        if (this.Disposing)
                        {
                            break;
                        }
                        if (!serialPort1.IsOpen)
                        {
                            break;
                        }
                        if (serialPort1.BytesToRead > 0)
                        {
                            serialPort1_DataReceived(null, null);
                        }

                    }
                    catch (Exception)
                    {
                    }
                }

                threadrun = false;
                try
                {
                    serialPort1.DtrEnable = false;
                }
                catch
                {
                }
                try
                {
                    serialPort1.Close();
                }
                catch
                {
                }
            });
            t11.IsBackground = true;
            t11.Name = "Terminal";
            t11.Start();

            if (IsDisposed || Disposing)
            {
                return;
            }

            inputStartPos = TXT_terminal.SelectionStart;


            TXT_terminal.Focus();
        }

        private void TXT_terminal_Click(object sender, EventArgs e)
        {
            //TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

            //TXT_terminal.ScrollToCaret();

            //TXT_terminal.Refresh();
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
                button1.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!conected)
                {
                    serialPort1.ReadBufferSize = 1024 * 1024 * 4;
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.Open();
                    serialPort1.DtrEnable = true;
                    TXT_terminal.AppendText("Comunicação serial aberta\r\n");
                    TXT_terminal.AppendText("\n");
                    conected = true;
                    button1.Enabled = true;
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