using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace SoftGenConverter
{
    class Aval
    {
        
        public string name { get; set; }
        public int isAval { get; set; }
        public int datePayment { get; set; }
        public string mfo { get; set; }
        public string rahunok { get; set; }
        public string zkpo { get; set; }
        //public string cliBankCode { get; set; }
        public string recivPayNum { get; set; }
        public string summa { get; set; }
        public DateTime dateP { get; set; }

        public void piece(string line, DateTime date, bool aval)
        {



            {
                string[] parts = line.Split(';');  //Разделитель в CSV файле.
                if (aval)
                {
                    name = parts[0].ToUpper();
                    mfo = parts[2];
                    rahunok = ""+ Convert.ToInt64(parts[3]);
                    zkpo = parts[4];
                    dateP = date;
                    summa = parts[8];
                    isAval = 1;
                }
                else
                {
                    name = parts[0].ToUpper();
                    mfo = parts[1];
                    rahunok = ""+ Convert.ToInt64(parts[2]);
                    zkpo = parts[3];
                    summa = parts[5];
                    isAval = 0;
                    dateP = date;
                }
                
            }


        }
        public static List<Aval> ReadFile(string filename)
        {
            List<Aval> res = new List<Aval>();
            int date = Properties.Settings.Default.datePayment;
            Regex regexDate = new Regex(@"\w*[0-9]{2}[.][0-9]{2}[.][0-9]{2}р.");
            Regex regexLine = new Regex(@".+;.*;.+;.+;.+;.+;.*;.*;.+;.*");
            bool flag = false;
            bool aval = false;
        DateTime datePl = DateTime.Today;
            try
            {
                using (StreamReader sr = new StreamReader(filename, Encoding.GetEncoding(1251)))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                MatchCollection dateMatch = regexDate.Matches(line);
                                if (dateMatch.Count > 0)
                                {
                                    CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                   
                                    MatchCollection matchess = Regex.Matches(line, regexDate.ToString(), RegexOptions.IgnoreCase);
                                    date = Int32.Parse(matchess[0].ToString().Replace("за", "").Replace("р.", "").Trim()
                                        .Replace(".", ""));
                                    datePl = DateTime.Parse(matchess[0].ToString().Replace("за", "").Replace("р.", "").Trim(),
                                        MyCultureInfo);
                   
                                }
                            }
                        }

            }
            catch (Exception) { }

            try
            {
        using (StreamReader sr = new StreamReader(filename, Encoding.GetEncoding(1251)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    MatchCollection dateMatch = regexDate.Matches(line);
                    if (dateMatch.Count > 0)
                    {
                        
                        flag = false;
                        aval = true;
                    }

                    
                    MatchCollection lineMatch = regexLine.Matches(line);
                    //MessageBox.Show(lines2);
                    if (lineMatch.Count > 0)
                    {
                        if (flag)
                        {
                            //MessageBox.Show(line);
                            if ((line.IndexOf("з банку \"АВАЛЬ\"")) > 0)
                            {
                               
                                flag = false;
                                aval = true;
                            }
                            Aval p = new Aval();
                            p.piece(line, datePl, aval);
                            res.Add(p);
                        }

                        flag = true;
                    }


                }
            }

            }
            catch (Exception) { }

            return res;
        }
        public static void StyleDataGridView(DataGridView dgv, bool isReadonly = true)
        {
            try
            {
                // Setting the style of the DataGridView control
                dgv.RowHeadersVisible = true;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold, GraphicsUnit.Point);
                dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.ControlDark;
                dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.DefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Regular, GraphicsUnit.Point);
                dgv.DefaultCellStyle.BackColor = Color.Empty;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dgv.AllowUserToAddRows = false;
                dgv.ReadOnly = isReadonly;
                //dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
                System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
                dataGridViewCellStyle1.BackColor = Color.LightBlue;
                dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            }
            catch (Exception )
            {
            }
        }
        public static string shortText(string str)
        {
            string pattern = @"будин\S+";
            string pattern2 = @"комунал\S+";
            string pattern3 = @"послуг\S+";
            string pattern4 = @"утриман\S+";
            string pattern5 = @"управл\S+";
            
            
            str = Regex.Replace(str.Trim(), @"[^\S\r\n]+", " ");
            str = Regex.Replace(str, pattern, "буд. ");
            str =Regex.Replace(str, pattern2, "комун. ");
            str =Regex.Replace(str, pattern3, "посл. ");
            str =Regex.Replace(str, pattern4, "утрим. ");
            str =Regex.Replace(str, pattern5, "управл. ");
          
           // str = str.Replace("  ", @" ").Replace("i","і");
            
            return str;
        }
        public static void Filter(DataGridView dataGridView1, string foundText, int[] col)
        {
            string textF = foundText.Trim().ToLower();

            for (int i = 0; i < dataGridView1.RowCount; i++)
                
                if (dataGridView1[col[0], i].FormattedValue.ToString().ToLower().
                        Contains(textF) || dataGridView1[col[1], i].FormattedValue.ToString().ToLower().
                        Contains(textF) || dataGridView1[col[2], i].FormattedValue.ToString().ToLower().
                        Contains(textF) || dataGridView1[col[3], i].FormattedValue.ToString().ToLower().
                        Contains(textF))
                {
                    //MessageBox.Show(dataGridView1[1, i].FormattedValue.ToString());
                    dataGridView1.Rows[i].Selected = true;
                    dataGridView1.Rows[i].Visible = true;


                }
                else
                {
                    dataGridView1.Rows[i].Visible = false;
                    dataGridView1.Rows[i].Selected = false;

                }
            if (string.IsNullOrEmpty(foundText)) dataGridView1.ClearSelection();

        }
        public static string convertDate(string text)
        {
            string pattern = @"за\s?[0-9]{2}[.][0-9]{2}[.][0-9]{4}\s*р?.?";
             
            return Regex.Replace(text, pattern, "  за ##.##.#### "); 
        }
    }
}
