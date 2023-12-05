using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoftGenConverter.Entity
{
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(Payments));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (Payments)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "DebitPrivate")]
    public class DebitPrivate
    {
        /// <summary>
        ///  Дата народження особи
        /// </summary>
                [XmlElement(ElementName = "birthDate")]
        public string BirthDate { get; set; }
        /// <summary>
        /// Місто народження (із вказівкою області)
        /// </summary>

        [XmlElement(ElementName = "birthCity")]
        public string BirthCity { get; set; }
        /// <summary>
        /// Країна народження, формат UA
        /// </summary>

        [XmlElement(ElementName = "birthCountry")]
        public string BirthCountry { get; set; }

        [XmlElement(ElementName = "birthCountryName")]
        public string BirthCountryName { get; set; }
        /// <summary>
        /// РНОКПП або документ
        /// </summary>

        [XmlElement(ElementName = "idCode")]
        public string IdCode { get; set; }
        /// <summary>
        /// Тип документа ідентифікації(визначає вміст поля idCode) : RNRCT - РНОКПП.PSPT - Серія (за наявності) та номер паспорта.UNKN - Платник не має інформації про ідентифікацію отримувача.Реквізит „Identification” заповнюється значенням 99999
        /// </summary>

        [XmlElement(ElementName = "idType")]
        public string IdType { get; set; }
        /// <summary>
        /// Ким виданий документ
        /// </summary>
        [XmlElement(ElementName = "issuer")]
        public string Issuer { get; set; }
        /// <summary>
        /// Коли виданий документ, формат dd.MM.yyyy
        /// </summary>

        [XmlElement(ElementName = "issueDate")]
        public string IssueDate { get; set; }
    }

    [XmlRoot(ElementName = "CreditOrganization")]
    public class CreditOrganization
    {

        [XmlElement(ElementName = "idCode")]
        public string IdCode { get; set; }

        [XmlElement(ElementName = "idType")]
        public string IdType { get; set; }
    }
    /// <summary>
    /// Дані ідентифікації платника юридичної особи
    /// </summary>
    [XmlRoot(ElementName = "DebitOrganization")]
    public class DebitOrganization
    {
        /// <summary>
        ///  Номер ідентифікації ЮО, наприклад код ЄДРПОУ
        /// </summary>
        [XmlElement(ElementName = "idCode")]
        public string IdCode { get; set; }
        /// <summary>
        /// Тип ідентифікації(визначає вміст поля idCode): USRC - Зазначається код ЄДРПОУ.TRAN -Зазначається реєстраційний номер платника податку(РНПП 9 символів). NA -Ідентифікаційний код юридичної особи не присвоєний.Зазначається 9 нулів 
        /// /// </summary>

        [XmlElement(ElementName = "idType")]
        public string IdType { get; set; }
    }

    [XmlRoot(ElementName = "header")]
    public class Header
    {

        [XmlElement(ElementName = "AdmstnZone")]
        public string AdmstnZone { get; set; }

        [XmlElement(ElementName = "RefNb")]
        public string RefNb { get; set; }
    }

    [XmlRoot(ElementName = "items")]
    public class Items
    {

        [XmlElement(ElementName = "Tp")]
        public string Tp { get; set; }

        [XmlElement(ElementName = "Ctgy")]
        public string Ctgy { get; set; }

        [XmlElement(ElementName = "CtgyDtls")]
        public string CtgyDtls { get; set; }

        [XmlElement(ElementName = "CertId")]
        public string CertId { get; set; }

        [XmlElement(ElementName = "TaxAmt")]
        public string TaxAmt { get; set; }

        [XmlElement(ElementName = "AddtlInf")]
        public string AddtlInf { get; set; }
    }

    [XmlRoot(ElementName = "taxes")]
    public class Taxes
    {

        [XmlElement(ElementName = "header")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "payertype")]
        public string Payertype { get; set; }

        [XmlElement(ElementName = "items")]
        public Items Items { get; set; }
    }

    [XmlRoot(ElementName = "refPurpose")]
    public class RefPurpose
    {

        [XmlElement(ElementName = "taxes")]
        public Taxes Taxes { get; set; }

        [XmlElement(ElementName = "AddtlRmtInf")]
        public string AddtlRmtInf { get; set; }
    }

    [XmlRoot(ElementName = "Docs")]
    public class Docs
    {
        /// <summary>
        ///1 Aтрибут тегу Docs. Сума платежу, формат ХХХ.ХХ(наприклад 1787.94)
        /// </summary>
        [XmlAttribute(AttributeName = "Amount")]
        public decimal Amount { get; set; }
        /// <summary>
        /// 1 . Атрибут тегу Docs.  Код валюти платежу, формат UAH, USD, EUR
        /// </summary>
        [XmlAttribute(AttributeName = "CurrencyTag")]
        public string CurrencyTag { get; set; }
        /// <summary>
        /// 1 Дата платежу, формат dd.MM.yyyy
        /// </summary>
        [XmlElement(ElementName = "OrgDate")]
        public string OrgDate { get; set; }
        /// <summary>
        /// 0 Дата проведення платежу, формат dd.MM.yyyy        
        /// </summary>
        [XmlElement(ElementName = "PayDate")]
        public string PayDate { get; set; }
        /// <summary>
        ///1 № платежу
        /// </summary>
        [XmlElement(ElementName = "Code")]
        public string Code { get; set; }
        /// <summary>
        ///0 МФО банку отримувача
        /// </summary>
        [XmlElement(ElementName = "CreditBankCode")]
        public string CreditBankCode { get; set; }
        /// <summary>
        ///0 Назва банку отримувача
        /// </summary>

        [XmlElement(ElementName = "CreditBankName")]
        public string CreditBankName { get; set; }
        /// <summary>
        /// 1 Рахунок отримувача (2600)
        /// </summary>
        [XmlElement(ElementName = "CreditAccount")]
        public string CreditAccount { get; set; }
        /// <summary>
        ///1 IBAN отримувача, формат UA
        /// </summary>
        [XmlElement(ElementName = "CreditCodeIBAN")]
        public string CreditCodeIBAN { get; set; }
        /// <summary>
        ///1  Назва\ім’я отримувача
        /// </summary>

        [XmlElement(ElementName = "CreditName")]
        public string CreditName { get; set; }
        /// <summary>
        /// 1 ЄДРПОУ\РНОКПП отримувача
        /// </summary>

        [XmlElement(ElementName = "CreditStateCode")]
        public string CreditStateCode { get; set; }
        /// <summary>
        ///0 МФО банку платника
        /// </summary>

        [XmlElement(ElementName = "DebitBankCode")]
        public string DebitBankCode { get; set; }
        /// <summary>
        ///0 Назва банку платника
        /// </summary>
        [XmlElement(ElementName = "DebitBankName")]
        public string DebitBankName { get; set; }
        /// <summary>
        /// 1 Рахунок платника (2600)
        /// </summary>

        [XmlElement(ElementName = "DebitAccount")]
        public string DebitAccount { get; set; }
        /// <summary>
        /// IBAN платника, формат UA
        /// </summary>
        [XmlElement(ElementName = "DebitCodeIBAN")]
        public string DebitCodeIBAN { get; set; }
        /// <summary>
        ///1 Назва\ім’я платника
        /// </summary>
        [XmlElement(ElementName = "DebitName")]
        public string DebitName { get; set; }
        /// <summary>
        /// 1 ЄДРПОУ\РНОКПП платника
        /// </summary>

        [XmlElement(ElementName = "DebitStateCode")]
        public string DebitStateCode { get; set; }
        /// <summary>
        ///1 Призначення платежу
        /// </summary>
        [XmlElement(ElementName = "Purpose")]
        public string Purpose { get; set; }
        /// <summary>
        /// Признак резидентності отримувача(1 –резидент, 0 – нерезидент)
        /// </summary>
        [XmlElement(ElementName = "IsCreditResident")]
        public int IsCreditResident { get; set; }

        /// <summary>
        /// Дані ідентифікації платника юридичної особи
        /// </summary>
        [XmlElement(ElementName = "DebitOrganization")]
        public DebitOrganization DebitOrganization { get; set; }

        [XmlElement(ElementName = "CreditOrganization")]
        public CreditOrganization CreditOrganization { get; set; }
        /// <summary>
        ///  Структуроване призначення платежу при сплаті  податків
        /// </summary>
         [XmlElement(ElementName = "refPurpose")]
        public RefPurpose RefPurpose { get; set; }



        [XmlText]
        public string Text { get; set; }
    }
    /// <summary>
    /// Кореневий тег
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Payments")]
    public class Payments
    {
        /// <summary>
        /// Основний тег структури
        /// </summary>
        [XmlElement(ElementName = "Docs")]
        public List<Docs> Docs { get; set; } = new List<Docs>();

        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }

        [XmlAttribute(AttributeName = "xsi",  Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }

       
    }


}
