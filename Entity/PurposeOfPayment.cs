using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SoftGenConverter.Entity
{
    public class PurposeOfPayment
    {
        public int ID { get; set; }
        public string NAME { get; set; } = "";
        public string PURPOSE { get; set; } = "";
    }
    public class PurposeOfPayment_
    {
        private static readonly string tableName = "PurposeOfPayment";
        public static int DeleteDublicate()
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = $"delete from {tableName} where id not in (select min(id) from {tableName} group by name, purpose)";
            int delItem = cmd.ExecuteNonQuery();
            con.Close();
            return delItem;
        }
        public static void UpdatePurpose(int ID, string NAME, string purpose)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = $"UPDATE {tableName} set NAME = @NAME,purpose=@purpose where id = @ID";
            cmd.Parameters.AddWithValue("@NAME", NAME);
            cmd.Parameters.AddWithValue("@purpose", purpose);
            cmd.Parameters.AddWithValue("@ID", ID);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public static void UpdatePurpose(string NAME, string purpose)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = $"UPDATE {tableName} set purpose=@purpose where NAME = @NAME";
            cmd.Parameters.AddWithValue("@NAME", NAME);
            cmd.Parameters.AddWithValue("@purpose", purpose);
            
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public static void UpdatePurpose(PurposeOfPayment purpose)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = $"UPDATE {tableName} set NAME = @NAME, PURPOSE=@PURPOSE where id = @ID";
            cmd.Parameters.AddWithValue("@NAME", purpose.NAME);
            cmd.Parameters.AddWithValue("@PURPOSE", purpose.PURPOSE);
            cmd.Parameters.AddWithValue("@ID", purpose.ID);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public static void InsertOrUpdatePurpose(string name, string purpose)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(purpose))
            {
                PurposeOfPayment ofPayment = new PurposeOfPayment() { NAME = name, PURPOSE = purpose };
                string existName = GetPurpose(name);
              
                if (string.IsNullOrEmpty(existName))
                {
                    InsertData(ofPayment, out long id);
                }
                else
                {
                    UpdatePurpose(ofPayment.NAME,ofPayment.PURPOSE);
                }
            }
        }
        public static int InsertTableFromList(List<PurposeOfPayment> purposes)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            con.Open();

            SQLiteCommand cmd = new SQLiteCommand(con);
            int count = 0;
            using (SQLiteTransaction transaction = con.BeginTransaction())
            {
                purposes.ForEach(lst =>
                {
                    cmd.CommandText = $"INSERT INTO {tableName}(NAME,PURPOSE) VALUES(@NAME,@PURPOSE)";
                    cmd.Parameters.AddWithValue("@NAME", lst.NAME);
                    cmd.Parameters.AddWithValue("@PURPOSE", lst.PURPOSE);
                    count += cmd.ExecuteNonQuery();
                });
                transaction.Commit();
            }

            con.Close();
            return count;
        }
        public static int InsertData(PurposeOfPayment purpose, out long id)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            con.Open();

            SQLiteCommand cmd = new SQLiteCommand(con);
            int count = 0;
            using (SQLiteTransaction transaction = con.BeginTransaction())
            {
                cmd.CommandText = $"INSERT INTO {tableName}(NAME,PURPOSE) VALUES(@NAME,@PURPOSE)";
                cmd.Parameters.AddWithValue("@NAME", purpose.NAME);
                cmd.Parameters.AddWithValue("@PURPOSE", purpose.PURPOSE);

                count += cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            id = con.LastInsertRowId;
            con.Close();


            return count;

        }
        public static string GetPurpose(string name)
        {
            SQLiteConnection con = new SQLiteConnection(Db.Cs);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand(con);
            string stm = $"SELECT * FROM {tableName} where name = @name";
            cmd.CommandText = stm;
            cmd.Parameters.AddWithValue("@name", name);
            PurposeOfPayment purp = new PurposeOfPayment();
            string purpuse = "";

            using (SQLiteDataReader readers = cmd.ExecuteReader())
            {
                var dataTable = new DataTable();
                dataTable.Load(readers);
                List<DataRow> listTable = dataTable.AsEnumerable().ToList();

                purp =
                        (from item in listTable
                         select new PurposeOfPayment
                         {
                             PURPOSE = item.Field<string>("PURPOSE"),


                         }).FirstOrDefault();
                purpuse = purp != null ? purp.PURPOSE : "";
            }

            con.Close();
            //MessageBox.Show(name + " " + purpuse);
            return purpuse;
        }

    }
}
