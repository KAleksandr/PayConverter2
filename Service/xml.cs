using SoftGenConverter.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace SoftGenConverter
{
    internal static class Xml
    {
        public static void loadXml(DataGridView dataGridView1, string path)
        {
            if (dataGridView1.Rows.Count > 0) dataGridView1.Rows.Clear();

            if (File.Exists(path)) //
            {
                var ds = new DataSet();
                ds.ReadXml(path);
                try
                {
                    foreach (DataRow item in ds.Tables["Employee"].Rows)
                    {
                        var n = dataGridView1.Rows.Add();
                        dataGridView1.Rows[n].Cells[0].Value = item["NAME"];
                        dataGridView1.Rows[n].Cells[1].Value = item["ERDPO"];

                        dataGridView1.Rows[n].Cells[2].Value = item["RRahunok"];
                        dataGridView1.Rows[n].Cells[3].Value = MyDataGrid.shortText(item["Comment"].ToString());
                        if (dataGridView1.Rows[n].Cells[3].Value.Equals("null"))
                            dataGridView1.Rows[n].DefaultCellStyle.BackColor = Color.BurlyWood;
                    }
                }
                catch{}
            }            
        }

        public static void isExistsFile(string path, string text)
        {
            var file = path = path.Remove(0, path.LastIndexOf("\\") + 1);           

            if (!File.Exists(path)) //
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);                
                var outStream = File.CreateText(file);
                doc.Save(outStream);
                outStream.Close();
                Thread.Sleep(300);
                MessageBox.Show(
                    file + " файл не знайдений!" + Environment.NewLine + " Файл створено з конфігурації програми.",
                    "Помилка.", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            if (!File.Exists(Db.runningPath))
            {
                MessageBox.Show("Ok");                
                MessageBox.Show(
                    Db.runningPath + " файл не знайдений!" + Environment.NewLine + " Файл створено з конфігурації програми.",
                    "Помилка.", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Db.CreateDb();                
            }
        }

        public static void ReWriteFile(string path)
        {
            var file = path = path.Remove(0, path.LastIndexOf("\\") + 1);
            if (!File.Exists(path)) //
                File.Delete(path);
        }

        public static List<Bank> loadXml(string path)
        {
            var ds = new DataSet();
            var config = new List<Bank>();

            if (File.Exists(path)) //
            {
                ds.ReadXml(path);
                try
                {
                    foreach (DataRow item in ds.Tables["Bank"].Rows)
                    {
                        var bank = new Bank
                        {
                            name = item["NAME"].ToString(),
                            mfo = item["MFO"].ToString(),
                            edrpou = item["edrpou"].ToString(),
                            rahunok = item["RRAHUNOK"].ToString(),
                            clientBankCode = item["clientBankCode"].ToString(),
                            iban = item["IBAN"].ToString()
                        };
                        try
                        {
                            bank.id = Convert.ToInt32(item["STATE"]);
                        }
                        catch{}
                        finally
                        {
                            config.Add(bank);
                        }
                    }
                }
                catch{}
            }
            return config;
        }

        public static void saveXml(DataGridView dataGridView, string path)
        {
            try
            {
                var ds = new DataSet(); 
                var dt = new DataTable
                {
                    TableName = "Employee" 
                }; 
                dt.Columns.Add("NAME"); 
                dt.Columns.Add("ERDPO"); 
                dt.Columns.Add("RRahunok");
                dt.Columns.Add("Comment");

                ds.Tables.Add(dt); 

                foreach (DataGridViewRow r in dataGridView.Rows) 
                    if (r.Cells != null)
                    {
                        var row = ds.Tables["Employee"].NewRow();
                        row["Name"] = r.Cells[0].Value;
                        row["ERDPO"] =
                            r.Cells[1].Value; 
                        row["Comment"] = r.Cells[3].Value; 
                        row["RRahunok"] = r.Cells[2].Value;

                        ds.Tables["Employee"].Rows.Add(row);
                    }

                ds.WriteXml(path);
               
            }
            catch { }
        }        
        public static Bank[] ReadXml(string fileName)
        {
            var bank1 = new Bank();
            var bank2 = new Bank();
            var bank3 = new Bank();
            var bank4 = new Bank();
            var bank5 = new Bank();           
            var doc = XDocument.Load(fileName);
           
            foreach (var el in doc.Root.Elements())
                if (Convert.ToInt32(el.Attribute("id").Value) == 0)
                    bank1 = FillBank(el);
                else if (Convert.ToInt32(el.Attribute("id").Value) == 1)
                    bank2 = FillBank(el);
                else if (Convert.ToInt32(el.Attribute("id").Value) == 2)
                    bank3 = FillBank(el);
                else if (Convert.ToInt32(el.Attribute("id").Value) == 3) bank4 = FillBank(el);
                else if (Convert.ToInt32(el.Attribute("id").Value) == 4) bank5 = FillBank(el);
            Bank[] banks = {bank1, bank2, bank3, bank4, bank5};
            return banks;
        }

        public static Bank FillBank(XElement el)
        {
            var bank = new Bank();
            foreach (var element in el.Elements())
            {
                if (element.Name.ToString().Equals("NAME")) bank.name = element.Value;
                if (element.Name.ToString().Equals("RAHUNOK"))
                    bank.rahunok = element.Value;
                else if (element.Name.ToString().Equals("MFO"))
                    bank.mfo = element.Value;
                else if (element.Name.ToString().Equals("EDRPOU"))
                    bank.edrpou = element.Value;
                else if (element.Name.ToString().Equals("EDRPOU"))
                    bank.edrpou = element.Value;
                else if (element.Name.ToString().Equals("clientBankCode"))
                    bank.clientBankCode = element.Value;
                else if (element.Name.ToString().Equals("IBAN")) bank.iban = element.Value;
            }
            return bank;
        }

        public static void EditXml(Bank bank, string fileName)
        {
            var doc = XDocument.Load(fileName);
            
            foreach (var el in doc.Root.Elements("bank"))
            {
                var id = int.Parse(el.Attribute("id").Value);               
                if (id == 0 && bank.id == 0)
                {
                    el.SetElementValue("NAME", bank.name);
                    el.SetElementValue("RAHUNOK", bank.rahunok);
                    el.SetElementValue("MFO", bank.mfo);
                    el.SetElementValue("EDRPOU", bank.edrpou);
                    el.SetElementValue("clientBankCode", bank.clientBankCode);
                }
                else if (id == 1 && bank.id == 1)
                {
                    el.SetElementValue("NAME", bank.name);
                    el.SetElementValue("RAHUNOK", bank.rahunok);
                    el.SetElementValue("MFO", bank.mfo);
                    el.SetElementValue("EDRPOU", bank.edrpou);
                    el.SetElementValue("IBAN", bank.iban);
                    el.SetElementValue("clientBankCode", bank.clientBankCode);
                }
                else if (id == 2 && bank.id == 2)
                {
                    el.SetElementValue("NAME", bank.name);
                    el.SetElementValue("RAHUNOK", bank.rahunok);
                    el.SetElementValue("MFO", bank.mfo);
                    el.SetElementValue("EDRPOU", bank.edrpou);
                    el.SetElementValue("clientBankCode", bank.clientBankCode);
                }
                else if (id == 3 && bank.id == 3)
                {                    
                    el.SetElementValue("NAME", bank.name);
                    el.SetElementValue("RAHUNOK", bank.rahunok);
                    el.SetElementValue("MFO", bank.mfo);
                    el.SetElementValue("EDRPOU", bank.edrpou);
                    el.SetElementValue("clientBankCode", bank.clientBankCode);
                }
                else if (id == 4 && bank.id == 4)
                {                    
                    el.SetElementValue("NAME", bank.name);
                    el.SetElementValue("RAHUNOK", bank.rahunok);
                    el.SetElementValue("MFO", bank.mfo);
                    el.SetElementValue("EDRPOU", bank.edrpou);
                    el.SetElementValue("clientBankCode", bank.clientBankCode);
                }
            }
            doc.Save(fileName);
        }       
    }
}