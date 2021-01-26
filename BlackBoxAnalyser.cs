using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ZedGraph;
using System.Xml;

namespace JCFLIGHTGCS
{
    public partial class BlackBoxAnalyser : Form
    {
        int ColumnCount = 0;
        DataTable DataTableCSV = new DataTable();

        PointPairList List1 = new PointPairList();
        PointPairList List2 = new PointPairList();
        PointPairList List3 = new PointPairList();
        PointPairList List4 = new PointPairList();
        PointPairList List5 = new PointPairList();

        int Graphs = 0;

        public BlackBoxAnalyser()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDialogInstance = new OpenFileDialog();
            OpenFileDialogInstance.InitialDirectory = Directory.GetCurrentDirectory() + "\\Caixa Preta";
            OpenFileDialogInstance.Filter = "Log Files|*.log";
            OpenFileDialogInstance.FilterIndex = 2;
            OpenFileDialogInstance.RestoreDirectory = true;

            OpenFileDialogInstance.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "logs";

            CreateChart(zg1);

            if (OpenFileDialogInstance.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream stream = File.Open(OpenFileDialogInstance.FileName, FileMode.Open);
                    PopulateDataTableFromUploadedFile(stream);
                    stream.Close();

                    dataGridView1.DataSource = DataTableCSV;

                }
                catch (Exception)
                {
                    MessageBox.Show("Falha ao ler o arquivo");
                }

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                int a = 1;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.HeaderCell.Value = a.ToString();
                    a++;
                }
            }
            else
            {
                this.Close();
                return;
            }
        }

        private void PopulateDataTableFromUploadedFile(System.IO.Stream strm)
        {
            System.IO.StreamReader srdr = new System.IO.StreamReader(strm);
            String strLine = String.Empty;
            Int32 iLineCount = 0;
            do
            {
                strLine = srdr.ReadLine();
                if (strLine == null)
                {
                    break;
                }
                if (0 == iLineCount++)
                {
                    DataTableCSV = new DataTable("CSVTable");
                }
                this.AddDataRowToTable(strLine, DataTableCSV);
            } while (true);
        }

        private DataRow AddDataRowToTable(String strCSVLine, DataTable dataTable)
        {
            String[] strVals = strCSVLine.Split(new char[] { ',' });
            Int32 iTotalNumberOfValues = strVals.Length;
            if (iTotalNumberOfValues > ColumnCount)
            {
                Int32 iDiff = iTotalNumberOfValues - ColumnCount;
                for (Int32 i = 0; i < iDiff; i++)
                {
                    String strColumnName = String.Format("{0}", (ColumnCount + i));
                    dataTable.Columns.Add(strColumnName, Type.GetType("System.String"));
                }
                ColumnCount = iTotalNumberOfValues;
            }
            int idx = 0;
            DataRow drow = dataTable.NewRow();
            foreach (String strVal in strVals)
            {
                String strColumnName = String.Format("{0}", idx++);
                drow[strColumnName] = strVal.Trim();
            }
            dataTable.Rows.Add(drow);
            return drow;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int a = 0;
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderText = a.ToString();
                    a++;
                }
            }
            catch { }

            try
            {
                string option = dataGridView1[0, e.RowIndex].EditedFormattedValue.ToString();
                using (XmlReader reader = XmlReader.Create("BlackBoxFields.xml"))
                {
                    reader.Read();
                    reader.ReadStartElement("LOGFORMAT");
                    reader.ReadToFollowing("JCFLIGHT");
                    reader.ReadToFollowing(option);

                    dataGridView1.Columns[0].HeaderText = "";

                    XmlReader inner = reader.ReadSubtree();

                    inner.MoveToElement();

                    int a = 1;

                    while (inner.Read())
                    {
                        inner.MoveToElement();
                        if (inner.IsStartElement())
                        {
                            if (inner.Name.StartsWith("F"))
                            {
                                dataGridView1.Columns[a].HeaderText = inner.ReadString();
                                a++;
                            }

                        }
                    }

                    for (; a < dataGridView1.Columns.Count; a++)
                    {
                        dataGridView1.Columns[a].HeaderText = "";
                    }

                }
            }
            catch
            {
            }
        }

        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Horário";
            myPane.YAxis.Title.Text = "Valor";
            myPane.XAxis.Type = AxisType.Date;
            myPane.XAxis.Scale.Format = "HH:mm:ss.fff";
            LineItem myCurve;
            myCurve = myPane.AddCurve("NULL", List1, Color.Red, SymbolType.None);
            myCurve = myPane.AddCurve("NULL", List2, Color.LightGreen, SymbolType.None);
            myCurve = myPane.AddCurve("NULL", List3, Color.LightBlue, SymbolType.None);
            myCurve = myPane.AddCurve("NULL", List4, Color.Pink, SymbolType.None);
            myCurve = myPane.AddCurve("NULL", List5, Color.Yellow, SymbolType.None);
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.Scale.FontSpec.FontColor = Color.White;
            myPane.YAxis.Title.FontSpec.FontColor = Color.White;
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorGrid.IsZeroLine = true;
            myPane.YAxis.Scale.Align = AlignP.Inside;
            myPane.Chart.Fill = new Fill(Color.DimGray, Color.DarkGray, 45.0f);
            myPane.Fill = new Fill(Color.DimGray, Color.DimGray, 45.0f);
            myPane.Legend.Fill = new Fill(Color.DimGray, Color.DimGray, 0);
            myPane.Legend.FontSpec.FontColor = Color.White;
            foreach (ZedGraph.LineItem li in myPane.CurveList)
            {
                li.Line.Width = 2;
            }
            myPane.XAxis.MajorTic.Color = Color.White;
            myPane.XAxis.MinorTic.Color = Color.White;
            myPane.YAxis.MajorTic.Color = Color.White;
            myPane.YAxis.MinorTic.Color = Color.White;
            myPane.XAxis.MajorGrid.Color = Color.White;
            myPane.YAxis.MajorGrid.Color = Color.White;
            myPane.YAxis.Scale.FontSpec.FontColor = Color.White;
            myPane.YAxis.Title.FontSpec.FontColor = Color.White;
            myPane.XAxis.Scale.FontSpec.FontColor = Color.White;
            myPane.XAxis.Title.FontSpec.FontColor = Color.White;
            myPane.Legend.Fill = new ZedGraph.Fill(Color.FromArgb(0x85, 0x84, 0x83));
            myPane.Legend.Position = LegendPos.TopCenter;
            try
            {
                zg1.AxisChange();
            }
            catch { }
        }

        private void Graphit_Click(object sender, EventArgs e)
        {

            double prvTimestamp = 0;


            if (dataGridView1.RowCount == 0 || dataGridView1.ColumnCount == 0)
            {
                MessageBox.Show("Arquivo invalido!");
                return;
            }

            GraphPane myPane = zg1.GraphPane;

            int col = dataGridView1.CurrentCell.ColumnIndex;
            int row = dataGridView1.CurrentCell.RowIndex;

            string type = dataGridView1[0, row].Value.ToString();
            double a = 0;

            if (col == 0)
            {
                return;
            }

            int error = 0;

            foreach (DataGridViewRow datarow in dataGridView1.Rows)
            {
                DateTime dt;
                try
                {
                    dt = Convert.ToDateTime(datarow.Cells[1].Value.ToString());
                }
                catch
                {
                    dt = Convert.ToDateTime("00:00:00.000");
                }

                double timestamp = dt.ToOADate();

                if (datarow.Cells[0].Value.ToString() == type)
                {
                    prvTimestamp = timestamp;
                    try
                    {
                        double value = double.Parse(datarow.Cells[col].Value.ToString(), new System.Globalization.CultureInfo("en-US"));
                        if (Graphs == 0)
                        {
                            zg1.GraphPane.CurveList[0].Label.Text = dataGridView1.Columns[col].HeaderText;
                            List1.Add(timestamp, value);
                        }
                        else if (Graphs == 1)
                        {
                            zg1.GraphPane.CurveList[1].Label.Text = dataGridView1.Columns[col].HeaderText;
                            List2.Add(timestamp, value);
                        }
                        else if (Graphs == 2)
                        {
                            zg1.GraphPane.CurveList[2].Label.Text = dataGridView1.Columns[col].HeaderText;
                            List3.Add(timestamp, value);
                        }
                        else if (Graphs == 3)
                        {
                            zg1.GraphPane.CurveList[3].Label.Text = dataGridView1.Columns[col].HeaderText;
                            List4.Add(timestamp, value);
                        }
                        else if (Graphs == 4)
                        {
                            zg1.GraphPane.CurveList[4].Label.Text = dataGridView1.Columns[col].HeaderText;
                            List5.Add(timestamp, value);
                        }
                        else
                        {
                            MessageBox.Show("Número maximo de componentes ao plotter atingido!");
                            break;
                        }
                    }
                    catch
                    {
                        error++;
                        if (error >= 500)
                        {
                            MessageBox.Show("Caixa-Preta Falhou!"); break;
                        }
                    }
                }
                a++;
            }

            try
            {
                zg1.AxisChange();
            }
            catch { }
            zg1.ZoomOutAll(zg1.GraphPane);
            zg1.Invalidate();
            Graphs++;
        }

        private void BUT_cleargraph_Click(object sender, EventArgs e)
        {
            Graphs = 0;
            foreach (LineItem line in zg1.GraphPane.CurveList)
            {
                line.Clear();
                line.Label.Text = "NULL";
            }
            zg1.Invalidate();
        }

        private void BUT_loadlog_Click(object sender, EventArgs e)
        {
            ColumnCount = 0;
            zg1.GraphPane.CurveList.Clear();
            Form1_Load(sender, e);
        }
    }
}