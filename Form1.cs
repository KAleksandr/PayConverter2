using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Globalization;


namespace SoftGenConverter
{
    public partial class Form1 : Form
    {
        private string name;

        
        private TextBox textImport = new TextBox();
        private string currentCellValue = "";

        bool editAval = false;
        bool editUkrG = false;
        Image editBtn = Properties.Resources.edit_property_16px; //
        Image saveBtn = Properties.Resources.save_as_16px;
        private int state = Properties.Settings.Default.state;

        private long numberDocAval;
        
        private string P = "·";
        
        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterData.xml");
       private string path3 = Properties.Resources.PayConverterData;
        private string path = "";



        

        public Form1()
        {
            InitializeComponent();
            initData();
            
        }

        public void initData()
        {
            Xml.loadXml(dataGridView3, path2);
            comboEdr.Items.Add(Properties.Settings.Default.name);
                // comboEdr2.Items.Add(Properties.Settings.Default.name2);
            //comboEdr2.Items.Add(Properties.Settings.Default.name3);
           
            comboEdr.Text = Properties.Settings.Default.name;

            setFieldsP();
            setFieldsP2();

            isEditAval(editAval);
            isEditUkrG(editUkrG);
            Aval.StyleDataGridView(dataGridView1, false);
            Aval.StyleDataGridView(dataGridView2, false);
            
                 comboEdr2.SelectedIndex = 0;
                 //setFieldsUkrGaz();
            
        }


        public void setFieldsP()
        {

            
            dateTimePicker1.Value = DateTime.Now;//convertStrToTime(Properties.Settings.Default.datePayment.ToString()); //
            mfo.Text = Properties.Settings.Default.mfo;
            rahunok.Text = Properties.Settings.Default.rahunok;
            cliBankCode.Text = Properties.Settings.Default.clientBankCode;


            tableLayoutPanel7.RowStyles[1].Height = 100;
            tableLayoutPanel7.RowStyles[0].Height = 0;
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;



        }

        public void setFieldsP2()
        {
            
                textBox2.Text = Properties.Settings.Default.edrpou;
                textBox4.Text = Properties.Settings.Default.rahunok2;
               
                //comboEdr2.Text = Properties.Settings.Default.name2;

           
            
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;

        }





        private void OpenFile_Click(object sender, EventArgs e)
        {
            // openFiles();
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

                path=name = openFileDialog1.FileName;
                loadFileRoot();

               
                if (isNull)
                {
                    Xml.saveXml(dataGridView3, path2);
                }
            }

        }

        public void loadFileRoot()
        {
            List<Aval> CSV_Struct = new List<Aval>();
            CSV_Struct = Aval.ReadFile(path);
            DateTime dt1 = DateTime.Today;
            for (int i = 0; i <= CSV_Struct.Count - 1; i++)
            {

                int n;
                if (CSV_Struct[i].isAval == 0)
                {
                    n = dataGridView1.Rows.Add();

                    dataGridView1.Rows[n].Cells[0].Value = CSV_Struct[i].summa;
                    dataGridView1.Rows[n].Cells[1].Value = "UAH";
                    dataGridView1.Rows[n].Cells[2].Value = addDateToStr(findZkpo(CSV_Struct[i].zkpo, CSV_Struct[i].rahunok),
                       (CSV_Struct[i].dateP==dt1 ? dateTimePicker1.Value.ToString("dd.MM.yyyy") : CSV_Struct[i].dateP.ToString("dd.MM.yyyy")));
                    //dateTimePicker1.Value.ToString("dd.MM.yyyy"));
                    if (dataGridView1.Rows[n].Cells[2].Value.Equals("null") )
                    {
                        dataGridView1.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                        int m = dataGridView3.Rows.Add();
                        dataGridView3.Rows[m].Cells[0].Value = CSV_Struct[i].name;
                        dataGridView3.Rows[m].Cells[1].Value = CSV_Struct[i].zkpo;
                        dataGridView3.Rows[m].Cells[2].Value =  CSV_Struct[i].rahunok;
                        dataGridView3.Rows[m].Cells[3].Value = dataGridView1.Rows[n].Cells[2].Value;
                        isNull = true;
                    }

                    ;
                    //dataGridView1.Rows[n].Cells[3].Value = CSV_Struct[i].datePayment.ToString();
                    //dataGridView1.Rows[n].Cells[4].Value = CSV_Struct[i].zkpo;
                    if (Properties.Settings.Default.state == 2)
                    {
                        dataGridView1.Rows[n].Cells[3].Value = Properties.Settings.Default.rahunok2;
                        dataGridView1.Rows[n].Cells[4].Value = Properties.Settings.Default.edrpou;

                    }
                    else
                    {
                        dataGridView1.Rows[n].Cells[3].Value = Properties.Settings.Default.rahunok3;
                        dataGridView1.Rows[n].Cells[4].Value = Properties.Settings.Default.edrpou2;

                    }

                    dataGridView1.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                    dataGridView1.Rows[n].Cells[6].Value = CSV_Struct[i].rahunok;
                    dataGridView1.Rows[n].Cells[7].Value = CSV_Struct[i].zkpo;
                    dataGridView1.Rows[n].Cells[8].Value = findNameZkpo(CSV_Struct[i].zkpo, CSV_Struct[i].rahunok).Equals("null") ? CSV_Struct[i].name : findNameZkpo(CSV_Struct[i].zkpo, CSV_Struct[i].rahunok);
                }

                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                if (CSV_Struct[i].isAval == 1)
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
                            //CSV_Struct[i].dateP.ToString("dd.MM.yyyy");

                        dataGridView2.Rows[n].Cells[4].Value = Properties.Settings.Default.mfo;
                        dataGridView2.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                        dataGridView2.Rows[n].Cells[6].Value = Properties.Settings.Default.rahunok;
                        dataGridView2.Rows[n].Cells[7].Value = Convert.ToInt64(CSV_Struct[i].rahunok);
                        dataGridView2.Rows[n].Cells[8].Value = CSV_Struct[i].summa;
                        dataGridView2.Rows[n].Cells[9].Value = "0";
                        dataGridView2.Rows[n].Cells[10].Value = findNameZkpo(CSV_Struct[i].zkpo, CSV_Struct[i].rahunok).Equals("null") ? CSV_Struct[i].name : findNameZkpo(CSV_Struct[i].zkpo,CSV_Struct[i].rahunok);
                        dataGridView2.Rows[n].Cells[12].Value = CSV_Struct[i].zkpo;
                        dataGridView2.Rows[n].Cells[11].Value = addDateToStr(findZkpo(CSV_Struct[i].zkpo, CSV_Struct[i].rahunok),
                            CSV_Struct[i].dateP.ToString("dd.MM.yyyy"));
                        if (dataGridView2.Rows[n].Cells[11].Value.Equals("null"))
                        {
                            dataGridView2.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                            int m = dataGridView3.Rows.Add();
                            dataGridView3.Rows[m].Cells[0].Value = CSV_Struct[i].name;
                            dataGridView3.Rows[m].Cells[1].Value = CSV_Struct[i].zkpo;
                            dataGridView3.Rows[m].Cells[2].Value = CSV_Struct[i].rahunok;
                            dataGridView3.Rows[m].Cells[3].Value = dataGridView2.Rows[n].Cells[11].Value;
                            isNull = true;
                        }
                    }
                    catch (Exception)
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
               // dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
        }
        public string addDateToStr(string str, string date)
        {
            if (str.Equals("null")) return "null";
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
                    catch (Exception)
                    {
                        return "null";
                    }


                }

            }


            return "null";
        }
        public string findNameZkpo(string zkpo, string rrahunok)
        {
            foreach (DataGridViewRow r in dataGridView3.Rows) // пока в dataGridView1 есть строки
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
                    catch (Exception)
                    {
                        return "null";
                    }


                }

            }


            return "null";
        }

        //public DateTime convertStrToTime(string dateP)
        //{
        //    DateTime CreatdDate = DateTime.Today;
        //    if (!dateP.Equals(0))
        //    {
        //        try
        //        {
        //            dateP = (dateP.Insert(4, "-")).Insert(7, "-");
        //            CreatdDate = DateTime.ParseExact(dateP, "yyyy-MM-dd",
        //                System.Globalization.CultureInfo.InvariantCulture);
        //        }
        //        catch (Exception)
        //        {

        //        }

        //    }



        //    return CreatdDate;
        //}

        public string converterDate(string dateS)
        {
            if (!string.IsNullOrEmpty(dateS))
            {
                string t = dateS.Replace(".", "");
                return t.Substring(4, 4) + t.Substring(2, 2) + t.Substring(0, 2);
            }
            else
                return "";


        }



        private void DropDownButton1_Click(object sender, EventArgs e)
        {


            contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);


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

                string texts = textImport.Text.Replace("і", "i").Replace("І","I");


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
                    catch (Exception)
                    {
                    }

                    //if (flag)
                    //{

                    //    textImport.Text += Environment.NewLine;
                    //}

                    textImport.Text += r.Cells[0].Value + P + r.Cells[1].Value + P + P + converterDate(t) + P;
                    textImport.Text += r.Cells[4].Value + P + r.Cells[5].Value + P + r.Cells[6].Value + P +
                                       r.Cells[7].Value + P;
                    textImport.Text += sum + P + r.Cells[9].Value + P + r.Cells[10].Value + P + r.Cells[11].Value + P +
                                       P + P + P + P + r.Cells[12].Value + P + P + "\r\n";

                    // flag = true;

                }

            }


        }



        public string getNameFile()
        {
            string bcode = cliBankCode.Text.Insert(1, ".");
            string name = "R";
            name += dateTimePicker1.Value.Day.ToString() + DateTime.Now.Hour + DateTime.Now.Minute + bcode + ".";
            return name;
        }

        


        public void saveExcel()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel files(2007+)| *.xlsx|Excel Files(2003)|*.xls";
            ;
            saveDialog.FilterIndex = 2;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                ExportToExcel.saveExcel(saveDialog, dataGridView1);
            }
        }



       

        private void Mfo_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.mfo = string.IsNullOrEmpty(mfo.Text) ? "0" : mfo.Text;
            Properties.Settings.Default.Save();

        }

        

        private void SaveFile_Click_1(object sender, EventArgs e)
        {

            saveExcel();
            Save();


        }

        private void УкрГазToolStripMenuItem_Click(object sender, EventArgs e)
        {


            //  label2.Text = "ЕДРПОУ Платника:";
            //setFields2();
            setFieldsP2();

        }


        private void АвальToolStripMenuItem_Click(object sender, EventArgs e)
        {

            setFieldsP();

        }

        public void isEditAval(bool edit)
        {
            cliBankCode.Visible = rahunok.Visible = mfo.Visible =
                rahunok.Visible = label1.Visible = label2.Visible =  label5.Visible = edit;
        }

        public void isEditUkrG(bool edit)
        {
            textBox2.Visible =  label6.Visible = label10.Visible = textBox4.Visible = edit;
        }



        private void CliBankCode_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.clientBankCode = cliBankCode.Text;
            Properties.Settings.Default.Save();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // WriteIni(recviz);
            if (state==2)
            {
                Properties.Settings.Default.state = state;
                Properties.Settings.Default.Save();

            }
            else
            {
                Properties.Settings.Default.state = state;
                Properties.Settings.Default.Save();

            }

            

        }





        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            Form frm = new Form2();
            dr = frm.ShowDialog();
           // MessageBox.Show("" + dr);
            if (dr == DialogResult.OK && !string.IsNullOrEmpty(path))
            {
                //MessageBox.Show(path + " " + dr);
                autoOpenCsv();
               
            }
        }


        private void Button3_Click(object sender, EventArgs e)
        {

            editAval = !editAval;
            if (editAval)
            {
                isEditAval(editAval);
                button3.Image = saveBtn;
            }
            else
            {
                button3.Image = editBtn;
                isEditAval(editAval);
                Properties.Settings.Default.name = comboEdr.Text;
                comboEdr.Items.Clear();
                comboEdr.Items.Add(Properties.Settings.Default.name);

                Properties.Settings.Default.mfo = mfo.Text;
                Properties.Settings.Default.rahunok = rahunok.Text;
                Properties.Settings.Default.clientBankCode = cliBankCode.Text;
                Properties.Settings.Default.Save();
            }
        }


        private void button5_Click_2(object sender, EventArgs e)
        {
            //MessageBox.Show(Properties.Settings.Default.state.ToString());
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

                
                   Properties.Settings.Default.edrpou = textBox2.Text;
                   Properties.Settings.Default.rahunok2 = textBox4.Text;
                   Properties.Settings.Default.state = 2;
                    Properties.Settings.Default.Save();

                
               
            }
        }

        public void setFieldsUkrGaz()
        {
           
                
                
                textBox2.Text = Properties.Settings.Default.edrpou;
                textBox4.Text = Properties.Settings.Default.rahunok2;
            
           
        }
        private void comboEdr2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            //string selectedState = comboEdr2.SelectedItem.ToString();
           
            
                state = 2;
                setFieldsUkrGaz();
           
            
        }


        private bool changeDataG = false; // false -  dataGrid1, true - dataGrid2
        private void Label8_MouseClick(object sender, MouseEventArgs e)
        {
            tableLayoutPanel7.RowStyles[1].Height = 100;
            tableLayoutPanel7.RowStyles[0].Height = 0;
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;
            changeDataG = false;
        }



        private void Label9_MouseClick(object sender, MouseEventArgs e)
        {
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView2.Visible = false;
            dataGridView1.Visible = true;
            changeDataG = true;
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
                if (selColNum == 11)
                {
                    dataGridView2.CurrentRow.Cells[11].Value =
                        Aval.shortText(dataGridView2.CurrentRow.Cells[11].Value.ToString());
                    //dataGridView2.CurrentRow.Cells[11].Value = dataGridView2.CurrentRow.Cells[11].Value.ToString().Replace("утримання", "утрим.").Replace("будинків", "буд.").Replace("утриман.", "утрим.").Replace("управління", "управл.").Replace("  ", @" ");
                    dataGridView2.CurrentRow.Cells[11].Value = dataGridView2.CurrentRow.Cells[11].Value.ToString().Replace("  ", @" ");
                    if (!currentCellValue.Equals(dataGridView2.CurrentRow.Cells[11].Value.ToString()))
                    {
                        DialogResult dialogResult = MessageBox.Show("Зміни записати базу данних", "Запис данних",
                            MessageBoxButtons.YesNo);
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
                            MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            string pattern = @"за\s[0-9]{2}[.][0-9]{2}[.][0-9]{4}р\.";
                            string str="";
                            int n = dataGridView3.Rows.Add();
                            dataGridView3.Rows[n].Cells[0].Value =
                                dataGridView1.Rows[selRowNum].Cells[selColNum + 6].Value; // 
                            dataGridView3.Rows[n].Cells[1].Value =
                                dataGridView1.Rows[selRowNum].Cells[selColNum + 5].Value; // 
                            try
                            {
                                 str = dataGridView1.Rows[selRowNum].Cells[selColNum].Value.ToString();
                            }
                            catch (Exception) { }
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
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void dataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.Visible)
            {

                int[] col = { 2, 6, 7, 8 };
            Aval.Filter(dataGridView1, textBox1.Text, col);
            }
            else
            {
                int[] col = { 4, 10, 11, 12 };
                Aval.Filter(dataGridView2, textBox1.Text, col);
            }

        }
    }
}