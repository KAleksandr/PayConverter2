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
        public string iban { get; set; }
        public string edrpou { get; set; }
        //public string cliBankCode { get; set; }
        public string clientBankCode { get; set; }
        public string summa { get; set; }
        public string pruznach {get; set;}
        public DateTime dateP { get; set; }

        public override string ToString()
        {
            return name + " " + rahunok;
        }

        public void piece(string line, DateTime date, bool aval, bool anotherPay)
        {
                

                string[] parts = line.Split(';');  //Разделитель в CSV файле.
                if (aval)
                {
                    if (!anotherPay)
                    {
                        name = parts[0].ToUpper();
                        mfo = parts[2];
                        rahunok = "" + parts[3];
                         //rahunok = "" + Convert.ToInt64(parts[3]);
                        edrpou = parts[4];
                        dateP = date;
                        summa = parts[8];
                        pruznach = parts[1];
                        id = 1;
                    }
                    else
                    {
                        name = parts[10].ToUpper();
                        mfo = parts[2];
                        rahunok = "" + parts[3];
                            //rahunok = "" + Convert.ToInt64(parts[3]);
                        edrpou = parts[4];
                        dateP = date;
                        summa = parts[8];
                        pruznach = parts[0] + " " + parts[1];
                        id = 1;
                    }

                //MessageBox.Show("Name"+ name+" Rahunok"+ rahunok);
                }
                else
                {
                    if (!anotherPay)
                    {
                        name = parts[10].ToUpper();
                        pruznach = parts[1];
                        mfo = parts[2];
                        rahunok = "" + parts[3];
                        //rahunok = "" + Convert.ToInt64(parts[2]);
                        edrpou = parts[4];
                        summa = parts[6];
                        id = 0;
                        dateP = date;
                    }
                    else
                    {
                        name = parts[10].ToUpper();
                        pruznach = parts[0];
                        mfo = parts[1];
                        rahunok = "" + parts[2];
                        //rahunok = "" + Convert.ToInt64(parts[2]);
                        edrpou = parts[3];
                        summa = parts[5];
                        id = 0;
                        dateP = date;
                    }
                    
                    
                
                }

            

                //todo: regex
        }
        public static List<Bank> ReadFile(string filename, bool anotherPay)
        {
            List<Bank> res = new List<Bank>();
            int date = 0;
            Regex regexDate = new Regex(@"\w*([0-9]{2}[.][0-9]{2}[.][0-9]{2}р.)");
            Regex regexLine = new Regex(@".+;.*;.+;.+;.+;.+;.*;.*;.+;.*");
            if (anotherPay)
            {
                regexDate = new Regex(@"\w*([0-9]{2}[.][0-9]{2}[.][0-9]{2}р.)");
                regexLine = new Regex(@".+;.*;.+;.+;.+;.+;.*;.*;.+;.*;*");
            }
            bool flag = false;
            bool aval = false;
            DateTime datePl = DateTime.Today;

           // string pattern = @"\w*([0-9]{2}[.][0-9]{2}[.][0-9]{2}р.)";

           // File.WriteAllText(filename, Regex.Replace(File.ReadAllText(filename, Encoding.GetEncoding(1251)), pattern, " "), Encoding.GetEncoding(1251));

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
                    {   MatchCollection lineMatch = regexLine.Matches(line);
                        MatchCollection dateMatch = regexDate.Matches(line);
                        if (dateMatch.Count > 0 && line.Length == 24)
                        {
                            flag = false;
                            aval = true;
                        }


                       // MatchCollection lineMatch = regexLine.Matches(line);
                        //MessageBox.Show(lines2);
                        if (lineMatch.Count > 0)
                        {
                            if (flag)
                            {
                                if ((line.IndexOf("з банку \"АВАЛЬ\"")) > 0 || (line.IndexOf("EasyPay")) > 0) //todo: добавил проверку на индустриал при чтении файла
                                {
                                    flag = false;
                                    aval = true;
                                }
                                Bank p = new Bank();
                                p.piece(line, datePl, aval, anotherPay);
                                res.Add(p);
                               // MessageBox.Show(string.Join(Environment.NewLine, p));  
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
