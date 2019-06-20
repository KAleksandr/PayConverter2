using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Globalization;


namespace SoftGenConverter
{
    public partial class Form1 : Form
    {
        private string name;
        
        private bool shemes = true;//true=Aval false= UkrGaz 
        private TextBox textImport = new TextBox();
        
        
        
        

        
        
        
        
        bool editAval = false;
        bool editUkrG = false;
        Image editBtn = Properties.Resources.edit_property_16px;//
        Image saveBtn = Properties.Resources.save_as_16px;
       
        
        private long numberDocAval;
        private long numberDocUkrg;
        private string P = "·";
       
      
        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data.xml");
        public Form1()
        {

            InitializeComponent();
            initData();



        }

        public void initData()
        {
            Xml.loadXml(dataGridView3, path2);
            comboEdr.Items.Add(Properties.Settings.Default.name);
            comboEdr2.Items.Add(Properties.Settings.Default.name2);
            comboEdr2.Items.Add(Properties.Settings.Default.name3);
            numberDocAval = Properties.Settings.Default.platNumber;
            comboEdr.Text = Properties.Settings.Default.name;

            setFieldsP();
            setFieldsP2();

            isEditAval(editAval);
            isEditUkrG(editUkrG);
        }
        public void setFieldsP()
        {
           
            platNumber.Text = Properties.Settings.Default.platNumber.ToString();
            dateTimePicker1.Value = convertStrToTime(Properties.Settings.Default.datePayment.ToString());//
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
            if (Properties.Settings.Default.state == 2)
            {
                textBox2.Text = Properties.Settings.Default.edrpou;
                textBox1.Text = Properties.Settings.Default.platNumber2.ToString();
                numberDocUkrg = Properties.Settings.Default.platNumber2;
                comboEdr2.Text = Properties.Settings.Default.name2;

            }
            else
            {
                textBox2.Text = Properties.Settings.Default.edrpou2;
                textBox1.Text = Properties.Settings.Default.platNumber3.ToString();
                numberDocUkrg = Properties.Settings.Default.platNumber3;
                comboEdr2.Text = Properties.Settings.Default.name3;
            }
            
           
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            
        }

        

        

        private void OpenFile_Click(object sender, EventArgs e)
        {
            // openFiles();
            openCsv();
        }

        public void openCsv()
        {
            openFileDialog1.FileName = "file";//
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Rows.Clear();
                }
                name = openFileDialog1.FileName;
                List<Aval> CSV_Struct = new List<Aval>();
                CSV_Struct = Aval.ReadFile(name);
                for (int i = 0; i <= CSV_Struct.Count - 1; i++)
                {

                    int n;
                    if (CSV_Struct[i].isAval == 0)
                    {
                        n = dataGridView1.Rows.Add();

                        dataGridView1.Rows[n].Cells[0].Value = CSV_Struct[i].summa;
                        dataGridView1.Rows[n].Cells[1].Value = "UAH";
                        dataGridView1.Rows[n].Cells[2].Value = addDateToStr(findZkpo(CSV_Struct[i].zkpo),
                            dateTimePicker1.Value.ToString("dd.MM.yyyy"));
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
                        dataGridView1.Rows[n].Cells[8].Value = CSV_Struct[i].name;
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
                                dataGridView2.Rows[n].Cells[3].Value = CSV_Struct[i].dateP.ToString("dd.MM.yyyy");

                                dataGridView2.Rows[n].Cells[4].Value = Properties.Settings.Default.mfo;
                                dataGridView2.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                                dataGridView2.Rows[n].Cells[6].Value = Properties.Settings.Default.rahunok;
                                dataGridView2.Rows[n].Cells[7].Value = CSV_Struct[i].rahunok;
                                dataGridView2.Rows[n].Cells[8].Value = CSV_Struct[i].summa;
                                dataGridView2.Rows[n].Cells[9].Value = "0";
                                dataGridView2.Rows[n].Cells[10].Value = CSV_Struct[i].name;
                                dataGridView2.Rows[n].Cells[12].Value = CSV_Struct[i].zkpo;
                                dataGridView2.Rows[n].Cells[11].Value = addDateToStr(findZkpo(CSV_Struct[i].zkpo),
                                CSV_Struct[i].dateP.ToString("dd.MM.yyyy"));
                                if (dataGridView2.Rows[n].Cells[11].Value.Equals("null"))
                                {
                                    dataGridView2.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                                }
                        }
                        catch (Exception)
                        {

                        }
                    }





                }
            }
             
        }
        public string addDateToStr(string str, string date)
        {
            if (str.Equals("null")) return "null";
            str = str.Replace("##.##.####", date);
            return str;
        }
       public  string findZkpo(string zkpo)
        {
            foreach (DataGridViewRow r in dataGridView3.Rows) // пока в dataGridView1 есть строки
            {
                    if (r.Cells != null)
                    {
                        try
                        {
                            if (r.Cells[1].Value.Equals(zkpo))
                            {
                                return r.Cells[2].Value.ToString();
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

      
        public DateTime convertStrToTime(string dateP)
        {
            DateTime CreatdDate;
            if (!dateP.Equals(0))
            {
                dateP = (dateP.Insert(4, "-")).Insert(7, "-");
                CreatdDate = DateTime.ParseExact(dateP, "yyyy-MM-dd",
                                               System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                CreatdDate = DateTime.Today;
            }


            return CreatdDate;
        }
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

                File.WriteAllText(name, textImport.Text);

            }
        }

        public void createBox()
        {
            
            bool flag = false;
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
                        catch (Exception) { }

                        if (flag)
                        {
                             
                            textImport.Text += Environment.NewLine;
                        }
                       
                            textImport.Text += r.Cells[0].Value + P + r.Cells[1].Value + P + r.Cells[2].Value + P + converterDate(t) + P;
                            textImport.Text += r.Cells[4].Value + P + r.Cells[5].Value + P + r.Cells[6].Value + P + r.Cells[7].Value + P;  
                            textImport.Text += sum + P + r.Cells[9].Value + P + r.Cells[10].Value + P + r.Cells[11].Value + P + P + P + P + P + r.Cells[12].Value + P + P; 
                            
                            flag = true;
                        
                    }

            }

           
        }



        public string getNameFile()
        {
            string bcode = cliBankCode.Text.Insert(1, ".");
            string name = "R";
            name += DateTime.Today.Day.ToString() + DateTime.Now.Hour + DateTime.Now.Minute + bcode;
            return name;
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            if (shemes)
            {
                saveExcel();
            }
            else
            {
                Save();
            }


        }

       
        public void saveExcel()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel files(2003)| *.xls|Excel Files(2007+)|*.xlsx"; ;
            saveDialog.FilterIndex = 2;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                ExportToExcel.saveExcel(saveDialog, dataGridView1);
            }
        }


       
        private void PlatNumber_TextChanged(object sender, EventArgs e)
        {
            numberDocAval = Properties.Settings.Default.platNumber = string.IsNullOrEmpty(platNumber.Text) ? 0 : Int64.Parse(platNumber.Text);
            
        }

        private void Mfo_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.mfo = string.IsNullOrEmpty(mfo.Text) ? "0" : mfo.Text;
            Properties.Settings.Default.Save();
            
        }

        private void Rahunok_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.rahunok = string.IsNullOrEmpty(rahunok.Text) ? "0" : rahunok.Text;
            
        }

        private void SaveFile_Click_1(object sender, EventArgs e)
        {
          
                saveExcel();
                Save();
          

        }

        private void УкрГазToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            
            label2.Text = "ЕДРПОУ Платника:";
            //setFields2();
            setFieldsP2();
            
        }

       
        private void АвальToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            setFieldsP();
            
        }
        public void isEditAval(bool edit)
        {
            cliBankCode.Visible = platNumber.Visible = mfo.Visible = rahunok.Visible =  label1.Visible = label2.Visible = label3.Visible =   label5.Visible =   edit;
        }
        public void isEditUkrG(bool edit)
        {
            textBox2.Visible =  textBox1.Visible = label7.Visible = label6.Visible = label10.Visible = textBox4.Visible = edit;
        }


       
        private void CliBankCode_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.clientBankCode = cliBankCode.Text;
            Properties.Settings.Default.Save();
        }
   

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           // WriteIni(recviz);
            if (shemes)
            {
                Properties.Settings.Default.state1 = 2;
                Properties.Settings.Default.Save();
                
            }
            else
            {
                Properties.Settings.Default.state1 = 1;
                Properties.Settings.Default.Save();
              
            }

        }

     

     

        private void button2_Click(object sender, EventArgs e)
        {
            Form frm = new Form2();
            frm.ShowDialog();
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
            }
        }

     
        private void button5_Click_2(object sender, EventArgs e)
        {
           //MessageBox.Show(Properties.Settings.Default.state.ToString());
            editUkrG = !editUkrG;
            if (editUkrG)
            {
                comboEdr2.Visible = !editUkrG;
                textBox3.Visible = editUkrG;
               // if (comboEdr2.Text.Equals(Properties.Settings.Default.name2))
               if(Properties.Settings.Default.state == 2 )
                {
                    textBox2.Text = Properties.Settings.Default.edrpou;
                    textBox1.Text = Properties.Settings.Default.platNumber2.ToString();
                    textBox3.Text = Properties.Settings.Default.name2;
                    textBox4.Text = Properties.Settings.Default.rahunok2;

                }
                else
                {
                    textBox3.Text = Properties.Settings.Default.name3;
                    textBox2.Text = Properties.Settings.Default.edrpou2;
                    textBox1.Text = Properties.Settings.Default.platNumber3.ToString();
                    textBox4.Text = Properties.Settings.Default.rahunok3;
                }

                isEditUkrG(editUkrG);
                button3.Image = saveBtn;
            }
            else
            {
                button3.Image = editBtn;
                isEditUkrG(editUkrG);
                comboEdr2.Visible = !editUkrG;
                textBox3.Visible = editUkrG;
                if (Properties.Settings.Default.state == 2)
                {
                    //MessageBox.Show(Properties.Settings.Default.state.ToString());
                    Properties.Settings.Default.name2 = textBox3.Text;
                    Properties.Settings.Default.edrpou = textBox2.Text;
                    Properties.Settings.Default.platNumber2 = Int64.Parse(textBox1.Text);
                    Properties.Settings.Default.rahunok2 = textBox4.Text;
                    Properties.Settings.Default.Save();
                    comboEdr2.Items.Clear();
                    comboEdr2.Items.Add(Properties.Settings.Default.name2);
                    comboEdr2.Items.Add(Properties.Settings.Default.name3);
                    comboEdr2.Text = Properties.Settings.Default.name2;
                }
                else
                {
                    //MessageBox.Show(Properties.Settings.Default.state.ToString());
                    Properties.Settings.Default.name3 = textBox3.Text;
                    Properties.Settings.Default.edrpou2 = textBox2.Text;
                    Properties.Settings.Default.platNumber3 = Int64.Parse(textBox1.Text);
                    Properties.Settings.Default.rahunok3 = textBox4.Text;
                    Properties.Settings.Default.Save();
                    comboEdr2.Items.Clear();
                    comboEdr2.Items.Add(Properties.Settings.Default.name2);
                    comboEdr2.Items.Add(Properties.Settings.Default.name3);
                    comboEdr2.Text = Properties.Settings.Default.name3;
                }
            }
        }

        private void comboEdr2_SelectedIndexChanged(object sender, EventArgs e)
        {
           // MessageBox.Show(Properties.Settings.Default.state.ToString());
            if (comboEdr2.Text.Equals(Properties.Settings.Default.name2))
            {
                Properties.Settings.Default.state = 2;
                numberDocUkrg = Properties.Settings.Default.platNumber2;
                Properties.Settings.Default.Save();
            }
            else
            {
                numberDocUkrg = Properties.Settings.Default.platNumber3;
                Properties.Settings.Default.state = 3;
                Properties.Settings.Default.Save();
            }
           // MessageBox.Show(Properties.Settings.Default.state.ToString());
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (comboEdr2.Text.Equals(Properties.Settings.Default.name2))
            {
                Properties.Settings.Default.name2 = textBox3.Text;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.name3 = textBox3.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            if (comboEdr2.Text.Equals(Properties.Settings.Default.name2))
            {
                Properties.Settings.Default.edrpou = string.IsNullOrEmpty(platNumber.Text) ? "0" : platNumber.Text;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.edrpou2 = string.IsNullOrEmpty(platNumber.Text) ? "0" : platNumber.Text;
                Properties.Settings.Default.Save();
            }
           
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (comboEdr2.Text.Equals(Properties.Settings.Default.name2))
            {
                try
                {
                    Properties.Settings.Default.platNumber2 = Int64.Parse(textBox1.Text);
                    Properties.Settings.Default.Save();
                }
                catch (System.FormatException)
                {
                    Properties.Settings.Default.platNumber2 = 0;
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                try
                {
                    Properties.Settings.Default.platNumber3 = Int64.Parse(textBox1.Text);
                    Properties.Settings.Default.Save();
                }
                catch (System.FormatException)
                {
                    Properties.Settings.Default.platNumber3 = 0;
                    Properties.Settings.Default.Save();
                }
                
            }
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            //Properties.Settings.Default.datePayment = dateTimePicker1.Value;
            Properties.Settings.Default.Save();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (comboEdr2.Text.Equals(Properties.Settings.Default.name2))
            {
                Properties.Settings.Default.rahunok2 = string.IsNullOrEmpty(textBox4.Text) ? "0" : textBox4.Text;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.rahunok3 = string.IsNullOrEmpty(textBox4.Text) ? "0" : textBox4.Text;
                Properties.Settings.Default.Save();
            }
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
        }

        private void Panel2_MouseClick(object sender, MouseEventArgs e)
        {
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView2.Visible = false;
            dataGridView1.Visible = true;
            gridHeader.Text = label9.Text;
        }
        //запис в data.xml призначення платежу
        private void DataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int selRowNum = dataGridView2.SelectedCells[0].RowIndex;
            int selColNum = dataGridView2.SelectedCells[0].ColumnIndex;
            if (dataGridView2[e.ColumnIndex, e.RowIndex].Value != null)
                if (selColNum == 11)
                {
                    DialogResult dialogResult = MessageBox.Show("Зміни записати базу данних", "Запис данних", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        int n = dataGridView1.Rows.Add();
                        dataGridView3.Rows[n].Cells[0].Value = dataGridView2.Rows[selRowNum].Cells[selColNum - 1].Value; // 
                        dataGridView3.Rows[n].Cells[1].Value = dataGridView2.Rows[selRowNum].Cells[selColNum + 1].Value; // 
                        dataGridView3.Rows[n].Cells[2].Value = dataGridView2.Rows[selRowNum].Cells[selColNum].Value; // 
                        Xml.saveXml(dataGridView3, path2);
                    }

                }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int selRowNum = dataGridView2.SelectedCells[0].RowIndex;
            int selColNum = dataGridView2.SelectedCells[0].ColumnIndex;
            if (dataGridView2[e.ColumnIndex, e.RowIndex].Value != null)
                if (selColNum == 11)
                {
                    DialogResult dialogResult = MessageBox.Show("Зміни записати базу данних", "Запис данних", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        int n = dataGridView1.Rows.Add();
                        dataGridView3.Rows[n].Cells[0].Value = dataGridView2.Rows[selRowNum].Cells[selColNum - 1].Value; // 
                        dataGridView3.Rows[n].Cells[1].Value = dataGridView2.Rows[selRowNum].Cells[selColNum + 1].Value; // 
                        dataGridView3.Rows[n].Cells[2].Value = dataGridView2.Rows[selRowNum].Cells[selColNum].Value; // 
                        Xml.saveXml(dataGridView3, path2);
                    }

                }
        }
    }
}