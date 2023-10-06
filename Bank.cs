﻿using Microsoft.VisualBasic.FileIO;
using SoftGenConverter.Entity;
using System;
using System.Collections.Generic;
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
        ///     ot
        /// </summary>
        public string mfo { get; set; }

        public string rahunok { get; set; }
        public string iban { get; set; }

        public string edrpou { get; set; }

        //public string cliBankCode { get; set; }
        public string clientBankCode { get; set; }
        public string summa { get; set; }
        public string pruznach { get; set; }
        public string Appointment { get; set; }
        public DateTime dateP { get; set; }
        public Bank() { }
        public Bank(PayConverterConfig config) {
            this.id = config.bankid;
            this.name = config.NAME;
            this.rahunok = config.RAHUNOK;
            this.mfo = config.MFO;
            this.edrpou = config.EDRPOU;
            this.clientBankCode = config.clientBankCode;
            this.iban = config.IBAN;
            
        }
        
        public override string ToString()
        {
            return name + " " + rahunok;
        }

        public void Piece(string line, DateTime date, bool aval, bool anotherPay)
        {
            var parts = line.Split(';'); //Разделитель в CSV файле.
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
                    Appointment = parts[1];
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
                    Appointment = parts[1];
                    id = 1;
                }

                //MessageBox.Show("Name"+ name+" Rahunok"+ rahunok);
            }
            else
            {
                if (!anotherPay)
                {
                    name = parts[0].ToUpper();
                    pruznach = parts[1];
                    Appointment = parts[1];
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
                    name = parts[0].ToUpper();
                   
                    pruznach = parts[10];
                    mfo = parts[1];
                    rahunok = "" + parts[2];
                    //rahunok = "" + Convert.ToInt64(parts[2]);
                    edrpou = parts[3];
                    summa = parts[5];
                    id = 0;
                    dateP = date;
                    Appointment = parts[1];
                }
            }


            //todo: regex
        }

       
      
    public static List<Bank> ReadCsv(string filePath)
    {
        
            List<string[]> csvData = ReadCSVFile(filePath);
        List<Bank> banks = new List<Bank>();
        int count = 0;
        foreach (string[] row in csvData)
        {
            if(count != 0)
            // Отримуємо дані з кожного стовпця
            banks.Add(new Bank
            {
                name = row[0].ToUpper(),
                Appointment = row[1].ToUpper(),
                pruznach = row[1],
                mfo = row[2],
                rahunok = row[3],
                edrpou = row[4],
                summa = row[5],
                id = 1,
                dateP = DateTime.Now
                
            });
            count++;
        }

        return banks;
    }
    static List<string[]> ReadCSVFile(string filePath)
        {
            List<string[]> csvData = new List<string[]>();
           
            try
            {
                using (TextFieldParser parser = new TextFieldParser(filePath, Encoding.GetEncoding(1251)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");
                    parser.HasFieldsEnclosedInQuotes = true;

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        csvData.Add(fields);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Помилка: " + e.Message);
            }

            return csvData;
        }
        public static List<Bank> ReadFile(string filename, bool anotherPay)
        {
            var res = new List<Bank>();
            var date = 0;
            var regexDate = new Regex(@"\w*([0-9]{2}[.][0-9]{2}[.][0-9]{2}р.)");
            var regexLine = new Regex(@".+;.*;.+;.+;.+;.+;.*;.*;.+;.*");
            var flag = false;
            var aval = false;
            var datePl = DateTime.Today;

            // string pattern = @"\w*([0-9]{2}[.][0-9]{2}[.][0-9]{2}р.)";

            // File.WriteAllText(filename, Regex.Replace(File.ReadAllText(filename, Encoding.GetEncoding(1251)), pattern, " "), Encoding.GetEncoding(1251));

            try
            {
                using (var sr = new StreamReader(filename, Encoding.GetEncoding(1251)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var dateMatch = regexDate.Matches(line);
                        if (dateMatch.Count > 0)
                        {
                            var MyCultureInfo = new CultureInfo("de-DE");

                            var matchess = Regex.Matches(line, regexDate.ToString(), RegexOptions.IgnoreCase);
                            date = int.Parse(matchess[0].ToString().Replace("за", "").Replace("р.", "").Trim()
                                .Replace(".", ""));
                            datePl = DateTime.Parse(matchess[0].ToString().Replace("за", "").Replace("р.", "").Trim(),
                                MyCultureInfo);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            try
            {
                using (var sr = new StreamReader(filename, Encoding.GetEncoding(1251)))
                {
                    string line;
                    int count = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var lineMatch = regexLine.Matches(line);
                        var dateMatch = regexDate.Matches(line);
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
                                if (line.IndexOf("з банку \"АВАЛЬ\"") > 0 || line.IndexOf("EasyPay") > 0
                                ) //todo: добавил проверку на индустриал при чтении файла
                                {
                                    flag = false;
                                    aval = true;
                                }

                                var p = new Bank();
                                p.Piece(line, datePl, aval, anotherPay);
                                res.Add(p);
                                // MessageBox.Show(string.Join(Environment.NewLine, p));  
                            }

                            flag = true;
                        }
                        //else if(count != 0)
                        {
                            
                            //res = Bank.ReadCsv(filename);
                            
                        }

                        count++;
                    }
                }
            }
            catch (Exception)
            {
            }

            return res;
        }
    }
}