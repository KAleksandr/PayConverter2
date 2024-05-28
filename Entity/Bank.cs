using Microsoft.VisualBasic.FileIO;
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
    public enum TypeFile
    {
        defaultType,
        standart
    }
    public class Bank
    {
        public string name { get; set; }
        public int id { get; set; }
        public string mfo { get; set; }
        public string rahunok { get; set; }
        public string iban { get; set; }
        public string edrpou { get; set; }
        public string clientBankCode { get; set; }
        public string summa { get; set; }
        public string pruznach { get; set; }
        public string Appointment { get; set; }
        public DateTime dateP { get; set; }
        public string specialPr { get; set; } = "";
        public Bank() { }
        public Bank(PayConverterConfig config)
        {
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
            string pattern = @";""(.*?)"";";
            string result = "";
            string mark = "&&";
            Match match = Regex.Match(line, pattern);
            if (match.Success)
            {
                // Отримайте значення підстрічки
                result = match.Groups[1].Value;
                line = line.Replace(@";""" + result + @""";", $";{mark};");
                Console.WriteLine(result);
            }
            var parts = line.Split(';');
            if (!string.IsNullOrEmpty(result))
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Contains(mark))
                    {
                        parts[i] = parts[i].Replace(mark, result);
                    }
                }
            }
            if (aval)
            {
                if (!anotherPay)
                {
                    name = parts[0].ToUpper();
                    mfo = parts[2];
                    rahunok = "" + parts[3];
                    edrpou = parts[4];
                    dateP = date;
                    summa = parts[8];
                    if (parts[1].ToString().Substring(0, 1).Equals("!"))
                    {
                        specialPr = "+";
                        parts[1] = parts[1].ToString().TrimStart().Substring(1, parts[1].Length - 1);
                    }
                    else
                    {
                        specialPr = "";
                    }
                    pruznach = parts[1];
                    Appointment = parts[1];
                    id = 1;
                }
                else
                {
                    name = parts[10].ToUpper();
                    mfo = parts[2];
                    rahunok = "" + parts[3];
                    edrpou = parts[4];
                    dateP = date;
                    summa = parts[8];
                    if (parts[1].ToString().Substring(0, 1).Equals("!"))
                    {
                        specialPr = "+";
                        parts[1] = parts[1].ToString().TrimStart().Substring(1, parts[1].Length - 1);
                        pruznach = parts[1];
                    }
                    else
                    {
                        pruznach = parts[0] + " " + parts[1];
                        specialPr = "";
                    }
                    Appointment = parts[1];
                    id = 1;
                }
            }
            else
            {
                if (!anotherPay)
                {
                    name = parts[0].ToUpper();
                    if (parts[1].ToString().Substring(0, 1).Equals("!"))
                    {
                        specialPr = "+";
                        parts[1] = parts[1].ToString().TrimStart().Substring(1, parts[1].Length - 1);
                    }
                    else
                    {
                        specialPr = "";
                    }
                    pruznach = parts[1];
                    Appointment = parts[1];
                    mfo = parts[2];
                    rahunok = "" + parts[3];
                    edrpou = parts[4];
                    summa = parts[6];
                    id = 0;
                    dateP = date;
                }
                else
                {
                    name = parts[0].ToUpper();
                    if (parts[10].ToString().Substring(0, 1).Equals("!"))
                    {
                        specialPr = "+";
                        parts[10] = parts[1].ToString().TrimStart().Substring(1, parts[10].Length - 1);
                    }
                    else
                    {
                        specialPr = "";
                    }
                    pruznach = parts[10];
                    mfo = parts[1];
                    rahunok = "" + parts[2];
                    edrpou = parts[3];
                    summa = parts[5];
                    id = 0;
                    dateP = date;
                    Appointment = parts[1];
                }
            }
        }



        public static List<Bank> ReadCsv(string filePath)
        {
            List<string[]> csvData = ReadCSVFile(filePath);
            List<Bank> banks = new List<Bank>();
            int count = 0;
            foreach (string[] row in csvData)
            {
                if (count != 0)
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
        public static List<Bank> ReadFile(string filename, bool anotherPay,DateTime dT, TypeFile type = TypeFile.defaultType)
        {
            var res = new List<Bank>();
            var date = 0;
            var regexDate = new Regex(@"\w*([0-9]{2}[.][0-9]{2}[.][0-9]{2})");
            var regexLine = new Regex(@".+;.*;.+;.+;.+;.+;.*;.*;.+;.*");
            var regexDate1 = new Regex(@"\b(\d{2}\.\d{2}\.\d{2})\b");
            var regexLine1 = new Regex(@".+;.*;.+;.+;.+;.+;.*;.*;.+;.*");
            var flag = false;
            var aval = false;
            var datePl = dT;
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
                        var dateMatch1 = regexDate1.Match(line);
                        if (dateMatch1.Success)
                        {
                            datePl = DateTime.ParseExact(dateMatch1.Value, "dd.MM.yy", CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading date: " + ex.Message);
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
                        if (lineMatch.Count > 0)
                        {
                            if (flag)
                            {
                                if (line.IndexOf("з банку \"АВАЛЬ\"") > 0 || line.IndexOf("EasyPay") > 0)
                                {
                                    flag = false;
                                    aval = true;
                                }
                                var p = new Bank();
                                p.Piece(line, datePl, aval, anotherPay);
                                res.Add(p);                               
                            }
                            flag = true;
                        }
                        else if (count != 0 && type == TypeFile.standart)
                        {
                            res = Bank.ReadCsv(filename);
                        }
                        count++;
                    }
                }
            }
            catch{}
            return res;
        }
    }
}