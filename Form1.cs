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
using System.Xml;
using DotNetDBF;
using Dynamitey.Internal.Optimization;
using static DotNetDBF.DBFSignature;


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
        private Bank industrial = new Bank();
        private Bank oschad = new Bank();

        private long numberDocAval;
        private string P = "·";

        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterData.xml");
        private string path3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AnotherPayConverterData.xml");
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
            PropertyInfo pi = typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic);
            if (pi != null)
            {
                pi.SetValue(c, value, null);
            }
        }

        public void initData()
        {
            initPData();
            Properties.Settings.Default.count++;
            Properties.Settings.Default.Save();
            backUpData();
        }

        public void initPData()
        {
            this.comboEdr.SelectedIndexChanged += new System.EventHandler(comboEdr_SelectedIndexChanged);
            if (!anotherPay.Checked)
            {
                Xml.isExistsFile(path2, strData);
                Xml.loadXml(dataGridView3, path2);
            }
            else
            {
                Xml.isExistsFile(path3, strData);
                Xml.loadXml(dataGridView3, path3);
            }

            Xml.isExistsFile(pathConfig, strConfig);
            try
            {
                Bank[] banks = Xml.ReadXml(pathConfig);
                aval = banks[0];
                ukrGaz = banks[1];
                industrial = banks[2];
                oschad = banks[3];
            }
            catch
            {
            }


            setFieldsP(aval);
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
        }

        private void comboEdr_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (comboEdr.SelectedIndex)
            {
                case (0):
                    setFieldsP(aval);
                    break;
                case (1):
                    setFieldsP(industrial);
                    break;
                case (2):
                    setFieldsP(oschad);
                    break;
            }
        }

        public void backUpData()
        {
            if (Properties.Settings.Default.count % 10 == 0)
            {
                string directory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}" +
                                   "\\PayConverterBackup";
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

        public void setFieldsP(Bank bank)
        {
            mfo.Text = bank.mfo;
            rahunok.Text = bank.rahunok;
            cliBankCode.Text = bank.clientBankCode;

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
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            this.path = string.Empty;
            // initPData();
            openCsv();
            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
        }

        private bool isNull = false;

        public void openCsv()
        {
            openFileDialog1.FileName = "file"; //
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show("row "+dataGridView1.Rows.Count);
                if (dataGridView1.Rows.Count > 0 || dataGridView2.Rows.Count > 0)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView2.Rows.Clear();
                    dataGridView3.Rows.Clear();

                    numberDocAval = 1;
                }

                path = name = openFileDialog1.FileName;
                loadFileRoot();

                if (isNull)
                {
                    if (!anotherPay.Checked)
                    {
                        Xml.saveXml(dataGridView3, path2);
                    }
                    else if (anotherPay.Checked)
                    {
                        Xml.saveXml(dataGridView3, path3);
                    }
                }
            }
        }

        public void loadFileRoot()
        {
            List<Bank> CSV_Struct = new List<Bank>();

            CSV_Struct = Bank.ReadFile(path, anotherPay.Checked);


            DateTime dt1 = DateTime.Today;
            for (int i = 0; i <= CSV_Struct.Count - 1; i++)
            {
                int n;
                if (CSV_Struct[i].id == 0)
                {
                    //todo: remove messagebox 
                    // MessageBox.Show("СТРУКТУРА АЙДИ 0");
                    n = dataGridView1.Rows.Add();

                    dataGridView1.Rows[n].Cells[0].Value = CSV_Struct[i].summa;
                    dataGridView1.Rows[n].Cells[1].Value = "UAH";
                    if (!anotherPay.Checked)
                    {
                        dataGridView1.Rows[n].Cells[2].Value = addDateToStr(
                            findZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                            (CSV_Struct[i].dateP == dt1
                                ? dateTimePicker1.Value.ToString("dd.MM.yyyy")
                                : CSV_Struct[i].dateP.ToString("dd.MM.yyyy")));

                        dataGridView1.Rows[n].Cells[8].Value =
                            findNameZkpo(CSV_Struct[i].name, CSV_Struct[i].edrpou, CSV_Struct[i].rahunok).Equals("null")
                                ? CSV_Struct[i].name
                                : findNameZkpo(CSV_Struct[i].name, CSV_Struct[i].edrpou, CSV_Struct[i].rahunok);
                    }
                    else
                    {
                        dataGridView1.Rows[n].Cells[2].Value = CSV_Struct[i].name;
                        dataGridView1.Rows[n].Cells[8].Value = CSV_Struct[i].pruznach;
                    }

                    //dataGridView1.Rows[n].Cells[2].Value = addDateToStr(findZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                    //(CSV_Struct[i].dateP == dt1 ? dateTimePicker1.Value.ToString("dd.MM.yyyy") : CSV_Struct[i].dateP.ToString("dd.MM.yyyy")));

                    if (dataGridView1.Rows[n].Cells[2].Value.Equals("null") || anotherPay.Checked &&
                        dataGridView1.Rows[n].Cells[2].Value.ToString() != "null")
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

                    dataGridView1.Rows[n].Cells[9].Value = ukrGaz.iban;
                }

                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                //comboEdr.SelectedIndex
                if (CSV_Struct[i].id == 1)
                {
                    //todo: remove messagebox 
                    // MessageBox.Show("СТРУКТУРА АЙДИ 1");  
                    try
                    {
                        dateTimePicker1.Value =
                            DateTime.Parse(CSV_Struct[i].dateP.ToString("dd.MM.yyyy"), MyCultureInfo);
                        n = dataGridView2.Rows.Add();
                        dataGridView2.Rows[n].Cells[0].Value = "0";
                        dataGridView2.Rows[n].Cells[1].Value = "1";
                        dataGridView2.Rows[n].Cells[2].Value = numberDocAval++;
                        dataGridView2.Rows[n].Cells[3].Value = DateTime.Today.ToString("dd.MM.yyyy");
                        dataGridView2.Rows[n].Cells[4].Value = comboEdr.SelectedIndex == 1 ? industrial.mfo : comboEdr.SelectedIndex == 2 ? oschad.mfo : aval.mfo;
                        dataGridView2.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                        dataGridView2.Rows[n].Cells[6].Value =
                            comboEdr.SelectedIndex == 1 ? industrial.rahunok : comboEdr.SelectedIndex == 2 ? oschad.rahunok : aval.rahunok;
                        dataGridView2.Rows[n].Cells[7].Value = CSV_Struct[i].rahunok;
                        dataGridView2.Rows[n].Cells[8].Value = CSV_Struct[i].summa;
                        dataGridView2.Rows[n].Cells[9].Value = "0";
                        dataGridView2.Rows[n].Cells[12].Value = CSV_Struct[i].edrpou;

                        if (!anotherPay.Checked && comboEdr.SelectedIndex.ToString() == "0")
                        {
                            dataGridView2.Rows[n].Cells[10].Value =
                                findNameZkpo(CSV_Struct[i].name, CSV_Struct[i].edrpou, CSV_Struct[i].rahunok)
                                    .Equals("null")
                                    ? CSV_Struct[i].name
                                    : findNameZkpo(CSV_Struct[i].name, CSV_Struct[i].edrpou, CSV_Struct[i].rahunok);
                            dataGridView2.Rows[n].Cells[11].Value = addDateToStr(
                                findZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                                CSV_Struct[i].dateP.ToString("dd.MM.yyyy"));
                        }
                        else //todo: пофиксить сохранение базы индустриала
                        {
                            dataGridView2.Rows[n].Cells[10].Value = CSV_Struct[i].name;
                            dataGridView2.Rows[n].Cells[11].Value = CSV_Struct[i].pruznach;
                        }

                        //dataGridView2.Rows[n].Cells[11].Value = addDateToStr(findZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                        // CSV_Struct[i].dateP.ToString("dd.MM.yyyy"));
                        //if (dataGridView2.Rows[n].Cells[11].Value.Equals("null") || !anotherPay.Checked && dataGridView2.Rows[n].Cells[11].Value.ToString() != "null")
                        if (dataGridView2.Rows[n].Cells[11].Value.Equals("null") || anotherPay.Checked &&
                            dataGridView2.Rows[n].Cells[11].Value.ToString() != "null")
                        {
                            dataGridView2.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                            int m = dataGridView3.Rows.Add();
                            dataGridView3.Rows[m].Cells[0].Value = CSV_Struct[i].name;
                            dataGridView3.Rows[m].Cells[1].Value = CSV_Struct[i].edrpou;
                            dataGridView3.Rows[m].Cells[2].Value = CSV_Struct[i].rahunok;
                            // dataGridView3.Rows[m].Cells[3].Value = CSV_Struct[i].pruznach;
                            dataGridView3.Rows[m].Cells[3].Value = dataGridView2.Rows[n].Cells[11].Value;
                            isNull = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (isNull)
            {
                if (!anotherPay.Checked)
                {
                    Xml.saveXml(dataGridView3, path2);
                }
                else if (anotherPay.Checked)
                {
                    Xml.saveXml(dataGridView3, path3);
                }
            }
        }

        public void autoOpenCsv(string path)
        {
            isNull = false;
            if (dataGridView1.Rows.Count > 0 || dataGridView2.Rows.Count > 0)
            {
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
            }

            dataGridView3.Rows.Clear();
            Xml.loadXml(dataGridView3, path);
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

        public string findNameZkpo(string name, string zkpo, string rrahunok)
        {
            foreach (DataGridViewRow r in dataGridView3.Rows)
            {
                if (r.Cells != null)
                {
                    try
                    {
                        if (r.Cells[0].Value.Equals(name) && r.Cells[1].Value.Equals(zkpo) &&
                            r.Cells[2].Value.Equals(rrahunok))
                        {
                            //MessageBox.Show(" " + name  + " summa " + r.Cells[0].Value);
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
                if (comboEdr.SelectedIndex != 2)
                {
                    string k = "" + DateTime.Now;
                    name = saveFileDialog1.FileName;

                    createBox();
                    string texts = textImport.Text.Replace("і", "i").Replace("І", "I");
                    File.WriteAllText(name, texts, Encoding.GetEncoding(866));
                }
            }
        }

        public void createBox()
        {
            foreach (DataGridViewRow r in dataGridView2.Rows) // пока в dataGridView2 есть строки
            {
                if (r.Cells != null)
                {
                    string t = "";
                    string sum = "";
                    try
                    {
                        t = r.Cells[3].Value.ToString();
                        //string repl = r.Cells[8].Value.ToString().Replace(",", "");
                        // sum = repl.ToString().Replace(".", "");
                        sum = r.Cells[8].Value.ToString().Replace(",", "").Replace(".", "");
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
            name += dateTimePicker1.Value.Day.ToString().Length == 1
                ? "0" + dateTimePicker1.Value.Day
                : dateTimePicker1.Value.Day.ToString();
            name += DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour : DateTime.Now.Hour.ToString();
            name += DateTime.Now.Minute + bcode + ".";
            return name;
        }


        public void saveOschadDbf()
        {
            
            // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string path_dbf = Directory.GetCurrentDirectory().ToString() + "\\ОщадБанк\\";

            if (!Directory.Exists(path_dbf))
                Directory.CreateDirectory(path_dbf);


            string dateTime = DateTime.Now.ToString("dd/MM/yy");


            using (Stream fos = File.Open($"{path_dbf}" + $"{dateTime}" + ".dbf", FileMode.OpenOrCreate,
                FileAccess.ReadWrite))
            using (var writer = new DBFWriter())
            {
                writer.CharEncoding = Encoding.GetEncoding(866);
                writer.Signature = DBFSignature.DBase3WithMemo;
                writer.LanguageDriver = 0x26; // кодировка 866
                var field1 = new DBFField("ndoc", NativeDbType.Char, 10); //номер документа
                var field2 = new DBFField("dt", NativeDbType.Date); //дата документа
                var field3 = new DBFField("mfocli", NativeDbType.Char, 12); //МФО клієнта    302076
                var field4 = new DBFField("okpocli", NativeDbType.Char, 14); //ЗКПО клієнта    40375721
                var field5 = new DBFField("acccli", NativeDbType.Char, 29); //рахунок клієнта   UA243020760000026501300388426
                var field6 = new DBFField("namecli", NativeDbType.Char, 38); //ім’я клієнта     ТОВ "ФК"МПС"
                var field7 = new DBFField("bankcli", NativeDbType.Char, 254); //назва банку клієнта    Вінницьке обласне управління АТ "Ощадбанк"
                var field8 = new DBFField("mfocor", NativeDbType.Char, 12); //МФО кореспондента
                var field9 = new DBFField("acccor", NativeDbType.Char, 29); //рахунок кореспондента
                var field10 = new DBFField("okpocor", NativeDbType.Char, 14); //ЗКПО кореспондента
                var field11 = new DBFField("namecor", NativeDbType.Char, 38); //ім’я кореспондента
                var field12 = new DBFField("bankcor", NativeDbType.Char, 254); //назва банку кореспондента
                var field13 = new DBFField("dk", NativeDbType.Numeric, 1); //ознака «дебет – 1; кредит – 0;»
                var field14 = new DBFField("summa", NativeDbType.Numeric, 20); //сума платежу «в копійках»
                var field15 = new DBFField("nazn", NativeDbType.Char, 160); //призначення платежу
                var field16 = new DBFField("val", NativeDbType.Numeric, 4); //код валюти платежу
                // var field17 = new DBFField("tp", NativeDbType.Char, 4); //час проведення платежу в банку
                // var field18 = new DBFField("dtpro", NativeDbType.Char, 8); //дата проведення платежу банком


                writer.Fields = new[]
                {
                    field1, field2, field3, field4, field5, field6, field7, field8, field9, field10, field11, field12,
                    field13, field14, field15, field16
                };
                
                int docNum = 1;
                string zkpo = "40375721";
                string cliName = "ТОВ \"ФК\"МПС\"";
                string cliBankName = "Вінницьке обласне управління АТ \"Ощадбанк\"";
                int debCred = 1;
                string bankKorespond = "";
                int codeVal = 980;
                
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {

                    int summa = Convert.ToInt32(row.Cells[5].Value);
                    writer.AddRecord(
                        // добавляем поля в набор
                        docNum.ToString(),  //1
                        DateTime.Now,                   //2
                        row.Cells[4].Value,             //3
                        zkpo,                           //4
                        row.Cells[6].Value,             //5
                        cliName,                        //6
                        cliBankName,                    //7
                        row.Cells[5].Value.ToString(),  //8
                        row.Cells[7].Value.ToString(),  //9
                        row.Cells[12].Value.ToString(),  //10
                        row.Cells[10].Value.ToString(),  //11
                        bankKorespond,                  //12
                        debCred,                        //13
                        summa *100,                     //14
                        row.Cells[11].Value,             //15
                        codeVal                         //16
                    );
                    docNum++;
                }
                writer.Write(fos);
            }
        
        }

        public void saveXml()
        {
            string time = DateTime.Now.ToString("ddMMyyyy");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.GetEncoding(1251);
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path + "//AlfaBankPays"))
            {
                Directory.CreateDirectory(path + "//AlfaBankPays");
            }

            XmlWriter xmlWriter = XmlWriter.Create(path + "//AlfaBankPays//" + "AlfaBankPay" + time + ".xml", settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("ROWDATA");
            foreach (DataGridViewRow r in dataGridView2.Rows) // пока в dataGridView2 есть строки
            {
                if (r.Cells != null)
                {
                    string t = "";
                    string sum = "";
                    try
                    {
                        t = r.Cells[3].Value.ToString();
                        sum = r.Cells[8].Value.ToString().Replace(",", "").Replace(".", "");
                    }
                    catch
                    {
                    }

                    xmlWriter.WriteStartElement("ROW");
                    xmlWriter.WriteAttributeString("DOCUMENTDATE", converterDate(t));
                    xmlWriter.WriteAttributeString("BANKID", "300346");
                    xmlWriter.WriteAttributeString("IBAN", "UA633003460000026507069842401");
                    xmlWriter.WriteAttributeString("CORRBANKID", r.Cells[5].Value.ToString());
                    xmlWriter.WriteAttributeString("CORRIBAN", r.Cells[7].Value.ToString());
                    xmlWriter.WriteAttributeString("AMOUNT", sum);
                    xmlWriter.WriteAttributeString("CORRSNAME", r.Cells[10].Value.ToString());
                    xmlWriter.WriteAttributeString("DETAILSOFPAYMENT", r.Cells[11].Value.ToString());
                    xmlWriter.WriteAttributeString("CORRIDENTIFYCODE", r.Cells[12].Value.ToString());
                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
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
                worksheet.Rows.NumberFormatLocal = "@";
                worksheet.Columns.NumberFormatLocal = "@";
                worksheet.Name = "ExportedFromDatGrid";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;

                // for (int i = 0; i <= dataGridView1.Rows.Count; i++) // todo: Для нового укргазбанку вигрузка
                // {
                //     
                //         if (cellRowIndex == 1)
                //         {
                //            // worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Columns[j].HeaderText;
                //             // worksheet.Cells[1, 1].Value = "Campus";
                //             worksheet.Cells[1, 1].Value = "FIELD_CUST_BANK_CODE";
                //             worksheet.Cells[1, 2].Value = "FIELD_CUST_ACCOUNT";
                //             worksheet.Cells[1, 3].Value = "FIELD_CUST_IBAN";
                //             worksheet.Cells[1, 4].Value = "FIELD_BENEF_BANK_CODE";
                //             worksheet.Cells[1, 5].Value = "FIELD_BENEF_ACCOUNT";
                //             worksheet.Cells[1, 6].Value = "FIELD_BENEF_IBAN";
                //             worksheet.Cells[1, 7].Value = "FIELD_OPERATION_TYPE";
                //             worksheet.Cells[1, 8].Value = "FIELD_AMOUNT";
                //             worksheet.Cells[1, 9].Value = "FIELD_DOCUMENT_TYPE";
                //             worksheet.Cells[1, 10].Value = "FIELD_NUMBER";
                //             worksheet.Cells[1, 11].Value = "FIELD_CURRENCY_NUMBER";
                //             worksheet.Cells[1, 12].Value = "FIELD_DOCUMENT_DATE";
                //             worksheet.Cells[1, 13].Value = "FIELD_VALUE_DATE";
                //             worksheet.Cells[1, 14].Value = "FIELD_CUST_NAME";
                //             worksheet.Cells[1, 15].Value = "FIELD_BENEF_NAME";
                //             worksheet.Cells[1, 16].Value = "FIELD_PURPOSE";
                //             worksheet.Cells[1, 17].Value = "FIELD_ADDITIONAL_DATA";
                //             worksheet.Cells[1, 18].Value = "FIELD_PURPOSE_CODE";
                //             worksheet.Cells[1, 19].Value = "FIELD_EMPTY_COLUMN";
                //             worksheet.Cells[1, 20].Value = "FIELD_CUST_TAX_CODE";
                //             worksheet.Cells[1, 21].Value = "FIELD_BENEF_TAX_CODE";
                //             worksheet.Cells[1, 22].Value = "FIELD_EXT_DOCUMENT_NUMBER";
                //             worksheet.Cells[1, 23].Value = "FIELD_VAT_TYPE";
                //         }
                //         else
                //         {
                //             // for (int t = 1; t < dataGridView1.Columns.Count; t++)
                //             // {
                //             //     worksheet.Cells[cellRowIndex, t].NumberFormat = "@";
                //             // }
                //
                //             worksheet.Cells[cellRowIndex, 8] = dataGridView1.Rows[i-1].Cells[0].Value.ToString();
                //             worksheet.Cells[cellRowIndex, 11] = dataGridView1.Rows[i-1].Cells[1].Value.ToString();
                //             worksheet.Cells[cellRowIndex, 16] = dataGridView1.Rows[i-1].Cells[2].Value.ToString();
                //             worksheet.Cells[cellRowIndex, 2] = dataGridView1.Rows[i-1].Cells[3].Value.ToString();
                //             worksheet.Cells[cellRowIndex, 20] = dataGridView1.Rows[i-1].Cells[4].Value.ToString();
                //             worksheet.Cells[cellRowIndex, 4] = dataGridView1.Rows[i-1].Cells[5].Value.ToString();
                //             worksheet.Cells[cellRowIndex, 6] = dataGridView1.Rows[i-1].Cells[6].Value.ToString();
                //             worksheet.Cells[cellRowIndex, 15] = dataGridView1.Rows[i-1].Cells[7].Value.ToString();
                //             worksheet.Cells[cellRowIndex, 3] = dataGridView1.Rows[i-1].Cells[8].Value.ToString();
                //         }
                //
                //         
                //     
                //     cellRowIndex++;
                //     progressBar1.PerformStep();
                // } 

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
                            worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
                            worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                dataGridView1.Rows[i - 1].Cells[j].Value.ToString();
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
            if (comboEdr.SelectedIndex == 0)
            {
                saveExcel();
                saveXml();
            }
            if (comboEdr.SelectedIndex == 2)
            {
                saveOschadDbf();
            }

            Save();
        }


        public void isEditAval(bool edit)
        {
            cliBankCode.Visible = rahunok.Visible = mfo.Visible =
                rahunok.Visible = label1.Visible = label2.Visible = label5.Visible = edit;
        }

        public void isEditUkrG(bool edit)
        {
            textBox2.Visible = label6.Visible =
                label10.Visible = textBox4.Visible = label3.Visible = textIban.Visible = edit;
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
            Form frm;
            if (anotherPay.Checked)
            {
                frm = new Form2(path3);
            }
            else
            {
                frm = new Form2(path2);
            }

            dr = frm.ShowDialog();

            if (dr == DialogResult.OK)
            {
                if (anotherPay.Checked)
                {
                    autoOpenCsv(path3);
                }
                else
                {
                    autoOpenCsv(path2);
                }
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
                if (comboEdr.SelectedIndex == 0)
                {
                    // MessageBox.Show(""+comboEdr.SelectedItem);
                    button3.Image = editBtn;
                    isEditAval(editAval);
                    aval.name = comboEdr.Text;
                    comboEdr.Items.Clear();
                    comboEdr.Items.Add("Райффайзен Банк Аваль");
                    comboEdr.Items.Add("Індустріал");
                    comboEdr.Items.Add("Ощадбанк");
                    aval.name = comboEdr.Text;
                    aval.mfo = mfo.Text;
                    aval.rahunok = rahunok.Text;
                    aval.clientBankCode = cliBankCode.Text;
                    aval.id = 0;
                    Xml.EditXml(aval, pathConfig);
                    comboEdr.Enabled = true;
                    initData();
                }
                else if (comboEdr.SelectedIndex == 1)
                {
                    //  MessageBox.Show(""+comboEdr.SelectedItem);
                    button3.Image = editBtn;
                    isEditAval(editAval);
                    aval.name = comboEdr.Text;
                    comboEdr.Items.Clear();
                    comboEdr.Items.Add("Райффайзен Банк Аваль");
                    comboEdr.Items.Add("Індустріал");
                    comboEdr.Items.Add("Ощадбанк");
                    aval.name = comboEdr.Text;
                    aval.mfo = mfo.Text;
                    aval.rahunok = rahunok.Text;
                    aval.clientBankCode = cliBankCode.Text;
                    aval.id = 2;
                    Xml.EditXml(aval, pathConfig);
                    comboEdr.Enabled = true;
                    initData();
                }
                else if (comboEdr.SelectedIndex == 2)
                {
                    //  MessageBox.Show(""+comboEdr.SelectedItem);
                    button3.Image = editBtn;
                    isEditAval(editAval);
                    aval.name = comboEdr.Text;
                    comboEdr.Items.Clear();
                    comboEdr.Items.Add("Райффайзен Банк Аваль");
                    comboEdr.Items.Add("Індустріал");
                    comboEdr.Items.Add("Ощадбанк");
                    aval.name = comboEdr.Text;
                    aval.mfo = mfo.Text;
                    aval.rahunok = rahunok.Text;
                    aval.clientBankCode = cliBankCode.Text;
                    aval.id = 3;
                    Xml.EditXml(aval, pathConfig);
                    comboEdr.Enabled = true;
                    initData();
                }
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
                    dataGridView2.CurrentRow.Cells[11].Value =
                        dataGridView2.CurrentRow.Cells[11].Value.ToString().Replace("  ", @" ");
                    //if (!currentCellValue.Equals(dataGridView2.CurrentRow.Cells[11].Value.ToString()))
                    //{
                    //    DialogResult dialogResult = MessageBox.Show("Зміни записати базу данних", "Запис данних",
                    //        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //    if (dialogResult == DialogResult.Yes)
                    //    {
                    //        int n = dataGridView1.Rows.Add();
                    //        dataGridView3.Rows[n].Cells[0].Value =
                    //            dataGridView2.Rows[selRowNum].Cells[selColNum - 1].Value; // 
                    //        dataGridView3.Rows[n].Cells[1].Value =
                    //            dataGridView2.Rows[selRowNum].Cells[selColNum + 1].Value; // 
                    //        dataGridView3.Rows[n].Cells[2].Value =
                    //            dataGridView2.Rows[selRowNum].Cells[selColNum].Value; // 
                    //        Xml.saveXml(dataGridView3, path2);
                    //    }
                    //}
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
                    //if (!currentCellValue.Equals(dataGridView1.CurrentRow.Cells[2].Value.ToString()))
                    //{
                    //    DialogResult dialogResult = MessageBox.Show("Зміни записати базу данних", "Запис данних",
                    //        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //    if (dialogResult == DialogResult.Yes)
                    //    {
                    //        string pattern = @"за\s[0-9]{2}[.][0-9]{2}[.][0-9]{4}р\.";
                    //        string str = "";
                    //        int n = dataGridView3.Rows.Add();
                    //        dataGridView3.Rows[n].Cells[0].Value =
                    //            dataGridView1.Rows[selRowNum].Cells[selColNum + 6].Value; // 
                    //        dataGridView3.Rows[n].Cells[1].Value =
                    //            dataGridView1.Rows[selRowNum].Cells[selColNum + 5].Value; // 
                    //        try
                    //        {
                    //            str = dataGridView1.Rows[selRowNum].Cells[selColNum].Value.ToString();
                    //        }
                    //        catch (NullReferenceException) { }
                    //        string newLine = Regex.Replace(str, pattern, "  за ##.##.#### ");
                    //        dataGridView3.Rows[n].Cells[2].Value = newLine;
                    //        ; // 
                    //        Xml.saveXml(dataGridView3, path2);
                    //    }
                    //}
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

            Rectangle headerBounds =
                new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
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

            Rectangle headerBounds =
                new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.Visible)
            {
                int[] col = {2, 6, 7, 8};
                MyDataGrid.Filter(dataGridView1, textBox1.Text, col);
            }
            else
            {
                int[] col = {7, 10, 11, 12};
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
            this.Text += " " + localVersion;
            new Update().Download();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void gridHeader_Click(object sender, EventArgs e)
        {
        }

        private void anotherPay_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            this.path = string.Empty;
            initPData();
        }

        private void anotherPay_CheckStateChanged(object sender, EventArgs e)
        {
        }
    }
}