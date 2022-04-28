using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftGenConverter.Entity
{
   public class Oschad
    {
        /// <summary>
        ///  "Номер платіжного документу (ndoc)";
        /// </summary>
        public string Ndoc { get; set; }
        /// <summary>
        /// "Дата документу, дд.мм.рррр (dt)";
        /// </summary>
        public DateTime Dt { get; set; }
        /// <summary>
        ///  "Дата валютування, дд.мм.рррр (dv)";
        /// </summary>
        public DateTime Dv { get; set; }
        /// <summary>
        ///  "Рахунок відправника (acccli)";
        /// </summary>
        public string Acccli { get; set; }
        /// <summary>
        ///  "Рахунок отримувача (acccor)";
        /// </summary>
        public string Acccor { get; set; }
        /// <summary>
        ///  "Податковий код отримувача (ІПН, ЄДРПОУ, ЗКПО)** (okpocor)";
        /// </summary>
        public string Okpocor { get; set; }
        /// <summary>
        /// "Назва отримувача (namecor)";
        /// </summary>
        public string Namecor { get; set; }
        /// <summary>
        ///  "Сума платежу    (у копійках) (summa)";
        /// </summary>
        public int Summa { get; set; }
        /// <summary>
        /// "Валюта, ISO 4217 (val)";
        /// </summary>
        public int Val { get; set; }
        /// <summary>
        /// "Призначення платежу (nazn)";
        /// </summary>
        public string Nazn { get; set; }
        /// <summary>
        /// "Код країни-нерезидента отримувача (ISO 3166-1 numeric) (cod_cor)";
        /// </summary>
        public int Cod_cor { get; set; }
        /// <summary>
        /// "Додаткові реквізити (add_req)";
        /// </summary>
        public string Add_req { get; set; }           
        
    }
}
