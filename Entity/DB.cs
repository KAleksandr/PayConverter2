using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace SoftGenConverter.Entity
{
    public static  class Db
    {
       
        // protected static string Cs  { get { return @"URI=file:D:\Shlyahovi\DbSqLite\DorMash.db"; } }

       public readonly static string runningPath = AppDomain.CurrentDomain.BaseDirectory + "Resources" + @"\db\PayConverterData.db";
        readonly static string uri = @"URI=file:";
        public static string Cs => uri + runningPath;
        public static void CreateDb()
        {
            SQLiteConnection.CreateFile(runningPath);
            if (File.Exists(runningPath))
            {
                CreateTable();
            }
        }
        public static void CreateTable()
        {
            SQLiteConnection con = new SQLiteConnection(Cs);
            con.Open();

            SQLiteCommand cmd = new SQLiteCommand(con)
            {
                CommandText = @"DROP TABLE IF EXISTS AnotherPayConverterData;
                                DROP TABLE IF EXISTS PayConverterData;
                                DROP TABLE IF EXISTS PayConverterConfig; "
            };
            cmd.ExecuteNonQuery();
            cmd.CommandText = @"CREATE TABLE AnotherPayConverterData ( ID INTEGER, NAME TEXT, ERDPO TEXT, RRahunok TEXT, Comment TEXT, PRIMARY KEY(ID AUTOINCREMENT));
                CREATE TABLE PayConverterData ( ID INTEGER, NAME TEXT, ERDPO TEXT, RRahunok TEXT, Comment TEXT, PRIMARY KEY(ID AUTOINCREMENT));
                CREATE TABLE PayConverterConfig (
	                                            ID	INTEGER,
	                                            NAME	TEXT NOT NULL,
	                                            RAHUNOK	TEXT NOT NULL,
	                                            MFO	TEXT NOT NULL,
	                                            EDRPOU	TEXT NOT NULL,
	                                            clientBankCode	TEXT NOT NULL,
	                                            IBAN	TEXT NOT NULL,
	                                            bankid	INTEGER NOT NULL DEFAULT 0,
	                                            PRIMARY KEY(ID AUTOINCREMENT));";
            
            cmd.ExecuteNonQuery();
            cmd.CommandText = @"insert into PayConverterConfig (NAME,RAHUNOK,MFO,EDRPOU,clientBankCode,IBAN,bankid) 
                            VALUES('Райффайзен Банк Аваль','UA643808050000000000265043345','313849','','40375721','',0 ),
                            ('УкрГазБанк','26545743585101.980','','40375721','','UA383204780000026545743585101',1 ),
                            ('Індустріал','UA173138490000026503010000233','313849','','40375721','',2 ),
                            ('Ощадбанк','UA243020760000026501300388426','302076','40375721','3069252999','',3 ),
                            ('Пумб','UA2','30','40','30','',4 )";
            cmd.ExecuteNonQuery();
            con.Close();
            
        }
       
        public static int DeleteById(string tableName,int id)
        {
            SQLiteConnection con = new SQLiteConnection(Cs);
            SQLiteCommand cmd = new SQLiteCommand(con);
            con.Open();
            cmd.CommandText = $"DELETE FROM {tableName} where id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            int delItem = cmd.ExecuteNonQuery();

            con.Close();
            return delItem;
        }
       
        //
       
        
        public static List<T> SelectTable<T>(string tableName) where T : new()
        {
            string TableName = tableName;// AnotherPayConverterData";
            SQLiteConnection con = new SQLiteConnection(Cs);
            con.Open();
            List<T> selectT = new List<T>();
            SQLiteCommand cmd = new SQLiteCommand(con);
            string stm =  $"SELECT * FROM {TableName}";
            
            cmd.CommandText = stm;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                DataTable dataTable = new DataTable(TableName);
                dataTable.Load(reader);
                selectT = DataTableToList<T>(dataTable);
            }
            con.Close();
            return selectT;
        }
        public static List<T> DataTableToList<T>(this DataTable table) where T : new()
        {
            List<T> list = new List<T>();
            var typeProperties = typeof(T).GetProperties().Select(propertyInfo => new
            {
                PropertyInfo = propertyInfo,
                Type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType
            }).ToList();

            foreach (DataRow row in table.Rows.Cast<DataRow>())
            {
                T obj = new T();
                foreach (var typeProperty in typeProperties)
                {
                    object value = row[typeProperty.PropertyInfo.Name];
                    object safeValue = value == null || DBNull.Value.Equals(value)
                        ? null
                        : Convert.ChangeType(value, typeProperty.Type);

                    typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                }
                list.Add(obj);
            }
            return list;
        }
    }
}
