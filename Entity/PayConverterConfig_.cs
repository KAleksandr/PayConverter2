using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SoftGenConverter.Entity
{
    public class PayConverterConfig_
    {
        
        public static void UpdateAnother(int ID, string NAME, string RAHUNOK, string MFO, string EDRPOU, string clientBankCode, string IBAN, int bankid)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = $"UPDATE PayConverterConfig set NAME = @NAME,RAHUNOK =@RAHUNOK,MFO=@MFO,EDRPOU=@EDRPOU,clientBankCode=@clientBankCode,IBAN=@IBAN,bankid=@bankid where id = @ID";
            cmd.Parameters.AddWithValue("@NAME", NAME);
            cmd.Parameters.AddWithValue("@RAHUNOK", RAHUNOK);
            cmd.Parameters.AddWithValue("@MFO", MFO);
            cmd.Parameters.AddWithValue("@EDRPOU", EDRPOU);
            cmd.Parameters.AddWithValue("@clientBankCode", clientBankCode);
            cmd.Parameters.AddWithValue("@IBAN", IBAN);
            cmd.Parameters.AddWithValue("@bankid", bankid);
            cmd.Parameters.AddWithValue("@ID", ID);
            cmd.ExecuteNonQuery();

            con.Close();
        }
        public static void UpdateByBankId(PayConverterConfig config)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = $"select count(*) from PayConverterConfig   where bankid=@bankid";
            cmd.Parameters.AddWithValue("@bankid", config.bankid);
            var countObj = cmd.ExecuteScalar();
           
            Int32.TryParse(countObj.ToString(), out int count);
            if(count > 0)
            {
                cmd.CommandText = $"UPDATE PayConverterConfig set NAME = @NAME,RAHUNOK =@RAHUNOK,MFO=@MFO,EDRPOU=@EDRPOU,clientBankCode=@clientBankCode,IBAN=@IBAN  where bankid=@bankid";
                cmd.Parameters.AddWithValue("@NAME", config.NAME);
                cmd.Parameters.AddWithValue("@RAHUNOK", config.RAHUNOK);
                cmd.Parameters.AddWithValue("@MFO", config.MFO);
                cmd.Parameters.AddWithValue("@EDRPOU", config.EDRPOU);
                cmd.Parameters.AddWithValue("@clientBankCode", config.clientBankCode);
                cmd.Parameters.AddWithValue("@IBAN", config.IBAN);
                cmd.Parameters.AddWithValue("@bankid", config.bankid);

                cmd.ExecuteNonQuery();
            }
            else
            {
                InsertByBankId(config);
            }
            

            con.Close();
        }
       private static void InsertByBankId(PayConverterConfig config)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = @"insert into PayConverterConfig (NAME,RAHUNOK,MFO,EDRPOU,clientBankCode,IBAN,bankid) 
                            VALUES(@NAME,@RAHUNOK,@MFO,@EDRPOU,@clientBankCode,@IBAN, @bankid )";            
            cmd.Parameters.AddWithValue("@NAME", config.NAME);
            cmd.Parameters.AddWithValue("@RAHUNOK", config.RAHUNOK);
            cmd.Parameters.AddWithValue("@MFO", config.MFO);
            cmd.Parameters.AddWithValue("@EDRPOU", config.EDRPOU);
            cmd.Parameters.AddWithValue("@clientBankCode", config.clientBankCode);
            cmd.Parameters.AddWithValue("@IBAN", config.IBAN);
            cmd.Parameters.AddWithValue("@bankid", config.bankid);

            cmd.ExecuteNonQuery();

            con.Close();
        }
    }
}
