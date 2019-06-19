﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Data;


namespace SoftGenConverter
{
    public partial class Form1 : Form
    {
        private string name;
        private int MAX = 999;
        private bool shemes = true;//true=Aval false= UkrGaz 
        private TextBox textImport = new TextBox();
        
        private List<Bank> banks = new List<Bank>();
        private Bank aval = new Bank();
        private Bank ukrBank = new Bank();
        //private string xmlConfig = "config.xml";

        Datashit recviz = new Datashit();
        string[] recvizs;
        string path;
        bool edit = false;
        Image editBtn = Properties.Resources.edit_property_16px;//
        Image saveBtn = Properties.Resources.save_as_16px;
        private string[] data = { "11", "22", "33", "44", "55", "66", "77", "88", "99" };
        // private CsvExport myExport = new CsvExport();
        // private Account new1 = new Account();
        private long numberDoc;
        private long numberDocAval;
        private long numberDocUkrg;
        private string P = "·";
        //private IniFile myIni;
      
        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data.xml");
        public Form1()
        {

            InitializeComponent();
           
            Xml.loadXml(dataGridView3,path2);
            comboEdr2.Items.Add(Properties.Settings.Default.name);
            comboEdr.Items.Add(Properties.Settings.Default.name2);
            comboEdr.Items.Add(Properties.Settings.Default.name3);
            numberDocAval = Properties.Settings.Default.platNumber;
            if(Properties.Settings.Default.state ==2)
                numberDocUkrg = Properties.Settings.Default.platNumber2;
            else
            {
                numberDocUkrg = Properties.Settings.Default.platNumber3;
            }

            //comboBox1.Items.Insert(1, "Боливия");


            if (shemes = Properties.Settings.Default.state1 == 1 ? true : false)
                {
                   //setFields();
                    setFieldsP();
                    texVisible(shemes);
                   

                }
                else
                {
                    setFieldsP2();
                    //setFields2();
                    texVisible(shemes);

                }

                isEdit(edit);
           
        }

        public void setFieldsP()
        {
            currentSheme.Text = Properties.Settings.Default.name;
            platNumber.Text = Properties.Settings.Default.platNumber.ToString();
            dateTimePicker1.Value = convertStrToTime(Properties.Settings.Default.datePayment.ToString());//
            mfo.Text = Properties.Settings.Default.mfo.ToString();
            rahunok.Text = Properties.Settings.Default.rahunok;

            cliBankCode.Text = Properties.Settings.Default.clientBankCode;
            recivPayNum.Text = Properties.Settings.Default.recivePayNum;
            if (Properties.Settings.Default.state1 == 1)
            {
                shemes = true;
            }
            else
            {
                shemes = false;
            }
            tableLayoutPanel7.RowStyles[1].Height = 100;
            tableLayoutPanel7.RowStyles[0].Height = 0;
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;
            
            

        }

        public void setFieldsP2()
        {
            platNumber.Text = Properties.Settings.Default.platNumber2.ToString();
            mfo.Text = Properties.Settings.Default.edrpou.ToString();
            rahunok.Text = Properties.Settings.Default.rahunok2;
            currentSheme.Text = Properties.Settings.Default.name2;
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
                    
                    int n = dataGridView1.Rows.Add();

                    dataGridView1.Rows[n].Cells[0].Value = "0";
                    dataGridView1.Rows[n].Cells[1].Value = "1";
                    dataGridView1.Rows[n].Cells[2].Value = numberDocUkrg++;
                    dataGridView1.Rows[n].Cells[3].Value = CSV_Struct[i].datePayment.ToString();
                    dataGridView1.Rows[n].Cells[4].Value = "!!";
                    dataGridView1.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                    dataGridView1.Rows[n].Cells[6].Value = "00";
                    dataGridView1.Rows[n].Cells[7].Value = CSV_Struct[i].rahunok;
                    dataGridView1.Rows[n].Cells[8].Value = CSV_Struct[i].summa;
                    //dataGridView1.Rows[n].Cells[9].Value = "0";
                   // dataGridView1.Rows[n].Cells[10].Value = CSV_Struct[i].name;

                    n  = dataGridView2.Rows.Add();
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
                    dataGridView2.Rows[n].Cells[11].Value = addDateToStr(findZkpo(CSV_Struct[i].zkpo), CSV_Struct[i].dateP.ToString("dd.MM.yyyy"));
                    



                }
            }
             
            
                
            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"RO1306__.csv");
            

            //Заполняем listView из нашей структуры
            
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
                        catch (Exception e)
                        {
                            return "null";
                        }
                       
                                              
                    }

                }
           

            return "null";
        }
        public void openFiles()
        {
            numberDoc = string.IsNullOrEmpty(platNumber.Text) ? 0 : Convert.ToInt32(platNumber.Text);
            int columnCount = 0;
            int rowcount = 0;
            string[] arText = new string[11];
            string[] arText2 = new string[10];
            int[] counts = new int[11];
            string date = "";
            openFileDialog1.FileName = "file";//
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                name = openFileDialog1.FileName;
                // currentSheme.Clear();
                string[] text = File.ReadAllLines(name, Encoding.GetEncoding(1251));
                int count = 0;
                Regex regexDate = new Regex(@"\w*[0-9]{2}[.][0-9]{2}[.][0-9]{2}\w*");
                Regex regex = new Regex(@"^[-]+[+][-]+[+][-]+[+][-]+[+][-]+[+][-]+[+][-]+[+][-]+[+][-]+[+][-]+[+][-]+[+]");
                Regex nextPage = new Regex(@"\W{1}");
                Regex regexAv = new Regex(@"^[-]{100}");
                int regAval = 0;
                int countStr = MAX;
                int countStr2 = MAX;
                int chekLines = 0;
                foreach (string line in text)
                {
                    MatchCollection matches = regex.Matches(line);
                    MatchCollection dateMatch = regexDate.Matches(line);
                    MatchCollection regexAval = regexAv.Matches(line);

                    if (line.Length == 1)
                    {
                        count++;
                        chekLines = 0;
                        countStr = MAX;
                        continue;

                    }
                    if (line.Contains("Всього"))
                    {
                        //chekLines = 0;
                        rowcount=0;
                        countStr = MAX;
                        // continue;
                    }
                    if (matches.Count > 0)
                    {
                        // textBox1.Text = line;

                        chekLines++;
                        // break;
                        if (chekLines == 2)
                        {
                            countStr = count + 2;


                        }
                    }
                    if (regexAval.Count > 0)
                    {
                        regAval++;

                        if (regAval == 2)
                        {
                            countStr2 = count + 1;

                        }
                        else if (regAval == 3)
                        {
                            countStr2 = MAX;
                            break;
                        }
                    }

                    if (dateMatch.Count > 0)
                    {
                        MatchCollection matchess = Regex.Matches(line, regexDate.ToString(), RegexOptions.IgnoreCase);

                        date = "" + matchess[0] + Environment.NewLine;
                    }
                    line.IndexOf('+');
                    //textBox1.Text = ""+ line.Length;
                    //textBox1.Text = line.Substring(0, 31).Trim() + " " + line.Substring(31, 20).Trim();
                    if (count >= 0 && count < 5)
                    {
                        // textBox1.Text += line + Environment.NewLine;
                    }
                    if (count >= countStr)
                    {
                        string newLine = "";

                        arText[0] = line.Substring(0, 31).Trim();
                        arText[1] = line.Substring(31, 20).Trim();
                        arText[2] = line.Substring(52, 6).Trim();
                        arText[3] = line.Substring(59, 14).Trim();
                        arText[4] = line.Substring(74, 10).Trim();
                        arText[5] = line.Substring(85, 12).Trim();
                        arText[6] = line.Substring(99, 12).Trim();
                        arText[7] = line.Substring(112, 10).Trim();
                        arText[8] = line.Substring(122, 10).Trim();
                        arText[9] = line.Substring(133, 10).Trim();
                        arText[10] = line.Substring(144, 7).Trim();


                        dataGridView1.Rows.Add();//·



                        string date1 = ((date.Replace(".", "")).Remove(4, 3)).Substring(0, 4) + DateTime.Today.Year;

                        newLine = "0·1·" + recviz.platNumber + P + converterDateToInt(dateTimePicker1.Value) + P + arText[2] + P + recviz.mfo + P + arText[3] + P + recviz.rahunok + P + arText[5].Replace(".", "")
                            + P + "0" + P + arText[0] + P + recivPayNum.Text + P + date1 + P + P + P + P + arText[4] + P + P + Environment.NewLine;
                        numberDoc++;
                        textImport.Text += newLine;
                        foreach (string grid in arText)
                        {
                            dataGridView1.Rows[rowcount].Cells[columnCount++].Value = grid;

                        }
                        rowcount++;
                        columnCount = 0;

                    }
                    if (count >= countStr2)
                    {
                        dataGridView2.Rows.Add();//·download character 2
                        arText2[0] = line.Substring(0, 20).Trim();
                        arText2[1] = line.Substring(20, 15).Trim();
                        arText2[2] = line.Substring(34, 6).Trim();
                        arText2[3] = line.Substring(40, 14).Trim();
                        arText2[4] = line.Substring(53, 8).Trim();
                        arText2[5] = line.Substring(65, 12).Trim();
                        arText2[6] = line.Substring(77, 12).Trim();
                        arText2[7] = line.Substring(89, 9).Trim();
                        arText2[8] = line.Substring(98, 11).Trim();
                        arText2[9] = line.Substring(109, 8).Trim();
                        foreach (string grid in arText2)
                        {
                           // dataGridView3.Rows[rowcount].Cells[columnCount++].Value = grid;

                        }
                        rowcount++;
                        columnCount = 0;
                    }
                    count++;


                }




               


            }
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
        public int converterDateToInt(DateTime date)
        {
            string dateP = string.Format("{0: yyyyMMdd}", date);

            return Convert.ToInt32(dateP);
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

                File.WriteAllText(name, textImport.Text);

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

        void progesDialog()
        {
            for (int i = 0; i <= 500; i++)
            {
                Thread.Sleep(20);
            }
        }

        public void saveExcel()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel files(2003)| *.xls|Excel Files(2007+)|*.xlsx"; ;
            saveDialog.FilterIndex = 2;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                //                using (ProcessLoadForm lfrm = new ProcessLoadForm(progesDialog))
                //                {
                //                    lfrm.ShowDialog(this);
                //  new Form1())              }
                // formaLoad = new ProcessLoadForm(OnWorkIsDone);
                // InitializeWaitForm();
                ExportToExcel.saveExcel(saveDialog, dataGridView1, recviz);
            }
        }




        public void getPayer()
        {
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"config.ini");
            recvizs = File.ReadAllLines(path, Encoding.Default);
            try
            {

                recviz.platNumber = Convert.ToInt32(string.IsNullOrEmpty(recvizs[0]) ? "0" : recvizs[0]);
                recviz.datePayment = string.IsNullOrEmpty(recvizs[3]) ? 0 : Convert.ToInt32(recvizs[3]);//??
                recviz.mfo = string.IsNullOrEmpty(recvizs[1]) ? "0" : recvizs[1];
                recviz.rahunok = string.IsNullOrEmpty(recvizs[2]) ? "0" : recvizs[2];
            }
            catch (Exception e)
            {
                MessageBox.Show("Помилка конвертації");
            }


        }
        public void setFields()
        {
            currentSheme.Text = recviz.name;
            platNumber.Text = recviz.platNumber.ToString();
            dateTimePicker1.Value = convertStrToTime(recviz.datePayment.ToString());//
            mfo.Text = recviz.mfo.ToString();
            rahunok.Text = recviz.rahunok;

            cliBankCode.Text = recviz.cliBankCode;
            recivPayNum.Text = recviz.recivPayNum;
            if (recviz.state == 1)
            {
                shemes = true;
            }
            else
            {
                shemes = false;
            }
            tableLayoutPanel7.RowStyles[1].Height = 100;
            tableLayoutPanel7.RowStyles[0].Height = 0;
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;
        }

        public void setFields2()
        {

            platNumber.Text = recviz.platNumber2.ToString();
            mfo.Text = recviz.edrpou.ToString();
            rahunok.Text = recviz.rahunok2;
            currentSheme.Text = recviz.name2;
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
        }

        private void PlatNumber_TextChanged(object sender, EventArgs e)
        {
            if (shemes)
            {
                recviz.platNumber = string.IsNullOrEmpty(platNumber.Text) ? 0 : Int64.Parse(platNumber.Text);
                numberDoc = recviz.platNumber;
            }
            else
            {
                recviz.platNumber2 = string.IsNullOrEmpty(platNumber.Text) ? 0 : Int64.Parse(platNumber.Text);
                numberDoc = recviz.platNumber;
            }

        }

        private void Mfo_TextChanged(object sender, EventArgs e)
        {

            if (shemes)
            {
                recviz.mfo = string.IsNullOrEmpty(mfo.Text) ? "0" : mfo.Text;
            }
            else
            {
                recviz.edrpou = string.IsNullOrEmpty(mfo.Text) ? "0" : mfo.Text;
            }


        }

        private void Rahunok_TextChanged(object sender, EventArgs e)
        {
            if (shemes)
            {
                recviz.rahunok = string.IsNullOrEmpty(rahunok.Text) ? "0" : rahunok.Text;
            }
            else
            {
                recviz.rahunok2 = string.IsNullOrEmpty(rahunok.Text) ? "0" : rahunok.Text;
            }

        }

        private void SaveFile_Click_1(object sender, EventArgs e)
        {
            if (!shemes)
            {
                saveExcel();
            }
            else
            {
                Save();
            }

        }

        private void УкрГазToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recviz.state = 2;
            texVisible(false);
            label2.Text = "ЕДРПОУ Платника:";
            //setFields2();
            setFieldsP2();
            


        }

        void texVisible(bool flag)
        {
            dateTimePicker1.Visible = label4.Visible = label5.Visible = cliBankCode.Visible = label6.Visible = recivPayNum.Visible = shemes = flag;
        }

        private void АвальToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recviz.state = 1;
            dateTimePicker1.Visible = label4.Visible = label5.Visible = cliBankCode.Visible = label6.Visible = recivPayNum.Visible =  shemes = true;
            label2.Text = "МФО Платника:";
            // setFields();
            
            setFieldsP();

            


        }
        public void isEdit(bool edit)
        {
            platNumber.Enabled = mfo.Enabled = rahunok.Enabled = dateTimePicker1.Enabled = currentSheme.Enabled = cliBankCode.Enabled = recivPayNum.Enabled =  edit;
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            edit = !edit;
            if (edit)
            {

                button1.Image = saveBtn;
                toolTip1.SetToolTip(button1, "Зберегти реквізити");
                button1.Text = "Зберегти шаблон";
                isEdit(true);

            }
            else
            {
                button1.Image = editBtn;
                toolTip1.SetToolTip(button1, "Редагувати реквізити");
                button1.Text = "Редагувати шаблон";
                if (shemes)
                {
                    WriteSettings(recviz, aval);
                }
                else
                {
                    WriteSettings(recviz, ukrBank);
                }
                
                isEdit(false);
            }
        }
        void WriteIni(Datashit recviz, Bank bank)
        {
            if (shemes)
            {
                bank.name =  recviz.name;
                bank.platNumber =  recviz.platNumber;
                bank.mfo = recviz.mfo;
                bank.rahunok = recviz.rahunok;
                bank.datePayment = recviz.datePayment;
                bank.cliBankCode = recviz.cliBankCode;
                bank.recivPayNum = recviz.recivPayNum;
                bank.state = recviz.state;
                
               
            }
            else
            {
                bank.name = recviz.name2;
                bank.platNumber = recviz.platNumber2;
                bank.rahunok =  recviz.rahunok2;
                bank.edrpou = recviz.edrpou;
                bank.state = recviz.state;
               
            }
           
            ////Bank 
            //myIni.Write("Bank", recviz.name);
            //myIni.Write("PlatNumber", recviz.platNumber.ToString());
            //myIni.Write("Mfo", recviz.mfo);
            //myIni.Write("Rahunok", recviz.rahunok);
            //myIni.Write("Paydate", recviz.datePayment.ToString());
            //myIni.Write("Bankclentnum", recviz.cliBankCode);
            //myIni.Write("PlatReciver", recviz.recivPayNum);
            ////Bank2
            //myIni.Write("Bank2", recviz.name2);
            //myIni.Write("Platnumber2", recviz.platNumber2.ToString());
            //myIni.Write("Edrpou", recviz.edrpou);
            //myIni.Write("Rahunok2", recviz.rahunok2);
            //myIni.Write("State", recviz.state.ToString());

        }
        void WriteSettings(Datashit recviz, Bank bank)
        {
            if (shemes)
            {
                Properties.Settings.Default.name = recviz.name;
                Properties.Settings.Default.platNumber = recviz.platNumber;
                Properties.Settings.Default.mfo = recviz.mfo;
                Properties.Settings.Default.rahunok = recviz.rahunok;
                Properties.Settings.Default.datePayment = recviz.datePayment;
                Properties.Settings.Default.clientBankCode = recviz.cliBankCode;
                Properties.Settings.Default.recivePayNum = recviz.recivPayNum;
                Properties.Settings.Default.state1 = recviz.state;
                Properties.Settings.Default.Save();
                //Xml.editXml(xmlConfig, bank);
            }
            else
            {
                Properties.Settings.Default.name2 = recviz.name2;
                Properties.Settings.Default.platNumber2 = recviz.platNumber2;
                Properties.Settings.Default.rahunok2 = recviz.rahunok2;
                Properties.Settings.Default.edrpou = recviz.edrpou;
                Properties.Settings.Default.state1 = recviz.state;
                //Xml.CreteConfig(xmlConfig, bank);
                Properties.Settings.Default.Save();
            }

            

        }
        void WriteIni()
        {
            try
            {
                ////Bank 
                //myIni.Write("Bank", "");
                //myIni.Write("PlatNumber", "2");
                //myIni.Write("Mfo", "00000");
                //myIni.Write("Rahunok", "1111");
                //myIni.Write("Paydate", "20190512");
                //myIni.Write("Bankclentnum", "");
                //myIni.Write("PlatReciver", "");
                ////Bank2
                //myIni.Write("Bank2", "");
                //myIni.Write("Platnumber2", "");
                //myIni.Write("Edrpou", "");
                //myIni.Write("Rahunok2", "");
                //myIni.Write("State", "1");
            }
            catch (System.NullReferenceException e)
            {

            }


        }
        private void CliBankCode_TextChanged(object sender, EventArgs e)
        {
            recviz.cliBankCode = cliBankCode.Text;
        }

        private void RecivPayNum_TextChanged(object sender, EventArgs e)
        {
            recviz.recivPayNum = recivPayNum.Text;
        }

        private void CurrentSheme_TextChanged(object sender, EventArgs e)
        {
            if (shemes)
            {
                recviz.name = currentSheme.Text;
            }
            else
            {
                recviz.name2 = currentSheme.Text;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           // WriteIni(recviz);
            if (shemes)
            {
                Properties.Settings.Default.state1 = 2;
                Properties.Settings.Default.Save();
                //myIni.Write("PlatNumber", numberDoc.ToString());
            }
            else
            {
                Properties.Settings.Default.state1 = 1;
                Properties.Settings.Default.Save();
               // myIni.Write("PlatNumber2", numberDoc.ToString());
            }

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form frm = new Form2();
            frm.ShowDialog();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            comboEdr.Items.Remove(comboEdr.SelectedItem);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            comboEdr.Items.Add(comboEdr.Text);
        }

        private void Label6_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tableLayoutPanel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboEdr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboEdr.Text.Equals(Properties.Settings.Default.name2))
            {
                Properties.Settings.Default.state = 2;
            }
            else
            {
                Properties.Settings.Default.state = 3;
            }
        }
    }
}