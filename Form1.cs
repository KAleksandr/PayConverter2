using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.Globalization;


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
        bool editAval = false;
        bool editUkrG = false;
        Image editBtn = Properties.Resources.edit_property_16px;//
        Image saveBtn = Properties.Resources.save_as_16px;
        private string[] data = { "11", "22", "33", "44", "55", "66", "77", "88", "99" };
       
        private long numberDoc;
        private long numberDocAval;
        private long numberDocUkrg;
        private string P = "·";
       
      
        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data.xml");
        public Form1()
        {

            InitializeComponent();
           
            Xml.loadXml(dataGridView3,path2);
            comboEdr.Items.Add(Properties.Settings.Default.name);
            comboEdr2.Items.Add(Properties.Settings.Default.name2);
            comboEdr2.Items.Add(Properties.Settings.Default.name3);
            numberDocAval = Properties.Settings.Default.platNumber;
            comboEdr.Text = Properties.Settings.Default.name;
            if (Properties.Settings.Default.state == 2)
            {
                numberDocUkrg = Properties.Settings.Default.platNumber2;
                comboEdr2.Text = Properties.Settings.Default.name2;
            }
                

            else
            {
                numberDocUkrg = Properties.Settings.Default.platNumber3;
                comboEdr2.Text = Properties.Settings.Default.name3;
            }

            //comboBox1.Items.Insert(1, "Боливия");


            if (shemes = Properties.Settings.Default.state1 == 1 ? true : false)
                {
                   //setFields();
                    setFieldsP();
                   
                   

                }
                else
                {
                    setFieldsP2();
                    //setFields2();
                   

                }

                isEditAval(editAval);
                isEditUkrG(editUkrG);

        }

        public void setFieldsP()
        {
           
            platNumber.Text = Properties.Settings.Default.platNumber.ToString();
            dateTimePicker1.Value = convertStrToTime(Properties.Settings.Default.datePayment.ToString());//
            mfo.Text = Properties.Settings.Default.mfo.ToString();
            rahunok.Text = Properties.Settings.Default.rahunok;

            cliBankCode.Text = Properties.Settings.Default.clientBankCode;
            
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
           
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            
        }

        

        

        private void OpenFile_Click(object sender, EventArgs e)
        {
            // openFiles();
            openCsv();
            dataGridView2.Sort(dataGridView2.Columns[11], ListSortDirection.Ascending);
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
                    CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                    n  = dataGridView2.Rows.Add();
                    dataGridView2.Rows[n].Cells[0].Value = "0";
                    dataGridView2.Rows[n].Cells[1].Value = "1";
                    dataGridView2.Rows[n].Cells[2].Value = numberDocAval++; 
                    dataGridView2.Rows[n].Cells[3].Value = CSV_Struct[i].dateP.ToString("dd.MM.yyyy");
                    dateTimePicker1.Value = DateTime.Parse(CSV_Struct[i].dateP.ToString("dd.MM.yyyy"), MyCultureInfo);
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
                            + P + "0" + P + arText[0] + P + "recviz" + P + date1 + P + P + P + P + arText[4] + P + P + Environment.NewLine;
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
            catch (Exception)
            {
                MessageBox.Show("Помилка конвертації");
            }


        }
        public void setFields()
        {
            
            platNumber.Text = recviz.platNumber.ToString();
            dateTimePicker1.Value = convertStrToTime(recviz.datePayment.ToString());//
            mfo.Text = recviz.mfo.ToString();
            rahunok.Text = recviz.rahunok;

            cliBankCode.Text = recviz.cliBankCode;
            
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
            
            tableLayoutPanel7.RowStyles[0].Height = 100;
            tableLayoutPanel7.RowStyles[1].Height = 0;
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
        }

        private void PlatNumber_TextChanged(object sender, EventArgs e)
        {
            numberDocAval = Properties.Settings.Default.platNumber = string.IsNullOrEmpty(platNumber.Text) ? 0 : Int64.Parse(platNumber.Text);
            
        }

        private void Mfo_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.mfo = string.IsNullOrEmpty(mfo.Text) ? "0" : mfo.Text;
            
        }

        private void Rahunok_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.rahunok = string.IsNullOrEmpty(rahunok.Text) ? "0" : rahunok.Text;
            
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
            
            label2.Text = "ЕДРПОУ Платника:";
            //setFields2();
            setFieldsP2();
            
        }

        //void texVisible(bool flag)
        //{
        //    dateTimePicker1.Visible = label4.Visible = label5.Visible = cliBankCode.Visible =   shemes = flag;
        //}

        private void АвальToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recviz.state = 1;
            //dateTimePicker1.Visible = label4.Visible = label5.Visible = cliBankCode.Visible =   shemes = true;
            //label2.Text = "МФО Платника:";
            // setFields();
            
            setFieldsP();
            
        }
        public void isEditAval(bool edit)
        {
            cliBankCode.Visible = platNumber.Visible = mfo.Visible = rahunok.Visible =  label1.Visible = label2.Visible = label3.Visible =   label5.Visible =   edit;
        }
        public void isEditUkrG(bool edit)
        {
            textBox2.Visible =  textBox1.Visible = label7.Visible = label6.Visible =  edit;
        }


        //private void Button1_Click(object sender, EventArgs e)
        //{
        //    edit = !edit;
        //    if (edit)
        //    {


        //        isEdit(true);

        //    }
        //    else
        //    {
        //        //button1.Image = editBtn;
        //        //toolTip1.SetToolTip(button1, "Редагувати реквізити");
        //        //button1.Text = "Редагувати шаблон";
        //        if (shemes)
        //        {
        //            WriteSettings(recviz, aval);
        //        }
        //        else
        //        {
        //            WriteSettings(recviz, ukrBank);
        //        }

        //        isEdit(false);
        //    }
        //}
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
                //myIni.Write("PlatNumber", numberDoc.ToString());
            }
            else
            {
                Properties.Settings.Default.state1 = 1;
                Properties.Settings.Default.Save();
               // myIni.Write("PlatNumber2", numberDoc.ToString());
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

       

       

        private void comboEdr_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            editUkrG = !editUkrG;
            if (editUkrG)
            {
                isEditUkrG(editUkrG);
                button5.Image = saveBtn;
            }
            else
            {
                button5.Image = editBtn;
                isEditUkrG(editUkrG);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.datePayment = converterDateToInt(dateTimePicker1.Value);
            Properties.Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.state == 2)
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.state == 2)
            {
                numberDocUkrg = Properties.Settings.Default.platNumber2 = string.IsNullOrEmpty(platNumber.Text) ? 0 : Int64.Parse(platNumber.Text);
                Properties.Settings.Default.Save();
            }
            else
            {
                numberDocUkrg = Properties.Settings.Default.platNumber3 = string.IsNullOrEmpty(platNumber.Text) ? 0 : Int64.Parse(platNumber.Text);
                Properties.Settings.Default.Save();
            }
            
        }


        private void button5_Click_2(object sender, EventArgs e)
        {
            editUkrG = !editUkrG;
            if (editUkrG)
            {
                if (Properties.Settings.Default.state == 2)
                {
                    textBox2.Text = Properties.Settings.Default.edrpou;
                    textBox1.Text = Properties.Settings.Default.platNumber2.ToString();

                }
                else
                {
                    textBox2.Text = Properties.Settings.Default.edrpou2;
                    textBox1.Text = Properties.Settings.Default.platNumber3.ToString();
                }

                isEditUkrG(editUkrG);
                button3.Image = saveBtn;
            }
            else
            {
                button3.Image = editBtn;
                isEditUkrG(editUkrG);
                if (Properties.Settings.Default.state == 2)
                {
                    Properties.Settings.Default.name2 = comboEdr2.Text;
                    Properties.Settings.Default.Save();
                    comboEdr2.Items.Clear();
                    comboEdr2.Items.Add(Properties.Settings.Default.name2);
                    comboEdr2.Items.Add(Properties.Settings.Default.name3);
                }
                else
                {
                    Properties.Settings.Default.name3 = comboEdr2.Text;
                    Properties.Settings.Default.Save();
                    comboEdr2.Items.Clear();
                    comboEdr2.Items.Add(Properties.Settings.Default.name2);
                    comboEdr2.Items.Add(Properties.Settings.Default.name3);
                }
            }
        }

        private void comboEdr2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboEdr.Text.Equals(Properties.Settings.Default.name))
            {
                Properties.Settings.Default.state = 2;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.state = 3;
                Properties.Settings.Default.Save();
            }
        }
    }
}