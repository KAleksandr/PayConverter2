using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftGenConverter
{
    class Bank
    {
        public string name { get; set; }
        public long platNumber { get; set; }
        public int datePayment { get; set; }
        public string mfo { get; set; }
        public string rahunok { get; set; }
        public string cliBankCode { get; set; }
        public string recivPayNum { get; set; }
        public string edrpou { get; set; }
        public int state { get; set; }

        public Bank()
        {
            this.name = "Назва";
            this.platNumber = 0;
            this.datePayment = 20010101;
            this.mfo = "0";
            this.rahunok = "0";
            this.recivPayNum = "test";
            this.cliBankCode = "0000";
            this.edrpou = "0";
            this.state = 1;

        }

    }
}
