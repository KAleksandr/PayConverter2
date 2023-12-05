using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace SoftGenConverter.Entity
{
   public  class AnotherPay_
    {
        public static int DeleteDublicate(string tableName)
        {
            if (tableName.ToUpper().Equals("PayConverterData".ToUpper()) || tableName.ToUpper().Equals("AnotherPayConverterData".ToUpper())) { 
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = $"delete from {tableName} where id not in (select min(id) from {tableName} group by name, ERDPO, RRahunok, Comment)";
            int delItem = cmd.ExecuteNonQuery();

            con.Close();
            return delItem;
            }
            else
            {
                return 0;
            }
    }
        public static void UpdateAnother(string tableName,int ID, string NAME, string ERDPO, string RRahunok, string Comment)
        {
            if (tableName.ToUpper().Equals("PayConverterData".ToUpper()) || tableName.ToUpper().Equals("AnotherPayConverterData".ToUpper()))
            {
                SQLiteConnection con = new SQLiteConnection(Db.Cs);
                SQLiteCommand cmd = new SQLiteCommand(con);
                con.Open();
                cmd.CommandText = $"UPDATE {tableName} set NAME = @NAME,ERDPO=@ERDPO,RRahunok=@RRahunok,Comment=@Comment where id = @ID";
                cmd.Parameters.AddWithValue("@NAME", NAME);
                cmd.Parameters.AddWithValue("@ERDPO", ERDPO);
                cmd.Parameters.AddWithValue("@RRahunok", RRahunok);
                cmd.Parameters.AddWithValue("@Comment", Comment);
                cmd.Parameters.AddWithValue("@ID", ID);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static void UpdateAnother(string tableName, AnotherPay pay)
        {
            if (tableName.ToUpper().Equals("PayConverterData".ToUpper()) || tableName.ToUpper().Equals("AnotherPayConverterData".ToUpper()))
            {
                SQLiteConnection con = new SQLiteConnection(Db.Cs);
                SQLiteCommand cmd = new SQLiteCommand(con);
                con.Open();
                cmd.CommandText = $"UPDATE {tableName} set NAME = @NAME,ERDPO=@ERDPO,RRahunok=@RRahunok,Comment=@Comment where id = @ID";
                cmd.Parameters.AddWithValue("@NAME", pay.NAME);
                cmd.Parameters.AddWithValue("@ERDPO", pay.ERDPO);
                cmd.Parameters.AddWithValue("@RRahunok", pay.RRahunok);
                cmd.Parameters.AddWithValue("@Comment", pay.Comment);
                cmd.Parameters.AddWithValue("@ID", pay.ID);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static int InsertTableFromList(string tableName, List<AnotherPay> anPay)
        {
            if (tableName.ToUpper().Equals("PayConverterData".ToUpper()) || tableName.ToUpper().Equals("AnotherPayConverterData".ToUpper()))
            {
                SQLiteConnection con = new SQLiteConnection(Db.Cs);
                con.Open();

                SQLiteCommand cmd = new SQLiteCommand(con);
                int count = 0;
                using (SQLiteTransaction transaction = con.BeginTransaction())
                {
                    anPay.ForEach(lst =>
                    {
                        cmd.CommandText = $"INSERT INTO {tableName}(NAME,ERDPO,RRahunok,Comment) VALUES(@NAME,@ERDPO,@RRahunok,@Comment)";
                        cmd.Parameters.AddWithValue("@NAME", lst.NAME);
                        cmd.Parameters.AddWithValue("@ERDPO", lst.ERDPO);
                        cmd.Parameters.AddWithValue("@RRahunok", lst.RRahunok);
                        cmd.Parameters.AddWithValue("@Comment", lst.Comment);
                        count += cmd.ExecuteNonQuery(); 
                    });
                    transaction.Commit();
                }               
                con.Close();
                return count;
            }
            else {return 0; }
        }
        public static int InsertData(string tableName, AnotherPay anPay, out long id)
        {
            if (tableName.ToUpper().Equals("PayConverterData".ToUpper()) || tableName.ToUpper().Equals("AnotherPayConverterData".ToUpper()))
            {
                SQLiteConnection con = new SQLiteConnection(Db.Cs);
                con.Open();

                SQLiteCommand cmd = new SQLiteCommand(con);
                int count = 0;
                using (SQLiteTransaction transaction = con.BeginTransaction())
                {
                    cmd.CommandText = $"INSERT INTO {tableName}(NAME,ERDPO,RRahunok,Comment) VALUES(@NAME,@ERDPO,@RRahunok,@Comment)";
                    cmd.Parameters.AddWithValue("@NAME", anPay.NAME);
                    cmd.Parameters.AddWithValue("@ERDPO", anPay.ERDPO);
                    cmd.Parameters.AddWithValue("@RRahunok", anPay.RRahunok);
                    cmd.Parameters.AddWithValue("@Comment", anPay.Comment);
                    count += cmd.ExecuteNonQuery();    
                    transaction.Commit();
                }
                id = con.LastInsertRowId;
                con.Close();
                return count;
            }
            else { id = 0; return 0; }
        }
    }
}
