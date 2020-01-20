using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace SoftGenConverter
{
    public partial class Form1 : Form
    {
        private string name;
        private TextBox textImport = new TextBox();
        private string currentCellValue = "";
        private bool editAval = false;
        private bool editUkrG = false;
        private Image editBtn = Properties.Resources.form1Edit; 
        private Image saveBtn = Properties.Resources.form1EndEdit;
        
        private Bank aval = new Bank();
        private Bank ukrGaz = new Bank();

        private long numberDocAval;
        private string P = "·";

        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterData.xml");
        private string pathConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterConfig.xml");
        private Version localVersion = new Version(Application.ProductVersion);
        private string path = "";
        private string strData = Properties.Resources.PayConverterData;
        private string strConfig = Properties.Resources.PayConverterConfig;

        
        

        public Form1()
        {
            

            InitializeComponent();
            // Bank[] banks = Xml.ReadXml(pathConfig);
            //MessageBox.Show(banks[0].ToString());
            initData();


        }

        //Двойная буферизация для таблиц
        private void SetDoubleBuffered(Control c, bool value)
        {
            PropertyInfo pi = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic);
            if (pi != null)
            {
                pi.SetValue(c, value, null);
            }
        }
        public void initData()
        {
            Xml.isExistsFile(path2, strData);
            Xml.isExistsFile(pathConfig, strConfig);

            Xml.loadXml(dataGridView3, path2);
            try
            {

            Bank[] banks = Xml.ReadXml(pathConfig);
            aval = banks[0];
            ukrGaz = banks[1];
            }
            catch
            {

            }

            
            setFieldsP();
            setFieldsP2();
            //comboEdr.Items.Add(aval.name);
            // comboEdr.Text = aval.name;

            isEditAval(editAval);
            isEditUkrG(editUkrG);
            MyDataGrid.StyleDataGridView(dataGridView1, false);
            MyDataGrid.StyleDataGridView(dataGridView2, false);

            comboEdr2.SelectedIndex = 0;
            comboEdr.SelectedIndex = 0;
            SetDoubleBuffered(dataGridView1, true);
            SetDoubleBuffered(dataGridView2, true);
            SetDoubleBuffered(dataGridView3, true);
            Properties.Settings.Default.count++;
            Properties.Settings.Default.Save();
            backUpData();

        }

        public void backUpData()
        {
            if (Properties.Settings.Default.count % 10 == 0)
            {
                string directory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}" + "\\PayConverterBackup";
                bool exists = System.IO.Directory.Exists(directory);
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                string date = DateTime.Today.ToString("ddMMyyyy");
                string bakFilePath = directory + "\\" + date + "PayConverterData.xml" + ".bak";
                Xml.saveXml(dataGridView3, bakFilePath);

            }
        }

        public void setFieldsP()
        {


            mfo.Text = aval.mfo;
            rahunok.Text = aval.rahunok;
            cliBankCode.Text = aval.clientBankCode;

            dateTimePicker1.Value = DateTime.Now;


            tableLayoutPanel7.RowStyles[1].Height = 100;
            tableLayoutPanel7.RowStyles[0].Height = 0;
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;

        }

        public void setFieldsP2()
        {
            textBox2.Text = ukrGaz.edrpou;
            textBox4.Text = ukrGaz.rahunok;
            textIban.Text = ukrGaz.iban;

            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            openCsv();
            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
        }

        private bool isNull = false;
        public void openCsv()
        {
            openFileDialog1.FileName = "file"; //
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView2.Rows.Clear();
                    numberDocAval = 1;
                }

                path = name = openFileDialog1.FileName;
                loadFileRoot();

                if (isNull)
                {
                    Xml.saveXml(dataGridView3, path2);
                }
            }
        }

        public void loadFileRoot()
        {
            List<Bank> CSV_Struct = new List<Bank>();
            CSV_Struct = Bank.ReadFile(path);
            DateTime dt1 = DateTime.Today;
            for (int i = 0; i <= CSV_Struct.Count - 1; i++)
            {
                int n;
                if (CSV_Struct[i].id == 0)
                {
                    n = dataGridView1.Rows.Add();

                    dataGridView1.Rows[n].Cells[0].Value = CSV_Struct[i].summa;
                    dataGridView1.Rows[n].Cells[1].Value = "UAH";
                    dataGridView1.Rows[n].Cells[2].Value = addDateToStr(findZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                       (CSV_Struct[i].dateP == dt1 ? dateTimePicker1.Value.ToString("dd.MM.yyyy") : CSV_Struct[i].dateP.ToString("dd.MM.yyyy")));

                    if (dataGridView1.Rows[n].Cells[2].Value.Equals("null"))
                    {
                        dataGridView1.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                        int m = dataGridView3.Rows.Add();
                        dataGridView3.Rows[m].Cells[0].Value = CSV_Struct[i].name;
                        dataGridView3.Rows[m].Cells[1].Value = CSV_Struct[i].edrpou;
                        dataGridView3.Rows[m].Cells[2].Value = CSV_Struct[i].rahunok;
                        dataGridView3.Rows[m].Cells[3].Value = dataGridView1.Rows[n].Cells[2].Value;
                        isNull = true;
                    }

                    dataGridView1.Rows[n].Cells[3].Value = ukrGaz.rahunok;
                    dataGridView1.Rows[n].Cells[4].Value = ukrGaz.edrpou;

                    dataGridView1.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                    dataGridView1.Rows[n].Cells[6].Value = CSV_Struct[i].rahunok;
                    dataGridView1.Rows[n].Cells[7].Value = CSV_Struct[i].edrpou;
                    dataGridView1.Rows[n].Cells[8].Value = findNameZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok).Equals("null") ? CSV_Struct[i].name : findNameZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok);
                    dataGridView1.Rows[n].Cells[9].Value = ukrGaz.iban;
                }
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                if (CSV_Struct[i].id == 1)
                {
                    try
                    {
                        dateTimePicker1.Value =
                            DateTime.Parse(CSV_Struct[i].dateP.ToString("dd.MM.yyyy"), MyCultureInfo);

                        n = dataGridView2.Rows.Add();
                        dataGridView2.Rows[n].Cells[0].Value = "0";
                        dataGridView2.Rows[n].Cells[1].Value = "1";
                        dataGridView2.Rows[n].Cells[2].Value = numberDocAval++;
                        dataGridView2.Rows[n].Cells[3].Value = DateTime.Today.ToString("dd.MM.yyyy");
                        dataGridView2.Rows[n].Cells[4].Value = aval.mfo;
                        dataGridView2.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                        dataGridView2.Rows[n].Cells[6].Value = aval.rahunok;
                        dataGridView2.Rows[n].Cells[7].Value = (CSV_Struct[i].rahunok);
                        dataGridView2.Rows[n].Cells[8].Value = CSV_Struct[i].summa;
                        dataGridView2.Rows[n].Cells[9].Value = "0";
                        dataGridView2.Rows[n].Cells[10].Value = findNameZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok).Equals("null") ? CSV_Struct[i].name : findNameZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok);
                        dataGridView2.Rows[n].Cells[12].Value = CSV_Struct[i].edrpou;
                        dataGridView2.Rows[n].Cells[11].Value = addDateToStr(findZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                            CSV_Struct[i].dateP.ToString("dd.MM.yyyy"));
                        if (dataGridView2.Rows[n].Cells[11].Value.Equals("null"))
                        {
                            dataGridView2.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                            int m = dataGridView3.Rows.Add();
                            dataGridView3.Rows[m].Cells[0].Value = CSV_Struct[i].name;
                            dataGridView3.Rows[m].Cells[1].Value = CSV_Struct[i].edrpou;
                            dataGridView3.Rows[m].Cells[2].Value = CSV_Struct[i].rahunok;
                            dataGridView3.Rows[m].Cells[3].Value = dataGridView2.Rows[n].Cells[11].Value;
                            isNull = true;
                        }
                    }
                    catch
                    {

                    }
                }

                if (isNull)
                {
                    Xml.saveXml(dataGridView3, path2);
                }
            }
        }
        public void autoOpenCsv()
        {
            isNull = false;
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
            }
            dataGridView3.Rows.Clear();
            Xml.loadXml(dataGridView3, path2);
            loadFileRoot();

            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            dataGridView2.Sort(dataGridView2.Columns[11], ListSortDirection.Ascending);

        }
        public string addDateToStr(string str, string date)
        {
            if (str.Equals("null"))
            {
                return "null";
            }
            str = str.Replace("##.##.####", date);
            return str;
        }

        public string findZkpo(string zkpo, string rrahunok)
        {
            foreach (DataGridViewRow r in dataGridView3.Rows) // пока в dataGridView1 есть строки
            {
                if (r.Cells != null)
                {
                    try
                    {
                        if (r.Cells[1].Value.Equals(zkpo) && r.Cells[2].Value.Equals(rrahunok))
                        {
                            return r.Cells[3].Value.ToString();
                        }
                    }
                    catch
                    {
                        return "null";
                    }
                }
            }
            return "null";
        }
        public string findNameZkpo(string zkpo, string rrahunok)
        {
            foreach (DataGridViewRow r in dataGridView3.Rows)
            {
                if (r.Cells != null)
                {
                    try
                    {
                        if (r.Cells[1].Value.Equals(zkpo) && r.Cells[2].Value.Equals(rrahunok))
                        {
                            return r.Cells[0].Value.ToString().ToUpper();
                        }
                    }
                    catch
                    {
                        return "null";
                    }
                }
            }
            return "null";
        }

        public string converterDate(string dateS)
        {
            if (!string.IsNullOrEmpty(dateS))
            {
                string t = dateS.Replace(".", "");
                return t.Substring(4, 4) + t.Substring(2, 2) + t.Substring(0, 2);
            }
            else
            {
                return "";
            }
        }

        public void Save()
        {
            saveFileDialog1.FileName = getNameFile();
            saveFileDialog1.Title = "Зберегти";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string k = "" + DateTime.Now;
                name = saveFileDialog1.FileName;
                createBox();
                string texts = textImport.Text.Replace("і", "i").Replace("І", "I");
                File.WriteAllText(name, texts, Encoding.GetEncoding(866));
            }
        }

        public void createBox()
        {
            foreach (DataGridViewRow r in dataGridView2.Rows) // пока в dataGridView1 есть строки
            {
                if (r.Cells != null)
                {
                    string t = "";
                    string sum = "";
                    try
                    {
                        t = r.Cells[3].Value.ToString();
                        sum = r.Cells[8].Value.ToString().Replace(".", "");
                    }
                    catch
                    {
                    }

                    textImport.Text += r.Cells[0].Value + P + r.Cells[1].Value + P + P + converterDate(t) + P;
                    textImport.Text += r.Cells[4].Value + P + r.Cells[5].Value + P + r.Cells[6].Value + P +
                                       r.Cells[7].Value + P;
                    textImport.Text += sum + P + r.Cells[9].Value + P + r.Cells[10].Value + P + r.Cells[11].Value + P +
                                       P + P + P + P + r.Cells[12].Value + P + P + "\r\n";
                }
            }
        }

        public string getNameFile()
        {
            string bcode = cliBankCode.Text.Insert(1, ".");
            string name = "R";
            name += dateTimePicker1.Value.Day.ToString().Length == 1 ? "0" + dateTimePicker1.Value.Day : dateTimePicker1.Value.Day.ToString();
            name += DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour : DateTime.Now.Hour.ToString();
            name += DateTime.Now.Minute + bcode + ".";
            return name;
        }

        public void saveExcel()
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel files(2007+)| *.xlsx|Excel Files(2003)|*.xls",
                FilterIndex = 2,
                FileName = DateTime.Now.ToString().Replace(":", "_")
            };
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                saveExcel(saveDialog, dataGridView1);
            }
        }

        public void saveExcel(SaveFileDialog saveDialog, DataGridView dataGridView1)
        {
            // Creating a Excel object.
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            try
            {
                progressBar1.Visible = true;
                ModifyProgressBarColor.SetState(progressBar1, 3);
                progressBar1.Minimum = 1;
                progressBar1.Maximum = dataGridView1.Rows.Count;
                progressBar1.Value = 1;
                progressBar1.Step = 1;

                worksheet = workbook.ActiveSheet;
                worksheet.Name = "ExportedFromDatGrid";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;

                for (int i = 0; i <= dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        if (cellRowIndex == 1)
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Columns[j].HeaderText;
                        }
                        else
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex].NumberFormat = "@";
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i-1].Cells[j].Value.ToString();
                        }

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                    progressBar1.PerformStep();
                }

                {
                    workbook.SaveAs(saveDialog.FileName);
                    MessageBox.Show("Експорт завершено ", "Інформація", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                    progressBar1.Value = 1;
                    progressBar1.Visible = false;
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void Mfo_TextChanged(object sender, EventArgs e)
        {
            aval.mfo = string.IsNullOrEmpty(mfo.Text) ? "0" : mfo.Text;
        }

        private void SaveFile_Click_1(object sender, EventArgs e)
        {
            saveExcel();
            Save();
        }



        public void isEditAval(bool edit)
        {
            cliBankCode.Visible = rahunok.Visible = mfo.Visible =
                rahunok.Visible = label1.Visible = label2.Visible = label5.Visible = edit;
        }

        public void isEditUkrG(bool edit)
        {
            textBox2.Visible = label6.Visible = label10.Visible = textBox4.Visible = label3.Visible = textIban.Visible = edit;
        }

        private void CliBankCode_TextChanged(object sender, EventArgs e)
        {
            aval.clientBankCode = cliBankCode.Text;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            Form frm = new Form2();
            dr = frm.ShowDialog();

            if (dr == DialogResult.OK && !string.IsNullOrEmpty(path))
            {
                autoOpenCsv();
            }
        }


        private void Button3_Click(object sender, EventArgs e)
        {
            editAval = !editAval;
            if (editAval)
            {
                comboEdr.Enabled = false;
                isEditAval(editAval);
                button3.Image = saveBtn;
            }
            else
            {
                button3.Image = editBtn;
                isEditAval(editAval);
                aval.name = comboEdr.Text;
                comboEdr.Items.Clear();
                comboEdr.Items.Add(aval.name);
                aval.name = comboEdr.Text;
                aval.mfo = mfo.Text;
                aval.rahunok = rahunok.Text;
                aval.clientBankCode = cliBankCode.Text;
                aval.id = 0;
                Xml.EditXml(aval, pathConfig);
                comboEdr.Enabled = true;


            }

        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            editUkrG = !editUkrG;

            if (editUkrG)
            {
                comboEdr2.Enabled = false;

                isEditUkrG(editUkrG);
                button5.Image = saveBtn;
            }
            else
            {
                button5.Image = editBtn;
                isEditUkrG(editUkrG);
                comboEdr2.Enabled = true;

                ukrGaz.edrpou = textBox2.Text;
                ukrGaz.rahunok = textBox4.Text;
                ukrGaz.iban = textIban.Text;
                ukrGaz.name = comboEdr2.Text;
                ukrGaz.id = 1;
                Xml.EditXml(ukrGaz, pathConfig);

            }
        }

        public void setFieldsUkrGaz()
        {
            textBox2.Text = ukrGaz.edrpou;
            textBox4.Text = ukrGaz.rahunok;
            textIban.Text = ukrGaz.iban;
        }
        private void comboEdr2_SelectedIndexChanged(object sender, EventArgs e)
        {
            setFieldsUkrGaz();
        }



        private void Label8_MouseClick(object sender, MouseEventArgs e)
        {
            tableLayoutPanel7.RowStyles[1].Height = 100;
            tableLayoutPanel7.RowStyles[0].Height = 0;
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;

        }



        private void Label9_MouseClick(object sender, MouseEventArgs e)
        {
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView2.Visible = false;
            dataGridView1.Visible = true;

        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            tableLayoutPanel7.RowStyles[1].Height = 100;
            tableLayoutPanel7.RowStyles[0].Height = 0;
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;
            gridHeader.Text = label8.Text;
            dataGridView2.Sort(dataGridView2.Columns[11], ListSortDirection.Ascending);
            textBox1.Text = string.Empty;
        }

        private void Panel2_MouseClick(object sender, MouseEventArgs e)
        {
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView2.Visible = false;
            dataGridView1.Visible = true;
            gridHeader.Text = label9.Text;
            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            textBox1.Text = string.Empty;
        }



        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int selRowNum = dataGridView2.SelectedCells[0].RowIndex;
            int selColNum = dataGridView2.SelectedCells[0].ColumnIndex;
            if (dataGridView2[e.ColumnIndex, e.RowIndex].Value != null)
            {
                if (selColNum == 11)
                {
                    dataGridView2.CurrentRow.Cells[11].Value =
                        MyDataGrid.shortText(dataGridView2.CurrentRow.Cells[11].Value.ToString());
                    //dataGridView2.CurrentRow.Cells[11].Value = dataGridView2.CurrentRow.Cells[11].Value.ToString().Replace("утримання", "утрим.").Replace("будинків", "буд.").Replace("утриман.", "утрим.").Replace("управління", "управл.").Replace("  ", @" ");
                    dataGridView2.CurrentRow.Cells[11].Value = dataGridView2.CurrentRow.Cells[11].Value.ToString().Replace("  ", @" ");
                    if (!currentCellValue.Equals(dataGridView2.CurrentRow.Cells[11].Value.ToString()))
                    {
                        DialogResult dialogResult = MessageBox.Show("Зміни записати базу данних", "Запис данних",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            int n = dataGridView1.Rows.Add();
                            dataGridView3.Rows[n].Cells[0].Value =
                                dataGridView2.Rows[selRowNum].Cells[selColNum - 1].Value; // 
                            dataGridView3.Rows[n].Cells[1].Value =
                                dataGridView2.Rows[selRowNum].Cells[selColNum + 1].Value; // 
                            dataGridView3.Rows[n].Cells[2].Value =
                                dataGridView2.Rows[selRowNum].Cells[selColNum].Value; // 
                            Xml.saveXml(dataGridView3, path2);
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int selRowNum = dataGridView1.SelectedCells[0].RowIndex;
            int selColNum = dataGridView1.SelectedCells[0].ColumnIndex;

            if (dataGridView1[e.ColumnIndex, e.RowIndex].Value != null)
            {
                if (selColNum == 2)
                {
                    if (!currentCellValue.Equals(dataGridView1.CurrentRow.Cells[2].Value.ToString()))
                    {
                        DialogResult dialogResult = MessageBox.Show("Зміни записати базу данних", "Запис данних",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            string pattern = @"за\s[0-9]{2}[.][0-9]{2}[.][0-9]{4}р\.";
                            string str = "";
                            int n = dataGridView3.Rows.Add();
                            dataGridView3.Rows[n].Cells[0].Value =
                                dataGridView1.Rows[selRowNum].Cells[selColNum + 6].Value; // 
                            dataGridView3.Rows[n].Cells[1].Value =
                                dataGridView1.Rows[selRowNum].Cells[selColNum + 5].Value; // 
                            try
                            {
                                str = dataGridView1.Rows[selRowNum].Cells[selColNum].Value.ToString();
                            }
                            catch (NullReferenceException) { }
                            string newLine = Regex.Replace(str, pattern, "  за ##.##.#### ");
                            dataGridView3.Rows[n].Cells[2].Value = newLine;
                            ; // 
                            Xml.saveXml(dataGridView3, path2);
                        }
                    }

                }
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            currentCellValue = dataGridView1.CurrentRow.Cells[2].Value.ToString();
        }

        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            currentCellValue = dataGridView2.CurrentRow.Cells[11].Value.ToString();
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            string rowIdx = (e.RowIndex + 1).ToString();

            StringFormat centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void dataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            string rowIdx = (e.RowIndex + 1).ToString();

            StringFormat centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.Visible)
            {
                int[] col = { 2, 6, 7, 8 };
                MyDataGrid.Filter(dataGridView1, textBox1.Text, col);
            }
            else
            {
                int[] col = { 7, 10, 11, 12 };
                MyDataGrid.Filter(dataGridView2, textBox1.Text, col);
            }
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            //DialogResult dr = new DialogResult();
            Form frm = new Form3();
            frm.StartPosition = FormStartPosition.CenterScreen;
             frm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += " "+  localVersion;
            new Update().Download();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}