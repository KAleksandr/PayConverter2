using Microsoft.Office.Interop.Excel;
using SoftGenConverter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace SoftGenConverter.Service
{
    public class FillingOutAbankXml
    {
        public Payments Payments { get; set; } 
        public List<Payments> PaymentsList { get; set; } = new List<Payments>();
        public FillingOutAbankXml(DataGridView dataGridView1N, Bank aBank, int docnum = 1, int type = 0)
        {
            int numberRecords = 499; //максимальна кількість записів для вивантаження АБанк
            string CreditBankCode = "899998";//Група реквізитів структурованого призначення платежів (обов’язково для платежів на користь   казначейства(МФО 899998) з 01.12.2023
           Payments = new Payments();
            int count = 0;
            //перебираємо всі записи
            for (int i = 1; i <= dataGridView1N.Rows.Count; i++) // todo: 
            {
                count++;
                if(count == numberRecords)
                {
                    PaymentsList.Add(Payments);
                    Payments = new Payments();
                    count = 1;
                }

                decimal amount = 0;
                string purpose = "";
                string creditBankCode = "";
                #region Заповнення полів
                //amount
                try
                {
                    if (type == 5)
                    {
                        decimal.TryParse(dataGridView1N.Rows[i - 1].Cells[8].Value.ToString().Replace(".", ","), out amount);//Сума платежу *
                    }
                    else if (type == 6)
                    {
                        decimal.TryParse(dataGridView1N.Rows[i - 1].Cells[0].Value.ToString().Replace(".", ","), out amount);//Сума платежу*
                    }


                }
                catch { }
                //purpose
                try
                {
                    bool isOriginPurpose = dataGridView1N.Rows[i - 1].Cells[2].Value.ToString().Equals("+");
                    if (type == 5)
                    {
                        string purpouseDefault = "";
                        if(dataGridView1N.Rows[i - 1].Cells[13].Value == null)
                        {
                            dataGridView1N.Rows[i - 1].Cells[13].Value = "";
                        }

                         purpouseDefault =  string.IsNullOrEmpty(dataGridView1N.Rows[i - 1].Cells[13].Value.ToString()) ? "" : dataGridView1N.Rows[i - 1].Cells[13].Value.ToString() + " ";
                        purpose = isOriginPurpose ? dataGridView1N.Rows[i - 1].Cells[11].Value.ToString() : purpouseDefault + dataGridView1N.Rows[i - 1].Cells[11].Value.ToString();//FIELD_PURPOSE_CODE Призначення платежу
                    }
                    else if (type == 6)
                    {
                        string purpouseDefault = "";
                        if(dataGridView1N.Rows[i - 1].Cells[10].Value == null)
                        {
                            dataGridView1N.Rows[i - 1].Cells[10].Value = "";
                        }
                        purpouseDefault = string.IsNullOrEmpty(dataGridView1N.Rows[i - 1].Cells[10].Value.ToString()) ? "" : dataGridView1N.Rows[i - 1].Cells[10].Value.ToString() + " ";
                        purpose = isOriginPurpose ? dataGridView1N.Rows[i - 1].Cells[2].Value.ToString() : purpouseDefault + dataGridView1N.Rows[i - 1].Cells[2].Value.ToString();//FIELD_PURPOSE_CODE Призначення платежу
                    }
                }
                catch { }
                //creditBankCode
                try
                {
                    creditBankCode = dataGridView1N.Rows[i - 1].Cells[5].Value.ToString();
                }
                catch { }
                //CreditStateCode
                string creditStateCode = "";
                try
                {
                    if (type == 5)
                    {
                        creditStateCode = dataGridView1N.Rows[i - 1].Cells[12].Value.ToString();//FIELD_BENEF_TAX_CODE Код ЕДРПОУ отримувача коштів
                    }
                    else if (type == 6)
                    {
                        creditStateCode = dataGridView1N.Rows[i - 1].Cells[7].Value.ToString();//FIELD_BENEF_TAX_CODE Код ЕДРПОУ отримувача коштів
                    }

                }
                catch { }
               
                //CreditCodeIBAN
                string creditCodeIBAN = "";
                try
                {

                    if (type == 5)
                    {
                        creditCodeIBAN = dataGridView1N.Rows[i - 1].Cells[7].Value.ToString();//FIELD_BENEF_IBAN Номер поточного рахунку отримувача*
                    }
                    else if (type == 6)
                    {
                        creditCodeIBAN = dataGridView1N.Rows[i - 1].Cells[6].Value.ToString();//FIELD_BENEF_IBAN Номер поточного рахунку отримувача*
                    }
                }
                catch { }
                //CreditAccount
                string creditAccount = "";
                try
                {
                    creditAccount = ExtractAccountNumberFromIBAN(creditCodeIBAN);
                }
                catch { }
                //CreditName
                string creditName = "";
                try
                {
                    if (type == 5)
                    {
                        creditName = dataGridView1N.Rows[i - 1].Cells[10].Value.ToString();//FIELD_BENEF_NAME Найменування отримуача*
                    }
                    else if (type == 6)
                    {
                        creditName = dataGridView1N.Rows[i - 1].Cells[8].Value.ToString();//FIELD_BENEF_NAME Найменування отримуача*
                    }

                }
                catch { }
                //DebitAccount
                string debitAccount = "";
                try
                {
                    debitAccount = ExtractAccountNumberFromIBAN(aBank.rahunok);
                }
                catch { }
                #endregion

                var docs = new Docs();
                if (creditBankCode.Equals(CreditBankCode))
                {
                    docs = new Docs()
                    {
                        Amount = amount,//+
                        CurrencyTag = "UAH",//+
                        OrgDate = DateTime.Now.ToString("yyyy-MM-dd"),//+
                                                                      //PayDate = DateTime.Now.ToString("dd.MM.yyyy"),
                        Code = (docnum + i).ToString(),//+
                                                  //отримувач
                        CreditBankCode = creditBankCode,//+
                        CreditAccount = creditAccount,
                        CreditCodeIBAN = creditCodeIBAN,
                        CreditName = creditName,
                        CreditStateCode = creditStateCode,
                        //платник
                        DebitBankCode = aBank.mfo,
                        DebitAccount = debitAccount,
                        DebitCodeIBAN = aBank.rahunok,
                        DebitBankName = @"АТ ""А - БАНК""",
                        DebitName = "ТОВ \"ФК \" МПС\"",
                        DebitStateCode = aBank.edrpou,
                        Purpose = purpose,
                        IsCreditResident = 1,
                        DebitOrganization = new DebitOrganization()
                        {
                            IdCode = aBank.edrpou,
                            IdType = GetIdType(aBank.edrpou)
                        },
                        CreditOrganization = new CreditOrganization()
                        {
                            IdCode = creditStateCode,
                            IdType = GetIdType(creditStateCode)
                        },
                        RefPurpose = new RefPurpose()
                        {
                            Taxes = new Taxes()
                            {
                                Header = new Header()
                                {
                                    AdmstnZone = "",
                                    RefNb = ""
                                },
                                Payertype = "Taxes",
                                Items = new Items()
                                {
                                    Tp = "",
                                    Ctgy = "",
                                    CtgyDtls = "",
                                    CertId = "121",
                                    TaxAmt = "0",
                                    AddtlInf = purpose
                                }

                            },
                            AddtlRmtInf = ""
                        }


                    };
                }
                else
                {
                    docs = new Docs()
                    {
                        Amount = amount,//+
                        CurrencyTag = "UAH",//+
                        OrgDate = DateTime.Now.ToString("yyyy-MM-dd"),//+
                        //PayDate = DateTime.Now.ToString("dd.MM.yyyy"),
                        Code = (docnum + i).ToString(),//+
                        //отримувач
                        CreditBankCode = creditBankCode,//+
                        CreditAccount = creditAccount,
                        CreditCodeIBAN = creditCodeIBAN,
                        CreditName = creditName,
                        CreditStateCode = creditStateCode,
                        //платник
                        DebitBankCode = aBank.mfo,
                        DebitAccount = debitAccount,
                        DebitCodeIBAN = aBank.rahunok,
                        DebitBankName = @"АТ ""А - БАНК""",
                        DebitName = "ТОВ \"ФК \" МПС\"",
                        DebitStateCode = aBank.edrpou,
                        Purpose = purpose,
                        IsCreditResident = 1                    


                    };
                }
                

                Payments.Docs.Add(docs);

            }
            PaymentsList.Add(Payments);
        }
        static string ExtractAccountNumberFromIBAN(string iban)
        {
            // Перевірка на правильність довжини IBAN
            if (iban.Length != 29)
            {
                throw new ArgumentException("Invalid IBAN length");
            }

            // Витягування другої групи символів (14 знаків) - номер рахунку клієнта банку
            string accountNumber = iban.Substring(10, 19); 

            // Видалення зайвих нулів на початку номеру рахунку
            accountNumber = accountNumber.TrimStart('0'); 

            return accountNumber;
        }
        static string GetIdType(string edrpou)
        {
            return edrpou.Length == 8 ? "USRC" : edrpou.Length == 9 ? "TRAN" : "NA";
        }
    }
}
