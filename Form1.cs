using DotNetDBF;
using dBASE.NET;
using Microsoft.Office.Interop.Excel;
using SoftGenConverter.Entity;
using SoftGenConverter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static DotNetDBF.DBFSignature;
using Application = System.Windows.Forms.Application;
using Rectangle = System.Drawing.Rectangle;
using TextBox = System.Windows.Forms.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;


namespace SoftGenConverter
{
    public partial class Form1 : Form
    {
        private Bank aval = new Bank();
        private string currentCellValue = "";
        private bool editAval;
        private Image editBtn = Resources.form1Edit;
        private bool editUkrG;

        private Bank industrial = new Bank();

        private bool isNull;
        private Version localVersion = new Version(Application.ProductVersion);
        private string name;

        private long numberDocAval;

        private Bank oschad = new Bank();
        private Bank pumb = new Bank();
        private string P = "·";
        private string path = "";

        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterData.xml");
        private string path3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AnotherPayConverterData.xml");
        private string pathConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterConfig.xml");
        private Image saveBtn = Resources.form1EndEdit;
        private string strConfig = Resources.PayConverterConfig;
        private string strData = Resources.PayConverterData;
        private TextBox textImport = new TextBox();
        private Bank ukrGaz = new Bank();
        private Bank ukrGaz2 = new Bank();


        public Form1()
        {
            InitializeComponent();
            // Bank[] banks = Xml.ReadXml(pathConfig);
            //MessageBox.Show(banks[0].ToString());
            erdpo1l.Visible = erdpo1.Visible = false;
            InitData();

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

        public void InitData()
        {
            InitPData();
            Settings.Default.count++;
            Settings.Default.Save();
            BackUpData();
        }

        public void InitPData()
        {
            if (!File.Exists(Db.runningPath))
            {
                //MessageBox.Show("Ok");
                //Thread.Sleep(300);
                MessageBox.Show(
                    Db.runningPath + " файл не знайдений!" + Environment.NewLine + " Файл створено з конфігурації програми.",
                    "Помилка.", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Db.CreateDb();

            }
            else
            {
                Db.CreateNewTablePurposeOfPayment();
            }
            comboEdr.SelectedIndexChanged += ComboEdr_SelectedIndexChanged;
            if (!anotherPay.Checked)
            {
                //Xml.isExistsFile(path2, strData);
                //Xml.loadXml(dataGridView3, path2);
                // ImportData(dataGridView3, "PayConverterData");
                var dell = AnotherPay_.DeleteDublicate("PayConverterData");
                dataGridView3.DataSource = Db.SelectTable<AnotherPay>("PayConverterData");
            }
            else
            {
                //Xml.isExistsFile(path3, strData);
                // Xml.loadXml(dataGridView3, path3);
                // ImportData(dataGridView3, "AnotherPayConverterData");

                var dell = AnotherPay_.DeleteDublicate("AnotherPayConverterData");
                dataGridView3.DataSource = Db.SelectTable<AnotherPay>("AnotherPayConverterData");



            }


            //Xml.isExistsFile(pathConfig, strConfig);


            try
            {
                var configB = Db.SelectTable<PayConverterConfig>("PayConverterConfig");
                // Bank[] banks = Xml.ReadXml(pathConfig);

                aval = new Bank(configB.Where(b => b.bankid == 0).First());// banks[0]; 
                ukrGaz = new Bank(configB.Where(b => b.bankid == 1).First());//banks[1];
                industrial = new Bank(configB.Where(b => b.bankid == 2).First());//banks[2];
                oschad = new Bank(configB.Where(b => b.bankid == 3).First());//banks[3];
                pumb = new Bank(configB.Where(b => b.bankid == 4).First());//banks[4];
                ukrGaz2 = new Bank(configB.Where(b => b.bankid == 1).First());//banks[5];

            }
            catch
            {
                // ignored
            }


            // SetFieldsP(aval);
            SetFieldsP2();
            //comboEdr.Items.Add(aval.name);
            // comboEdr.Text = aval.name;

            IsEditAval(editAval);
            IsEditUkrG(editUkrG);
            MyDataGrid.StyleDataGridView(dataGridView1, false);
            MyDataGrid.StyleDataGridView(dataGridView2, false);

            docNumOschadL.Visible = docNumOschad.Visible = false;
            docNumOschad.Text = "1";
            comboEdr2.SelectedIndex = 0;

            if (comboEdr.SelectedItem == null)
            {
                comboEdr.SelectedIndex = 0;
            }
            else
            {
                switch (comboEdr.SelectedIndex)
                {
                    case 0:
                        SetFieldsP(aval);
                        break;
                    case 1:
                        SetFieldsP(industrial);
                        break;
                    case 2:
                        SetFieldsP(oschad);
                        docNumOschadL.Visible = docNumOschad.Visible = false;
                        break;
                    case 3:
                        SetFieldsP(pumb);
                        docNumOschadL.Visible = docNumOschad.Visible = true;
                        break;
                    case 4:
                        SetFieldsP(ukrGaz2);
                        docNumOschadL.Visible = docNumOschad.Visible = false;
                        break;
                }
            }

            SetDoubleBuffered(dataGridView1, true);
            SetDoubleBuffered(dataGridView2, true);
            SetDoubleBuffered(dataGridView3, true);
            FIOL.Visible = FIO.Visible = !editAval && comboEdr.SelectedIndex == 3;
        }
        public static void ImportData(DataGridView dataGridView, string tableName)
        {
            List<AnotherPay> aPays = new List<AnotherPay>();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {

                AnotherPay aPay = new AnotherPay()
                {
                    NAME = row.Cells[0].Value.ToString(),
                    ERDPO = row.Cells[1].Value.ToString(),
                    RRahunok = row.Cells[2].Value.ToString(),
                    Comment = row.Cells[3].Value.ToString()

                };
                aPays.Add(aPay);
            }
            var count = AnotherPay_.InsertTableFromList(tableName, aPays);
            var dell = AnotherPay_.DeleteDublicate(tableName);
            // MessageBox.Show("Кількість " + (count - dell));
        }


        private void ComboEdr_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboEdr.SelectedIndex)
            {
                case 0:
                    SetFieldsP(aval);
                    button3.Enabled = true;
                    break;
                case 1:
                    SetFieldsP(industrial);
                    button3.Enabled = true;
                    break;
                case 2:
                    SetFieldsP(oschad);
                    button3.Enabled = true;
                    break;
                case 3:
                    SetFieldsP(pumb);
                    button3.Enabled = true;
                    break;
                case 4:
                    SetFieldsP(ukrGaz2);
                    button3.Enabled = false;
                    break;
            }
        }

        public void BackUpData()
        {
            if (Settings.Default.count % 10 == 0)
            {
                string directory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}" +
                                "\\PayConverterBackup";
                bool exists = Directory.Exists(directory);
                if (!exists)
                {
                    Directory.CreateDirectory(directory);
                }

                string date = DateTime.Today.ToString("ddMMyyyy");
                string bakFilePath = directory + "\\" + date + "PayConverterData.xml" + ".bak";
                Xml.saveXml(dataGridView3, bakFilePath);
            }
        }

        public void SetFieldsP(Bank bank)
        {
            mfo.Text = bank.mfo;
            rahunok.Text = bank.rahunok;
            cliBankCode.Text = bank.clientBankCode;
            erdpo1.Text = bank.edrpou;
            dateTimePicker1.Value = DateTime.Now;
            tableLayoutPanel7.RowStyles[1].Height = 100;
            tableLayoutPanel7.RowStyles[0].Height = 0;
            dataGridView2.Visible = true;
            dataGridView1.Visible = false;
        }

        public void SetFieldsP2()
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
            //dataGridView3.Rows.Clear();
            //dataGridView1.DataSource ="";
            // dataGridView1.DataSource ="";
            dataGridView3.DataSource = "";

            path = string.Empty;
            InitPData();
            OpenCsv();
            // dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
        }

        public void OpenCsv()
        {
            openFileDialog1.FileName = "file"; //
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show("row "+dataGridView1.Rows.Count);
                if (dataGridView1.Rows.Count > 0 || dataGridView2.Rows.Count > 0)
                {
                    //dataGridView1.Rows.Clear();
                    // dataGridView2.Rows.Clear();
                    //dataGridView3.Rows.Clear();
                    dataGridView1.Rows.Clear();
                    dataGridView2.Rows.Clear();
                    dataGridView3.DataSource = "";

                    numberDocAval = 1;
                }

                path = name = openFileDialog1.FileName;
                LoadFileRoot();

                if (isNull)
                {
                    if (!anotherPay.Checked)
                    {
                        //Xml.saveXml(dataGridView3, path2);
                        var dell = AnotherPay_.DeleteDublicate("PayConverterData");
                        dataGridView3.DataSource = Db.SelectTable<AnotherPay>("PayConverterData");
                    }
                    else if (anotherPay.Checked)
                    {
                        var dell = AnotherPay_.DeleteDublicate("AnotherPayConverterData");
                        dataGridView3.DataSource = Db.SelectTable<AnotherPay>("AnotherPayConverterData");
                        //Xml.saveXml(dataGridView3, path3);
                    }
                }
            }
        }

        public void LoadFileRoot()
        {
            List<Bank> CSV_Struct = new List<Bank>();

            CSV_Struct = Bank.ReadFile(path, anotherPay.Checked);


            DateTime dt1 = DateTime.Today;
            for (int i = 0; i <= CSV_Struct.Count - 1; i++)
            {
                int n = 0;
                CultureInfo myCultureInfo = new CultureInfo("uk-UA");
                if (CSV_Struct[i].id == 0)
                {
                    //todo: remove messagebox 
                    // MessageBox.Show("СТРУКТУРА АЙДИ 0");
                    n = dataGridView1.Rows.Add();

                    dataGridView1.Rows[n].Cells[0].Value = CSV_Struct[i].summa;
                    dataGridView1.Rows[n].Cells[1].Value = "UAH";
                    if (!anotherPay.Checked)
                    {
                        //Призначення платежу
                        dataGridView1.Rows[n].Cells[2].Value = AddDateToStr(
                            FindZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                            CSV_Struct[i].dateP == dt1
                                ? dateTimePicker1.Value.ToString("dd.MM.yyyy").Replace("null", "") + " " + CSV_Struct[i].Appointment
                                : CSV_Struct[i].dateP.ToString("dd.MM.yyyy")).Replace("null", "") + " " + CSV_Struct[i].Appointment;

                        dataGridView1.Rows[n].Cells[8].Value =
                            FindNameZkpo(CSV_Struct[i].name, CSV_Struct[i].edrpou, CSV_Struct[i].rahunok).Equals("null")
                                ? CSV_Struct[i].name
                                : FindNameZkpo(CSV_Struct[i].name, CSV_Struct[i].edrpou, CSV_Struct[i].rahunok);
                    }
                    else
                    {
                        dataGridView1.Rows[n].Cells[2].Value = CSV_Struct[i].name;
                        dataGridView1.Rows[n].Cells[8].Value = CSV_Struct[i].pruznach;
                    }

                    //dataGridView1.Rows[n].Cells[2].Value = AddDateToStr(FindZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                    //(CSV_Struct[i].dateP == dt1 ? dateTimePicker1.Value.ToString("dd.MM.yyyy") : CSV_Struct[i].dateP.ToString("dd.MM.yyyy")));

                    if (dataGridView1.Rows[n].Cells[2].Value.Equals("null") || anotherPay.Checked &&
                        dataGridView1.Rows[n].Cells[2].Value.ToString() != "null")
                    {
                        dataGridView1.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                        //int m = dataGridView3.Rows.Add();
                        //dataGridView3.Rows[m].Cells[0].Value = CSV_Struct[i].name;
                        //dataGridView3.Rows[m].Cells[1].Value = CSV_Struct[i].edrpou;
                        //dataGridView3.Rows[m].Cells[2].Value = CSV_Struct[i].rahunok;
                        //dataGridView3.Rows[m].Cells[3].Value = dataGridView1.Rows[n].Cells[2].Value;
                        try
                        {
                            AnotherPay pay = new AnotherPay()
                            {
                                NAME = CSV_Struct[i].name,
                                ERDPO = CSV_Struct[i].edrpou,
                                RRahunok = CSV_Struct[i].pruznach,
                                Comment = dataGridView2.Rows[n].Cells[2].Value.ToString()
                            };
                            AnotherPay_.InsertData("AnotherPayConverterData", pay, out long idN);
                        }
                        catch { }
                        isNull = true;
                    }

                    dataGridView1.Rows[n].Cells[3].Value = ukrGaz.rahunok;
                    dataGridView1.Rows[n].Cells[4].Value = ukrGaz.edrpou;

                    dataGridView1.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                    dataGridView1.Rows[n].Cells[6].Value = CSV_Struct[i].rahunok;
                    dataGridView1.Rows[n].Cells[7].Value = CSV_Struct[i].edrpou;

                    dataGridView1.Rows[n].Cells[9].Value = ukrGaz.iban;
                    dataGridView1.Rows[n].Cells[10].Value = anotherPay.Checked ? PurposeOfPayment_.GetPurpose(dataGridView1.Rows[n].Cells[8].Value.ToString()) : "";
                    //if (dataGridView1.Rows[n].Cells[10].Value.ToString().Length > 10)
                    //    {
                    //        dataGridView1.Rows[n].Cells[10].Style.BackColor = Color.Red;
                    //    }
                }


                //comboEdr.SelectedIndex
                else if (CSV_Struct[i].id == 1)
                {
                    //todo: remove messagebox 
                    //MessageBox.Show("СТРУКТУРА АЙДИ 1");  
                    // try
                    // {

                    dateTimePicker1.Value =
                        DateTime.Parse(CSV_Struct[i].dateP.ToString("dd.MM.yyyy"), myCultureInfo);

                    n = dataGridView2.Rows.Add();

                    dataGridView2.Rows[n].Cells[0].Value = "0";

                    dataGridView2.Rows[n].Cells[1].Value = "1";
                    dataGridView2.Rows[n].Cells[2].Value = numberDocAval++;
                    // MessageBox.Show(dataGridView2.Rows[n].Cells[2].Value.ToString());
                    dataGridView2.Rows[n].Cells[3].Value = DateTime.Today.ToString("dd.MM.yyyy");
                    dataGridView2.Rows[n].Cells[4].Value = comboEdr.SelectedIndex == 1 ? industrial.mfo :
                        comboEdr.SelectedIndex == 2 ? oschad.mfo : comboEdr.SelectedIndex == 3 ? pumb.mfo : aval.mfo;
                    dataGridView2.Rows[n].Cells[5].Value = CSV_Struct[i].mfo;
                    dataGridView2.Rows[n].Cells[6].Value =
                        comboEdr.SelectedIndex == 1 ? industrial.rahunok :
                        comboEdr.SelectedIndex == 2 ? oschad.rahunok : comboEdr.SelectedIndex == 3 ? pumb.rahunok : aval.rahunok;
                    dataGridView2.Rows[n].Cells[7].Value = CSV_Struct[i].rahunok;
                    dataGridView2.Rows[n].Cells[8].Value = CSV_Struct[i].summa;
                    dataGridView2.Rows[n].Cells[9].Value = "0";
                    dataGridView2.Rows[n].Cells[12].Value = CSV_Struct[i].edrpou;

                    if (!anotherPay.Checked && (comboEdr.SelectedIndex.ToString() == "0" || comboEdr.SelectedIndex.ToString() == "2" || comboEdr.SelectedIndex.ToString() == "3"))
                    {
                        dataGridView2.Rows[n].Cells[10].Value =
                        FindNameZkpo(CSV_Struct[i].name, CSV_Struct[i].edrpou, CSV_Struct[i].rahunok)
                            .Equals("null")
                            ? CSV_Struct[i].name
                            : FindNameZkpo(CSV_Struct[i].name, CSV_Struct[i].edrpou, CSV_Struct[i].rahunok);
                        //Призначення платежу
                        dataGridView2.Rows[n].Cells[11].Value = AddDateToStr(
                            FindZkpo(CSV_Struct[i].edrpou, CSV_Struct[i].rahunok),
                            CSV_Struct[i].dateP.ToString("dd.MM.yyyy")).Replace("null", "") + " " + CSV_Struct[i].Appointment;
                    }
                    else //todo: пофиксить сохранение базы индустриала
                    {
                        string cells10 = CSV_Struct[i].name;
                        string nameRecipient = AlphaBeta(cells10);
                        dataGridView2.Rows[n].Cells[10].Value = nameRecipient;

                        dataGridView2.Rows[n].Cells[11].Value = AlphaBeta(CSV_Struct[i].pruznach);
                        dataGridView2.Rows[n].Cells[13].Value = anotherPay.Checked ? PurposeOfPayment_.GetPurpose(nameRecipient.Trim()) : "";
                        // MessageBox.Show(nameRecipient);
                    }


                    if (dataGridView2.Rows[n].Cells[11].Value.Equals("null") || anotherPay.Checked &&
                        dataGridView2.Rows[n].Cells[11].Value.ToString() != "null")
                    {
                        dataGridView2.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                        // int m = dataGridView3.Rows.Add();
                        // dataGridView3.Rows[m].Cells[0].Value = CSV_Struct[i].name;
                        // dataGridView3.Rows[m].Cells[1].Value = CSV_Struct[i].edrpou;
                        //dataGridView3.Rows[m].Cells[2].Value = CSV_Struct[i].rahunok;
                        // dataGridView3.Rows[m].Cells[3].Value = CSV_Struct[i].pruznach;
                        //dataGridView3.Rows[m].Cells[3].Value = dataGridView2.Rows[n].Cells[11].Value;
                        try
                        {


                            AnotherPay pay = new AnotherPay()
                            {
                                NAME = CSV_Struct[i].name,
                                ERDPO = CSV_Struct[i].edrpou,
                                RRahunok = CSV_Struct[i].pruznach,
                                Comment = dataGridView2.Rows[n].Cells[11].Value.ToString()
                            };
                            AnotherPay_.InsertData("AnotherPayConverterData", pay, out long idN);
                        }
                        catch { }
                        isNull = true;
                    }
                    //}
                    // catch(Exception ex)
                    //{
                    //     MessageBox.Show(ex.Message);
                    // }
                }
            }

            if (isNull)
            {
                if (!anotherPay.Checked)
                {
                    //Xml.saveXml(dataGridView3, path2);
                    var dell = AnotherPay_.DeleteDublicate("PayConverterData");
                    dataGridView3.DataSource = Db.SelectTable<AnotherPay>("PayConverterData");
                }
                else if (anotherPay.Checked)
                {
                    var dell = AnotherPay_.DeleteDublicate("AnotherPayConverterData");
                    dataGridView3.DataSource = Db.SelectTable<AnotherPay>("AnotherPayConverterData");
                    //Xml.saveXml(dataGridView3, path3);
                }
            }
        }
        public string AlphaBeta(string text)
        {
            string alfabet = @"""ЙЦУКЕНГШЩЗХЇФІВАПРОЛДЖЄЯЧСМИТЬБЮйцукенгшщзхїфівапролджєячсмитьбю '-0123456789()!N№qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBM`?";
            text.ToList().ForEach(x => { if (!(alfabet.Contains(x))) { text = text.Replace(x, '_').Replace("_", "").Replace("undefined", ""); }; });
            return text;
        }
        public void AutoOpenCsv(string path, int type)
        {
            isNull = false;
            if (dataGridView1.Rows.Count > 0 || dataGridView2.Rows.Count > 0)
            {
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
            }

            dataGridView3.DataSource = "";
            //Xml.loadXml(dataGridView3, path);
            string tableName = type == 2 ? "PayConverterData" : "AnotherPayConverterData";
            dataGridView3.DataSource = Db.SelectTable<AnotherPay>(tableName);
            LoadFileRoot();

            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            // dataGridView2.Sort(dataGridView2.Columns[11], ListSortDirection.Ascending);
        }

        public string AddDateToStr(string str, string date)
        {
            if (str.Equals("null"))
            {
                return "null";
            }

            str = str.Replace("##.##.####", date);
            return str;
        }

        public string FindZkpo(string zkpo, string rrahunok)
        {
            foreach (DataGridViewRow r in dataGridView3.Rows) // пока в dataGridView1 есть строки
            {
                if (r.Cells != null)
                {
                    try
                    {
                        if (r.Cells[2].Value.Equals(zkpo) && r.Cells[3].Value.Equals(rrahunok))
                        {
                            return r.Cells[4].Value.ToString();
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

        public string FindNameZkpo(string zkpo, string rrahunok)
        {
            foreach (DataGridViewRow r in dataGridView3.Rows)
            {
                if (r.Cells != null)
                {
                    try
                    {
                        if (r.Cells[2].Value.Equals(zkpo) && r.Cells[3].Value.Equals(rrahunok))
                        {
                            return r.Cells[1].Value.ToString().ToUpper();
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

        public string FindNameZkpo(string name, string zkpo, string rrahunok)
        {
            foreach (DataGridViewRow r in dataGridView3.Rows)
            {
                if (r.Cells != null)
                {
                    try
                    {
                        if (r.Cells[1].Value.Equals(name) && r.Cells[2].Value.Equals(zkpo) &&
                            r.Cells[3].Value.Equals(rrahunok))
                        {
                            //MessageBox.Show(" " + name  + " summa " + r.Cells[0].Value);
                            return r.Cells[1].Value.ToString().ToUpper();
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

        public string ConverterDate(string dateS)
        {
            if (!string.IsNullOrEmpty(dateS))
            {
                string t = dateS.Replace(".", "");
                return t.Substring(4, 4) + t.Substring(2, 2) + t.Substring(0, 2);
            }

            return "";
        }

        public void Save()
        {
            saveFileDialog1.FileName = GetNameFile();
            saveFileDialog1.Title = "Зберегти";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string k = "" + DateTime.Now;
                name = saveFileDialog1.FileName;
                CreateBox();
                string texts = textImport.Text.Replace("і", "i").Replace("І", "I");
                File.WriteAllText(name, texts, Encoding.GetEncoding(866));
                // MessageBox.Show("Збережено!");
                MessageBox.Show(this, "Збережено!",
                                   "Збережено", MessageBoxButtons.OK,
                                   MessageBoxIcon.Information,
                                   MessageBoxDefaultButton.Button1);
            }
        }

        public void CreateBox() //создаем файл импорта для аваля
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

                    textImport.Text += r.Cells[0].Value + P + r.Cells[1].Value + P + P + ConverterDate(t) + P;
                    textImport.Text += r.Cells[4].Value + P + r.Cells[5].Value + P + r.Cells[6].Value + P +
                                       r.Cells[7].Value + P;
                    textImport.Text += sum + P + r.Cells[9].Value + P + r.Cells[10].Value + P + r.Cells[11].Value + P +
                                       P + P + P + P + r.Cells[12].Value + P + P + "\r\n";
                }
            }
        }

        public string GetNameFile()
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


        public void SaveOschadDbf()
        {
            // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string pathDbf = Directory.GetCurrentDirectory() + "\\ОщадБанк\\";

            if (!Directory.Exists(pathDbf))
            {
                Directory.CreateDirectory(pathDbf);
            }

            string dateTime = DateTime.Now.ToString("dd/MM/yy");
            string fileName = $"{pathDbf}{dateTime}.dbf";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (Stream fos = File.Open(fileName, FileMode.OpenOrCreate,
                FileAccess.ReadWrite))
            using (DBFWriter writer = new DBFWriter())
            {
                writer.CharEncoding = Encoding.GetEncoding(1251);//866
                writer.Signature = NotSet;
                //writer.LanguageDriver = 0x26; // кодировка 866
                DBFField field1 = new DBFField("ndoc", NativeDbType.Char, 10); //номер документа
                DBFField field2 = new DBFField("dt", NativeDbType.Date); //дата документа
                DBFField field3 = new DBFField("dv", NativeDbType.Date); //Дата валютування
                DBFField field4 =
                    new DBFField("acccli", NativeDbType.Char, 29); //рахунок клієнта   UA243020760000026501300388426 //Рахунок (IBAN )відправника
                DBFField field5 = new DBFField("acccor", NativeDbType.Char, 29); //рахунок кореспондента Рахунок (IBAN )отримувача
                DBFField field6 = new DBFField("okpocor", NativeDbType.Char, 14); //ЗКПО кореспондента //Податковий код отримувача (ІПН, ЄДРПОУ, ЗКПО)
                DBFField field7 = new DBFField("namecor", NativeDbType.Char, 38); //Назва отримувача
                DBFField field8 = new DBFField("summa", NativeDbType.Numeric, 20); //сума платежу «в копійках»
                DBFField field9 = new DBFField("val", NativeDbType.Numeric, 4); //код валюти платежу
                DBFField field10 = new DBFField("nazn", NativeDbType.Char, 160); //призначення платежу
                DBFField field11 = new DBFField("cod_cor", NativeDbType.Numeric, 4);//Код країни-нерезидента отримувача (ISO 3166-1 numeric)**
                DBFField field12 = new DBFField("add_req", NativeDbType.Char, 38);//Додаткові реквізити*



                writer.Fields = new[]
                {
                    field1, field2, field3, field4, field5, field6, field7, field8, field9, field10, field11, field12,

                };
                if (string.IsNullOrEmpty(docNumOschad.Text))
                {
                    docNumOschad.Text = "1";
                }
                Int32.TryParse(docNumOschad.Text, out int docNum);

                string zkpo = string.IsNullOrEmpty(erdpo1.Text) ? "40375721" : erdpo1.Text;
                // string cliName = "ТОВ \"ФК\"МПС\"";
                // string cliBankName = "Вінницьке обласне управління АТ \"Ощадбанк\"";
                // int debCred = 1;
                //  string bankKorespond = "";
                int codeVal = 980;

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    // MessageBox.Show("type " + int.Parse(row.Cells[8].Value.ToString(), NumberStyles.AllowThousands, new CultureInfo("en-au")));

                    int summa = Convert.ToInt32(row.Cells[8].Value.ToString().Replace(".", ""));
                    writer.AddRecord(
                        // добавляем поля в набор
                        "", //docNum.ToString(), //1
                        DateTime.Now, //2 dt
                         DateTime.Now, //3 dv
                         row.Cells[6].Value, //4 acccli
                         row.Cells[7].Value.ToString(), //5 acccor
                         row.Cells[12].Value.ToString(), //6 okpocor
                         row.Cells[10].Value.ToString(), //7 namecor
                         summa, //8 summa
                         codeVal, //9 val
                         row.Cells[11].Value.ToString().Trim(), //10 nazn
                         804,//cod_cor 11
                         "" //add_req 12




                    );
                    docNum++;
                }

                writer.Write(fos);
            }
            if (File.Exists(fileName))
            {
                MessageBox.Show($"Файл збережено: {fileName}");
            }

        }
        /// <summary>
        /// Pumb
        /// </summary>
        public bool SavePumbDbf(out string pathT, bool anotherPayCh)
        {
            // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string pathDbf = Directory.GetCurrentDirectory() + "\\Пумб\\";

            if (!Directory.Exists(pathDbf))
            {
                Directory.CreateDirectory(pathDbf);
            }

            string dateTime = DateTime.Now.ToString().Replace(":", "_");


            string path = $"{pathDbf}" + $"{dateTime}" + ".dbf";

            using (Stream pumb = File.Open(path, FileMode.OpenOrCreate,
                FileAccess.ReadWrite))
            using (DBFWriter writer = new DBFWriter())
            {
                writer.CharEncoding = Encoding.GetEncoding(866);
                writer.Signature = DBase3WithMemo;
                writer.LanguageDriver = 0x26; // кодировка 866
                DBFField field1 = new DBFField("DAY", NativeDbType.Char, 10); //Дата документа 
                DBFField field2 = new DBFField("NUMBER", NativeDbType.Char, 10); //Номер документа  
                DBFField field3 = new DBFField("A", NativeDbType.Char, 38); // Наименование плательщика 
                DBFField field4 = new DBFField("B", NativeDbType.Char, 38); //Наименование получателя
                DBFField field5 = new DBFField("OKPO_A", NativeDbType.Char, 14); //Код плательщика 
                DBFField field6 = new DBFField("OKPO_B", NativeDbType.Char, 14); //Код получателя  
                DBFField field7 = new DBFField("ACCOUNT_A", NativeDbType.Char, 29); //Номер счета плательщика 
                DBFField field8 = new DBFField("ACCOUNT_B", NativeDbType.Char, 29); //Номер счета получателя  
                DBFField field9 = new DBFField("BANK_A", NativeDbType.Char, 38); //Наименование банка плательщика 
                DBFField field10 = new DBFField("BANK_B", NativeDbType.Char, 38); //Наименование банка получателя 
                DBFField field11 = new DBFField("MFO_A", NativeDbType.Char, 9); //Код МФО банка плательщика  
                DBFField field12 = new DBFField("MFO_B", NativeDbType.Char, 9); //Код МФО банка получателя 
                DBFField field13 = new DBFField("CITY_A", NativeDbType.Char, 3); //код страны плательщика (для нерезидентов)  804 
                DBFField field14 = new DBFField("CITY_B", NativeDbType.Char, 3); // код страны получателя (для нерезидентов) 804 
                DBFField field15 = new DBFField("AMOUNT", NativeDbType.Char, 18); //(максимум 15 знаков + 2 знака после десятичного разделителя)    
                DBFField field16 = new DBFField("DETAILS", NativeDbType.Char, 160); //Назначение платежа 
                DBFField field17 = new DBFField("GUILTY", NativeDbType.Char, 50); //Ответственный  
                DBFField field18 = new DBFField("DETAILS_T", NativeDbType.Char, 50); //Доп. Характеристика  


                writer.Fields = new[]
                {
                    field1, field2, field3, field4, field5, field6, field7, field8, field9, field10, field11, field12,
                    field13, field14, field15, field16, field17, field18
                };

                if (string.IsNullOrEmpty(docNumOschad.Text))
                {
                    docNumOschad.Text = "1";
                }
                Int32.TryParse(docNumOschad.Text, out int docNum);
                string zkpo = string.IsNullOrEmpty(erdpo1.Text) ? "40375721" : erdpo1.Text;
                string cliName = "ТОВ \"ФК\"МПС\"";
                string cliBankName = "Відділення ПУМБ у Вінницькій області";


                int codeVal = 804;
                List<Dbf> dbfs = new List<Dbf>();
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells[13].Value == null)
                    {
                        row.Cells[13].Value = string.Empty;
                    }
                    if (row.Cells[11].Value == null)
                    {
                        row.Cells[11].Value = string.Empty;
                    }
                    string details = anotherPayCh ? ChangeI(string.Join(" ", row.Cells[13].Value.ToString(), row.Cells[11].Value.ToString())) : ChangeI(row.Cells[11].Value.ToString());
                    if (details.Length > 160)
                    {
                        details = ChangeI(row.Cells[11].Value.ToString());
                    }
                    Dbf dbf = new Dbf()
                    {
                        DAY = row.Cells[3].Value.ToString(),
                        NUMBER = docNum.ToString(),
                        A = ChangeI(cliName),
                        B = ChangeI(row.Cells[10].Value.ToString()),
                        OKPO_A = zkpo,
                        OKPO_B = ChangeI(row.Cells[12].Value.ToString()),
                        ACCOUNT_A = rahunok.Text,
                        ACCOUNT_B = ChangeI(row.Cells[7].Value.ToString()),
                        BANK_A = ChangeI(cliBankName),
                        BANK_B = "",//"ПАT \"ПУМБ\"",
                        MFO_A = ChangeI(row.Cells[4].Value.ToString()),
                        MFO_B = ChangeI(row.Cells[5].Value.ToString()),
                        CITY_A = codeVal.ToString(),
                        CITY_B = codeVal.ToString(),
                        AMOUNT = ChangeI(row.Cells[8].Value.ToString()),
                        // DETAILS = ChangeI(string.Join(" ", row.Cells[13].Value.ToString(), row.Cells[11].Value.ToString())),                        
                        DETAILS = details,
                        GUILTY = FIO.Text,
                        DETAILS_T = ""
                    };

                    dbfs.Add(dbf);

                    docNum++;
                }
                dbfs.ForEach(db =>
                {
                    writer.AddRecord(db.DAY, db.NUMBER, db.A, db.B, db.OKPO_A, db.OKPO_B, db.ACCOUNT_A, db.ACCOUNT_B, db.BANK_A, db.BANK_B, db.MFO_A, db.MFO_B, db.CITY_A, db.CITY_B, db.AMOUNT, db.DETAILS, db.GUILTY, db.DETAILS_T);
                });
                writer.Write(pumb);
            }
            pathT = path;
            return File.Exists(path);
        }
        public bool SavePumbDbf2(out string pathT, bool anotherPayCh)
        {

            string pathDbf = Directory.GetCurrentDirectory() + "\\Пумб\\";

            if (!Directory.Exists(pathDbf))
            {
                Directory.CreateDirectory(pathDbf);
            }

            string dateTime = DateTime.Now.ToString().Replace(":", "_");


            string path = $"{pathDbf}" + $"{dateTime}" + ".dbf";

            using (Stream pumb = File.Open(path, FileMode.OpenOrCreate,
                FileAccess.ReadWrite));

            //dBASE.NET.Dbf writer = new dBASE.NET.Dbf(Encoding.GetEncoding(866));
            dBASE.NET.Dbf writer = new dBASE.NET.Dbf(Encoding.GetEncoding(1251));
            dBASE.NET.DbfField field1 = new dBASE.NET.DbfField("DAY", DbfFieldType.Character, 10); //Дата документа 

            dBASE.NET.DbfField field2 = new dBASE.NET.DbfField("NUMBER", DbfFieldType.Character, 10); //Номер документа  
            dBASE.NET.DbfField field3 = new dBASE.NET.DbfField("A", DbfFieldType.Character, 38); // Наименование плательщика 
            dBASE.NET.DbfField field4 = new dBASE.NET.DbfField("B", DbfFieldType.Character, 38); //Наименование получателя
            dBASE.NET.DbfField field5 = new dBASE.NET.DbfField("OKPO_A", DbfFieldType.Character, 14); //Код плательщика 
            dBASE.NET.DbfField field6 = new dBASE.NET.DbfField("OKPO_B", DbfFieldType.Character, 14); //Код получателя  
            dBASE.NET.DbfField field7 = new dBASE.NET.DbfField("ACCOUNT_A", DbfFieldType.Character, 29); //Номер счета плательщика 
            dBASE.NET.DbfField field8 = new dBASE.NET.DbfField("ACCOUNT_B", DbfFieldType.Character, 29); //Номер счета получателя  
            dBASE.NET.DbfField field9 = new dBASE.NET.DbfField("BANK_A", DbfFieldType.Character, 38); //Наименование банка плательщика 
            dBASE.NET.DbfField field10 = new dBASE.NET.DbfField("BANK_B", DbfFieldType.Character, 38); //Наименование банка получателя 
            dBASE.NET.DbfField field11 = new dBASE.NET.DbfField("MFO_A", DbfFieldType.Character, 9); //Код МФО банка плательщика  
            dBASE.NET.DbfField field12 = new dBASE.NET.DbfField("MFO_B", DbfFieldType.Character, 9); //Код МФО банка получателя 
            dBASE.NET.DbfField field13 = new dBASE.NET.DbfField("CITY_A", DbfFieldType.Character, 3); //код страны плательщика (для нерезидентов)  804 
            dBASE.NET.DbfField field14 = new dBASE.NET.DbfField("CITY_B", DbfFieldType.Character, 3); // код страны получателя (для нерезидентов) 804 
            dBASE.NET.DbfField field15 = new dBASE.NET.DbfField("AMOUNT", DbfFieldType.Character, 18); //(максимум 15 знаков + 2 знака после десятичного разделителя)    
            dBASE.NET.DbfField field16 = new dBASE.NET.DbfField("DETAILS", DbfFieldType.Character, 160); //Назначение платежа 
            dBASE.NET.DbfField field17 = new dBASE.NET.DbfField("GUILTY", DbfFieldType.Character, 50); //Ответственный  
            dBASE.NET.DbfField field18 = new dBASE.NET.DbfField("DETAILS_T", DbfFieldType.Character, 50); //Доп. Характеристика  

            var fields = new List<DbfField>
                {
                    field1, field2, field3, field4, field5, field6, field7, field8, field9, field10, field11, field12,
                    field13, field14, field15, field16, field17, field18
                };
            fields.ForEach(f => writer.Fields.Add(f));

            if (string.IsNullOrEmpty(docNumOschad.Text))
            {
                docNumOschad.Text = "1";
            }
            Int32.TryParse(docNumOschad.Text, out int docNum);
            string zkpo = string.IsNullOrEmpty(erdpo1.Text) ? "40375721" : erdpo1.Text;
            string cliName = "ТОВ \"ФК\"МПС\"";
            string cliBankName = "Відділення ПУМБ у Вінницькій області";


            int codeVal = 804;
            List<Dbf> dbfs = new List<Dbf>();
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells[13].Value == null)
                {
                    row.Cells[13].Value = string.Empty;
                }
                if (row.Cells[11].Value == null)
                {
                    row.Cells[11].Value = string.Empty;
                }
                string details = anotherPayCh ? ChangeI(string.Join(" ", row.Cells[13].Value.ToString(), row.Cells[11].Value.ToString())) : ChangeI(row.Cells[11].Value.ToString());
                if (details.Length > 160)
                {
                    details = ChangeI(row.Cells[11].Value.ToString());
                }
                Dbf dbf = new Dbf()
                {
                    DAY = row.Cells[3].Value.ToString(),
                    NUMBER = docNum.ToString(),
                    A = ChangeI(cliName),
                    B = ChangeI(row.Cells[10].Value.ToString()),
                    OKPO_A = zkpo,
                    OKPO_B = ChangeI(row.Cells[12].Value.ToString()),
                    ACCOUNT_A = rahunok.Text,
                    ACCOUNT_B = ChangeI(row.Cells[7].Value.ToString()),
                    BANK_A = ChangeI(cliBankName),
                    BANK_B = "",//"ПАT \"ПУМБ\"",
                    MFO_A = ChangeI(row.Cells[4].Value.ToString()),
                    MFO_B = ChangeI(row.Cells[5].Value.ToString()),
                    CITY_A = codeVal.ToString(),
                    CITY_B = codeVal.ToString(),
                    AMOUNT = ChangeI(row.Cells[8].Value.ToString()),
                    // DETAILS = ChangeI(string.Join(" ", row.Cells[13].Value.ToString(), row.Cells[11].Value.ToString())),                        
                    DETAILS = details,
                    GUILTY = FIO.Text,
                    DETAILS_T = ""
                };

                dbfs.Add(dbf);

                docNum++;
            }
            dbfs.ForEach(db =>
            {
                DbfRecord record = writer.CreateRecord();
                record.Data[0] = db.DAY;
                record.Data[1] = db.NUMBER;
                record.Data[2] = db.A;
                record.Data[3] = db.B;
                record.Data[4] = db.OKPO_A;
                record.Data[5] = db.OKPO_B;
                record.Data[6] = db.ACCOUNT_A;
                record.Data[7] = db.ACCOUNT_B;
                record.Data[8] = db.BANK_A;
                record.Data[9] = db.BANK_B;
                record.Data[10] = db.MFO_A;
                record.Data[11] = db.MFO_B;
                record.Data[12] = db.CITY_A;
                record.Data[13] = db.CITY_B;
                record.Data[14] = db.AMOUNT;
                record.Data[15] = db.DETAILS;
                record.Data[16] = db.GUILTY;
                record.Data[17] = db.DETAILS_T;


            });
            writer.Write(path, DbfVersion.FoxBaseDBase3NoMemo);

            pathT = path;
            return File.Exists(path);
        }


        public string ChangeI(string text)
        {
            return text.Replace("і", "i").Replace("І", "I");
        }

        public void SaveXml()
        {
            string time = DateTime.Now.ToString("ddMMyyyy");


            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.GetEncoding(1251)
            };
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
                    xmlWriter.WriteAttributeString("DOCUMENTDATE", ConverterDate(t));
                    xmlWriter.WriteAttributeString("BANKID", "300346");
                    xmlWriter.WriteAttributeString("IBAN", "UA633003460000026507069842401");
                    try
                    {
                        xmlWriter.WriteAttributeString("CORRBANKID", r.Cells[5].Value.ToString());
                    }
                    catch { }
                    try
                    {
                        xmlWriter.WriteAttributeString("CORRIBAN", r.Cells[7].Value.ToString());
                    }
                    catch { }
                    try
                    {

                    }
                    catch { }
                    xmlWriter.WriteAttributeString("AMOUNT", sum);
                    try
                    {
                        xmlWriter.WriteAttributeString("CORRSNAME", r.Cells[10].Value.ToString());
                    }
                    catch { }
                    try
                    {
                        xmlWriter.WriteAttributeString("DETAILSOFPAYMENT", r.Cells[11].Value.ToString());
                    }
                    catch { }
                    try
                    {
                        xmlWriter.WriteAttributeString("CORRIDENTIFYCODE", r.Cells[12].Value.ToString());
                    }
                    catch { }


                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }


        public void SaveExcel(DataGridView dataGridViewn, int type, string rahunok = "", bool isUkrGaz = false)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files(2003)|*.xls|Excel files(2007+)| *.xlsx",
                FilterIndex = 2,
                FileName = DateTime.Now.ToString().Replace(":", "_")
            };
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                if (type == 0)
                {
                    SaveExcel(saveDialog, dataGridViewn, anotherPay.Checked, isUkrGaz);
                }
                else if (type == 2)
                {
                    Int32.TryParse(docNumOschad.Text, out int docnum);
                    if (docnum == 0)
                    {
                        docnum = 1;
                    }
                    Service.Template.GetExcel(Service.Template.ConvertTableToOschad(dataGridView2, docnum, rahunok, anotherPay.Checked), saveDialog.FileName, progressBar1);
                    //SaveExcelOschad(saveDialog);
                }


            }
        }
        public void SaveExcelOschad(SaveFileDialog saveDialog) //ощад банк
        {
            // Creating a Excel object.
            _Application excel = new Microsoft.Office.Interop.Excel.Application();
            _Workbook workbook = excel.Workbooks.Add(Type.Missing);
            _Worksheet worksheet = null;
            try
            {
                progressBar1.Visible = true;
                ModifyProgressBarColor.SetState(progressBar1, 3);
                progressBar1.Minimum = 1;
                progressBar1.Maximum = dataGridView2.Rows.Count + 1;
                progressBar1.Value = 1;
                progressBar1.Step = 1;

                worksheet = workbook.ActiveSheet;
                worksheet.Rows.NumberFormatLocal = "@";
                worksheet.Columns.NumberFormatLocal = "@";
                worksheet.Name = "Data";

                int cellRowIndex = 3;

                int codeVal = 980;
                int countryCode = 804;
                worksheet.Cells[1, 1].Value = "обов'язкове";
                worksheet.Cells[1, 2].Value = "обов'язкове";
                worksheet.Cells[1, 3].Value = "необов'язкове";
                worksheet.Cells[1, 4].Value = "обов'язкове";
                worksheet.Cells[1, 5].Value = "обов'язкове";
                worksheet.Cells[1, 6].Value = "обов'язкове";
                worksheet.Cells[1, 7].Value = "обов'язкове";
                worksheet.Cells[1, 8].Value = "обов'язкове";
                worksheet.Cells[1, 9].Value = "обов'язкове";
                worksheet.Cells[1, 10].Value = "обов'язкове";
                worksheet.Cells[1, 11].Value = "* якщо податковий код дорівнює 00000000 це поле обов'язкове";
                worksheet.Cells[1, 12].Value = "** якщо податковий код дорівнює 0000000000 це поле обов'язкове";

                worksheet.Cells[2, 1].Value = "Номер платіжного документу (ndoc)";
                worksheet.Cells[2, 2].Value = "Дата документу, дд.мм.рррр (dt)";
                worksheet.Cells[2, 3].Value = "Дата валютування, дд.мм.рррр (dv)";
                worksheet.Cells[2, 4].Value = "Рахунок відправника (acccli)";
                worksheet.Cells[2, 5].Value = "Рахунок отримувача (acccor)";
                worksheet.Cells[2, 6].Value = "Податковий код отримувача (ІПН, ЄДРПОУ, ЗКПО)** (okpocor)";
                worksheet.Cells[2, 7].Value = "Назва отримувача (namecor)";
                worksheet.Cells[2, 8].Value = "Сума платежу    (у копійках) (summa)";
                worksheet.Cells[2, 9].Value = "Валюта, ISO 4217 (val)";
                worksheet.Cells[2, 10].Value = "Призначення платежу (nazn)";
                worksheet.Cells[2, 11].Value = "Код країни-нерезидента отримувача (ISO 3166-1 numeric) (cod_cor)";
                worksheet.Cells[2, 12].Value = "Додаткові реквізити (add_req)";
                Int32.TryParse(docNumOschad.Text, out int docnum);
                if (docnum == 0)
                {
                    docnum = 1;
                }
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    // MessageBox.Show("type " + int.Parse(row.Cells[8].Value.ToString(), NumberStyles.AllowThousands, new CultureInfo("en-au")));

                    int summa = Convert.ToInt32(row.Cells[8].Value.ToString().Replace(".", ""));

                    worksheet.Cells[cellRowIndex, 1] = docnum.ToString();//1 ndoc
                    worksheet.Cells[cellRowIndex, 2].NumberFormat = "DD.MM.YYYY";
                    worksheet.Cells[cellRowIndex, 2] = DateTime.Now.Date; //2 dt                   
                    worksheet.Cells[cellRowIndex, 3].NumberFormat = "DD.MM.YYYY";
                    worksheet.Cells[cellRowIndex, 3] = DateTime.Now.Date; //3 dv
                    worksheet.Cells[cellRowIndex, 4] = row.Cells[6].Value; //4 acccli
                    worksheet.Cells[cellRowIndex, 5] = row.Cells[7].Value.ToString(); //5 acccor
                    worksheet.Cells[cellRowIndex, 6] = row.Cells[12].Value.ToString(); //6 okpocor
                    worksheet.Cells[cellRowIndex, 7] = row.Cells[10].Value.ToString(); //7 namecor
                    worksheet.Cells[cellRowIndex, 8].NumberFormat = "0"; //8 summa ThisRange.NumberFormat = "0.00%";
                    worksheet.Cells[cellRowIndex, 8] = summa; //8 summa
                    worksheet.Cells[cellRowIndex, 9].NumberFormat = "0";  //9 val
                    worksheet.Cells[cellRowIndex, 9] = codeVal; //9 val
                    worksheet.Cells[cellRowIndex, 10] = row.Cells[11].Value.ToString().Trim(); //10 nazn

                    worksheet.Cells[cellRowIndex, 11].NumberFormat = "0";  //cod_cor 11 
                    worksheet.Cells[cellRowIndex, 11] = countryCode; //cod_cor 11
                    worksheet.Cells[cellRowIndex, 12] = ""; //add_req 12
                    cellRowIndex++;
                    docnum++;
                    progressBar1.PerformStep();
                }


                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Експорт завершено", "Інформація", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                progressBar1.Value = 1;
                progressBar1.Visible = false;

            }
            catch (Exception ex)
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

        public void SaveExcel(SaveFileDialog saveDialog, DataGridView dataGridView1N, bool anotherPay, bool isUkrGaz = false) //створюємо файл імпорту для укргаз банка
        {
            // Creating a Excel object.
            _Application excel = new Microsoft.Office.Interop.Excel.Application();
            _Workbook workbook = excel.Workbooks.Add(Type.Missing);
            _Worksheet worksheet = null;
            try
            {
                progressBar1.Visible = true;
                ModifyProgressBarColor.SetState(progressBar1, 3);
                progressBar1.Minimum = 1;
                progressBar1.Maximum = dataGridView1N.Rows.Count + 1;
                progressBar1.Value = 1;
                progressBar1.Step = 1;

                worksheet = workbook.ActiveSheet;
                worksheet.Rows.NumberFormatLocal = "@";
                worksheet.Columns.NumberFormatLocal = "@";
                worksheet.Name = "ExportedFromDatGrid";

                int cellRowIndex = 1;


                for (int i = 0; i <= dataGridView1N.Rows.Count; i++) // todo: 
                {

                    if (cellRowIndex == 1)
                    {

                        worksheet.Cells[1, 1].Value = "FIELD_CUST_BANK_CODE";
                        worksheet.Cells[1, 2].Value = "FIELD_CUST_ACCOUNT";
                        worksheet.Cells[1, 3].Value = "FIELD_CUST_IBAN";
                        worksheet.Cells[1, 4].Value = "FIELD_BENEF_BANK_CODE";
                        worksheet.Cells[1, 5].Value = "FIELD_BENEF_ACCOUNT";
                        worksheet.Cells[1, 6].Value = "FIELD_BENEF_IBAN";
                        worksheet.Cells[1, 7].Value = "FIELD_OPERATION_TYPE";
                        worksheet.Cells[1, 8].Value = "FIELD_AMOUNT";
                        worksheet.Cells[1, 9].Value = "FIELD_DOCUMENT_TYPE";
                        worksheet.Cells[1, 10].Value = "FIELD_NUMBER";
                        worksheet.Cells[1, 11].Value = "FIELD_CURRENCY_NUMBER";
                        worksheet.Cells[1, 12].Value = "FIELD_DOCUMENT_DATE";
                        worksheet.Cells[1, 13].Value = "FIELD_VALUE_DATE";
                        worksheet.Cells[1, 14].Value = "FIELD_CUST_NAME";
                        worksheet.Cells[1, 15].Value = "FIELD_BENEF_NAME";
                        worksheet.Cells[1, 16].Value = "FIELD_PURPOSE";
                        worksheet.Cells[1, 17].Value = "FIELD_ADDITIONAL_DATA";
                        worksheet.Cells[1, 18].Value = "FIELD_PURPOSE_CODE";
                        worksheet.Cells[1, 19].Value = "FIELD_EMPTY_COLUMN";
                        worksheet.Cells[1, 20].Value = "FIELD_CUST_TAX_CODE";
                        worksheet.Cells[1, 21].Value = "FIELD_BENEF_TAX_CODE";
                        worksheet.Cells[1, 22].Value = "FIELD_EXT_DOCUMENT_NUMBER";
                        worksheet.Cells[1, 23].Value = "FIELD_VAT_TYPE";
                    }
                    else
                    {
                        
                            worksheet.Cells[cellRowIndex, 1] = 320478;//FIELD_CUST_BANK_CODE
                        worksheet.Cells[cellRowIndex, 2] = isUkrGaz ? ukrGaz.rahunok : dataGridView1N.Rows[i - 1].Cells[3].Value.ToString();//FIELD_CUST_ACCOUNT
                        worksheet.Cells[cellRowIndex, 3] = textIban.Text;//FIELD_CUST_IBAN
                        try
                            {
                                worksheet.Cells[cellRowIndex, 4] = dataGridView1N.Rows[i - 1].Cells[5].Value.ToString();//FIELD_BENEF_BANK_CODE


                        }
                            catch { }
                            try
                            {
                                worksheet.Cells[cellRowIndex, 6] = isUkrGaz ? dataGridView1N.Rows[i - 1].Cells[7].Value.ToString() : dataGridView1N.Rows[i - 1].Cells[6].Value.ToString();//FIELD_BENEF_IBAN

                        }
                            catch { }
                            try
                            {
                                worksheet.Cells[cellRowIndex, 8] = isUkrGaz ? dataGridView1N.Rows[i - 1].Cells[8].Value.ToString().Replace(".",",") : dataGridView1N.Rows[i - 1].Cells[0].Value.ToString();//FIELD_AMOUNT
                        }
                            catch { }

                            try
                            {
                                worksheet.Cells[cellRowIndex, 11] = isUkrGaz ? "UAH" : dataGridView1N.Rows[i - 1].Cells[1].Value.ToString();//FIELD_CURRENCY_NUMBER
                        }
                            catch { }
                        try
                        {
                            worksheet.Cells[cellRowIndex, 15] = isUkrGaz ? dataGridView1N.Rows[i - 1].Cells[10].Value.ToString():  dataGridView1N.Rows[i - 1].Cells[8].Value.ToString();//FIELD_BENEF_NAME
                        }
                        catch { }
                        //FIELD_PURPOSE
                        try
                        {
                            if (isUkrGaz && !anotherPay)
                            {
                                string addPaym = dataGridView1N.Rows[i - 1].Cells[11].Value.ToString();

                                if (dataGridView1N.Rows[i - 1].Cells[13].Value == null)
                                {

                                    dataGridView1N.Rows[i - 1].Cells[13].Value = string.Empty;

                                }
                                addPaym = string.Join(" ", addPaym, dataGridView1N.Rows[i - 1].Cells[13].Value.ToString());
                                worksheet.Cells[cellRowIndex, 16] = addPaym;
                            }else if (isUkrGaz && anotherPay)
                            {
                                if (dataGridView1N.Rows[i - 1].Cells[13].Value == null)
                                {

                                    dataGridView1N.Rows[i - 1].Cells[13].Value = string.Empty;

                                }
                                
                                worksheet.Cells[cellRowIndex, 16] = string.Join(" ", dataGridView1N.Rows[i - 1].Cells[13].Value.ToString(), dataGridView1N.Rows[i - 1].Cells[11].Value.ToString());
                            }
                            else
                            {
                                worksheet.Cells[cellRowIndex, 16] = anotherPay ? string.Join(" ", dataGridView1N.Rows[i - 1].Cells[10].Value.ToString(), dataGridView1N.Rows[i - 1].Cells[2].Value.ToString()) : dataGridView1N.Rows[i - 1].Cells[2].Value.ToString();
                            }
                            

                            
                        }
                            catch { }
                          
                            try
                            {
                                worksheet.Cells[cellRowIndex, 20] = isUkrGaz ? ukrGaz.edrpou : dataGridView1N.Rows[i - 1].Cells[4].Value.ToString();//FIELD_CUST_TAX_CODE
                        }
                            catch { }

                            try
                            {
                                worksheet.Cells[cellRowIndex, 21] = isUkrGaz ?  dataGridView1N.Rows[i - 1].Cells[12].Value.ToString() : dataGridView1N.Rows[i - 1].Cells[7].Value.ToString();//FIELD_BENEF_TAX_CODE

                        }
                            catch { }
                       

                    }



                    cellRowIndex++;
                    progressBar1.PerformStep();
                }




                {
                    workbook.SaveAs(saveDialog.FileName);
                    MessageBox.Show("Експорт завершено", "Інформація", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    progressBar1.Value = 1;
                    progressBar1.Visible = false;
                }
            }
            catch (Exception ex)
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

            DataGridView dataGrid = dataGridView2.Visible ? dataGridView2 : dataGridView1;

            if (dataGrid.Rows.Count > 0)
            {
                if (comboEdr.SelectedIndex == 0 || (comboEdr2.SelectedIndex == 0 && dataGridView1.Visible))// Аваль || УКрГаз
                {
                    SaveExcel(dataGrid, comboEdr.SelectedIndex);
                    try
                    {
                        SaveXml();
                    }
                    catch { }

                }
                else if (comboEdr.SelectedIndex == 2)//ощад
                {
                    SaveExcel(dataGrid, comboEdr.SelectedIndex, oschad.rahunok);
                    //SaveOschadDbf();
                }
                else if (comboEdr.SelectedIndex == 3)//пумб
                {
                    try
                    {
                        //if (SavePumbDbf(out string path, anotherPay.Checked))
                        //{
                        //    MessageBox.Show(($"Файл збережено!{Environment.NewLine}{path}"),"Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //}
                        if (SavePumbDbf2(out string path2, anotherPay.Checked))
                        {
                            MessageBox.Show(($"Файл збережено!{Environment.NewLine}{path2}"), "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }

                    }
                    catch (Exception ex) { MessageBox.Show(($"Файл не збережено!"), "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); MessageBox.Show("Файл не збережено!" + Environment.NewLine + ex.Message); }

                }
                else if (comboEdr.SelectedIndex == 4)//Кошти отримані через ПУМБ формувати для Укргазбанку
                {
                    SaveExcel(dataGrid, 0, "", true);

                }
                // Save();
            }
            else
            {
                MessageBox.Show(($"Дані відсутні!"), "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

        }


        public void IsEditAval(bool edit)
        {
            erdpo1l.Visible = erdpo1.Visible = cliBankCode.Visible = rahunok.Visible = mfo.Visible =
                  rahunok.Visible = label1.Visible = label2.Visible = label5.Visible = edit;
        }

        public void IsEditUkrG(bool edit)
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
            Settings.Default.Save();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            Form frm;
            if (anotherPay.Checked)
            {
                frm = new Form2(path3, 3);
            }
            else
            {
                frm = new Form2(path2, 2);
            }

            dr = frm.ShowDialog();

            if (dr == DialogResult.OK)
            {
                if (anotherPay.Checked)
                {
                    AutoOpenCsv(path3, 3);
                }
                else
                {
                    AutoOpenCsv(path2, 2);
                }
            }
        }


        private void Button3_Click(object sender, EventArgs e)
        {
            editAval = !editAval;
            FIOL.Visible = FIO.Visible = !editAval;
            if (editAval)
            {
                comboEdr.Enabled = false;
                IsEditAval(editAval);
                button3.Image = saveBtn;
            }
            else
            {
                switch (comboEdr.SelectedIndex)
                {
                    case 0:
                        button3.Image = editBtn;
                        IsEditAval(editAval);
                        aval.name = comboEdr.Text;
                        comboEdr.Items.Clear();
                        comboEdr.Items.Add("Райффайзен Банк Аваль");
                        comboEdr.Items.Add("Індустріал");
                        comboEdr.Items.Add("Ощадбанк");
                        comboEdr.Items.Add("Пумб");
                        comboEdr.Items.Add("УкрГаз");
                        aval.name = comboEdr.Text;
                        aval.mfo = mfo.Text;
                        aval.rahunok = rahunok.Text;
                        aval.clientBankCode = cliBankCode.Text;
                        aval.edrpou = erdpo1.Text;
                        aval.id = 0;
                        // Xml.EditXml(aval, pathConfig);
                        PayConverterConfig_.UpdateByBankId(new PayConverterConfig(aval));
                        comboEdr.Enabled = true;
                        InitData();
                        break;
                    case 1:
                        button3.Image = editBtn;
                        IsEditAval(editAval);
                        aval.name = comboEdr.Text;
                        comboEdr.Items.Clear();
                        comboEdr.Items.Add("Райффайзен Банк Аваль");
                        comboEdr.Items.Add("Індустріал");
                        comboEdr.Items.Add("Ощадбанк");
                        comboEdr.Items.Add("Пумб");
                        comboEdr.Items.Add("УкрГаз");
                        aval.name = comboEdr.Text;
                        aval.mfo = mfo.Text;
                        aval.edrpou = erdpo1.Text;
                        aval.rahunok = rahunok.Text;
                        aval.clientBankCode = cliBankCode.Text;
                        aval.id = 2;
                        // Xml.EditXml(aval, pathConfig);
                        PayConverterConfig_.UpdateByBankId(new PayConverterConfig(aval));
                        comboEdr.Enabled = true;
                        InitData();
                        break;
                    case 2:
                        button3.Image = editBtn;
                        IsEditAval(editAval);
                        aval.name = comboEdr.Text;
                        comboEdr.Items.Clear();
                        comboEdr.Items.Add("Райффайзен Банк Аваль");
                        comboEdr.Items.Add("Індустріал");
                        comboEdr.Items.Add("Ощадбанк");
                        comboEdr.Items.Add("Пумб");
                        comboEdr.Items.Add("УкрГаз");
                        aval.name = comboEdr.Text;
                        aval.mfo = mfo.Text;
                        aval.edrpou = erdpo1.Text;
                        aval.rahunok = rahunok.Text;
                        aval.clientBankCode = cliBankCode.Text;
                        aval.id = 3;
                        // Xml.EditXml(aval, pathConfig);
                        PayConverterConfig_.UpdateByBankId(new PayConverterConfig(aval));
                        comboEdr.Enabled = true;
                        InitData();
                        break;
                    case 3:
                        button3.Image = editBtn;
                        IsEditAval(editAval);
                        aval.name = comboEdr.Text;
                        comboEdr.Items.Clear();
                        comboEdr.Items.Add("Райффайзен Банк Аваль");
                        comboEdr.Items.Add("Індустріал");
                        comboEdr.Items.Add("Ощадбанк");
                        comboEdr.Items.Add("Пумб");
                        comboEdr.Items.Add("УкрГаз");
                        aval.name = comboEdr.Text;
                        aval.mfo = mfo.Text;
                        aval.edrpou = erdpo1.Text;
                        aval.rahunok = rahunok.Text;
                        aval.clientBankCode = cliBankCode.Text;
                        aval.id = 4;
                        //Xml.EditXml(aval, pathConfig);
                        PayConverterConfig_.UpdateByBankId(new PayConverterConfig(aval));
                        comboEdr.Enabled = true;
                        InitData();
                        break;
                }


            }
        }

        private void Button5_Click_2(object sender, EventArgs e)
        {
            editUkrG = !editUkrG;

            if (editUkrG)
            {
                comboEdr2.Enabled = false;

                IsEditUkrG(editUkrG);
                button5.Image = saveBtn;
            }
            else
            {
                button5.Image = editBtn;
                IsEditUkrG(editUkrG);
                comboEdr2.Enabled = true;

                ukrGaz.edrpou = textBox2.Text;
                ukrGaz.rahunok = textBox4.Text;
                ukrGaz.iban = textIban.Text;
                ukrGaz.name = comboEdr2.Text;
                ukrGaz.id = 1;
                //Xml.EditXml(ukrGaz, pathConfig);
                PayConverterConfig_.UpdateByBankId(new PayConverterConfig(ukrGaz));
            }
        }

        public void SetFieldsUkrGaz()
        {
            textBox2.Text = ukrGaz.edrpou;
            textBox4.Text = ukrGaz.rahunok;
            textIban.Text = ukrGaz.iban;
        }

        private void ComboEdr2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFieldsUkrGaz();
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
            gridHeader.Text = NameBank1.Text;
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


        private void DataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
                    //        Xml.SaveXml(dataGridView3, path2);
                    //    }
                    //}
                }
                if (selColNum == 13)
                {
                    string OrgName = dataGridView2.CurrentRow.Cells[10].Value.ToString();
                    dataGridView2.CurrentRow.Cells[13].Value =
                       MyDataGrid.shortText(dataGridView2.CurrentRow.Cells[13].Value.ToString());
                    string purpose = dataGridView2.CurrentRow.Cells[13].Value.ToString().Replace("  ", @" ").Trim();
                    dataGridView2.CurrentRow.Cells[13].Value = purpose;

                    var dialogResult = MessageBox.Show("Оновити довідник?", "Додати/Оновити довідник Призначення за умовчанням", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        PurposeOfPayment_.InsertOrUpdatePurpose(OrgName, purpose);
                    }

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {

                        if (row.Cells[10].Value.ToString().Equals(OrgName))
                        {
                            row.Cells[13].Value = purpose;
                        }
                    }
                }
            }
            else
            {
                dataGridView2[e.ColumnIndex, e.RowIndex].Value = string.Empty;
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
                    //        Xml.SaveXml(dataGridView3, path2);
                    //    }
                    //}
                }
                if (selColNum == 10)
                {
                    string OrgName = dataGridView1.CurrentRow.Cells[8].Value.ToString();
                    string purpose =
                       MyDataGrid.shortText(dataGridView1.CurrentRow.Cells[10].Value.ToString());
                    purpose = purpose.Replace("  ", @" ").Trim();
                    dataGridView2.CurrentRow.Cells[10].Value = purpose;

                    var dialogResult = MessageBox.Show("Оновити довідник?", "Додати/Оновити довідник Призначення за умовчанням", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        PurposeOfPayment_.InsertOrUpdatePurpose(OrgName, purpose);
                    }

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {

                        if (row.Cells[8].Value.ToString().Equals(OrgName))
                        {
                            row.Cells[10].Value = purpose;
                        }
                    }
                }
            }
            else
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = string.Empty;
            }
        }

        private void DataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            currentCellValue = dataGridView1.CurrentRow.Cells[2].Value.ToString();
        }

        private void DataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            currentCellValue = dataGridView2.CurrentRow.Cells[11].Value.ToString();
        }

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            string rowIdx = (e.RowIndex + 1).ToString();

            StringFormat centerFormat = new StringFormat
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds =
                new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void DataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            string rowIdx = (e.RowIndex + 1).ToString();

            StringFormat centerFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds =
                new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
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
            Form frm = new Form3
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            frm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " " + localVersion;
            new Update().Download();
            new Update().DownloadTemplate();

        }

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void GridHeader_Click(object sender, EventArgs e)
        {
        }

        private void AnotherPay_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.DataSource = "";
            path = string.Empty;
            InitPData();
        }

        private void ComboEdr_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            switch (comboEdr.SelectedIndex)
            {
                case 0:
                    gridHeader.Text = NameBank1.Text = "АВАЛЬ";
                    docNumOschadL.Visible = docNumOschad.Visible = false;
                    break;
                case 1:
                    gridHeader.Text = NameBank1.Text = "ІНДУСТРІАЛ";
                    docNumOschadL.Visible = docNumOschad.Visible = false;
                    break;
                case 2:
                    gridHeader.Text = NameBank1.Text = "ОЩАДБАНК";
                    docNumOschadL.Visible = docNumOschad.Visible = true;
                    docNumOschad.Text = "1";
                    break;
                case 3:
                    gridHeader.Text = NameBank1.Text = "ПУМБ";
                    docNumOschadL.Visible = docNumOschad.Visible = true;
                    docNumOschad.Text = "1";
                    break;
                case 4:
                    gridHeader.Text = NameBank1.Text = "УКРГАЗ";
                    docNumOschadL.Visible = docNumOschad.Visible = false;
                    docNumOschad.Text = "1";

                    break;
            }

            FIOL.Visible = FIO.Visible = !editAval && comboEdr.SelectedIndex == 3;

        }



        private void button6_Click(object sender, EventArgs e)
        {

            DialogResult dr = new DialogResult();
            Form frm;
            frm = new Form4();
            dr = frm.ShowDialog();
        }
    }
}