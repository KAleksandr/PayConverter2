using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftGenConverter.Entity
{
   public class PayConverterConfig
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string RAHUNOK { get; set; }
        public string MFO { get; set; }
        public string EDRPOU { get; set; }
        public string clientBankCode { get; set; }
        public string IBAN { get; set; }
        public int bankid { get; set; }
        public PayConverterConfig() { }
        public PayConverterConfig(Bank bank) {
            this.NAME = bank.name;
            this.RAHUNOK = bank.rahunok;
            this.MFO = bank.mfo;
            this.EDRPOU = bank.edrpou;
            this.clientBankCode = bank.clientBankCode;
            this.IBAN = bank.iban;
            this.bankid = bank.id;
        }

    }
}
