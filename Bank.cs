using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SoftGenConverter
{
    public class Bank
    {

        public string name { get; set; }
        public int id { get; set; }
        /// <summary>
        /// ot
        /// </summary>
        public string mfo { get; set; }
        public string rahunok { get; set; }
        public string edrpou { get; set; }
        //public string cliBankCode { get; set; }
        public string clientBankCode { get; set; }
        public string summa { get; set; }
        public DateTime dateP { get; set; }

        public override string ToString()
        {
            return name + " " + rahunok;
        }

        public void piece(string line, DateTime date, bool aval)
        {
                

                string[] parts = line.Split(';');  //Разделитель в CSV файле.
                if (aval)
                {
                    name = parts[0].ToUpper();
                    mfo = parts[2];
                    rahunok = "" + parts[3];
                     //rahunok = "" + Convert.ToInt64(parts[3]);
                    edrpou = parts[4];
                    dateP = date;
                    summa = parts[8];
                    id = 1;
                //MessageBox.Show("Name"+ name+" Rahunok"+ rahunok);
                }
                else
                {
                    name = parts[0].ToUpper();
                    mfo = parts[1];
                    rahunok = "" + Convert.ToInt64(parts[2]);
                    edrpou = parts[3];
                    summa = parts[5];
                    id = 0;
                    dateP = date;
                }

            


        }
        public static List<Bank> ReadFile(string filename)
        {
            List<Bank> res = new List<Bank>();
            int date = 0;
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
                                if ((line.IndexOf("з банку \"АВАЛЬ\"")) > 0)
                                {
                                    flag = false;
                                    aval = true;
                                }
                                Bank p = new Bank();
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
        
    }
}
