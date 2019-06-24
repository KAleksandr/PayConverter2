using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;

using System.Text;

using System.Windows.Forms;


using System.Text.RegularExpressions;

namespace SoftGenConverter
{
    public partial class Form2 : Form
    {
        private bool edit = false;
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data.xml");
        public Form2()
        {
            InitializeComponent();

            Xml.loadXml(dataGridView1, path);
           
            Aval.StyleDataGridView(dataGridView1, false);
            try
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            }
            catch (Exception) { }
          
           // RemoveDuplicate();

        }

        //public string shortText(string str)
        //{
        //    str = str.Replace("  ", @" ");
        //    str = str.Replace("утримання", "утрим.").Replace("будинків", "буд.").Replace("утриман.", "утрим.").Replace("управління", "управл.").Replace("будинку", @"буд.").Replace("комунальні","комун. ").Replace("комунальних","комун. ").Replace("послуги","посл. ").Replace("послуг", "посл. ");
        //    return str;
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = Aval.shortText(textBox1.Text);
            //textBox1.Text = textBox1.Text.Replace("утримання", "утрим.").Replace("будинків", "буд.").Replace("утриман.", "утрим.").Replace("управління", "управл.").Replace("  ",@" ");
            if (ederpo.Text == "" || textBox1.Text == "")
                {
                        MessageBox.Show("Заповніть всі поля.", "Помилка.");
                }
            else if (textBox1.Text.Length > 160)
            {
                

                MessageBox.Show("Перевищено мінімальну кількість символів (160) - "+ textBox1.Text.Length, "Помилка.");
            }
            else 
                if (!edit)
                {
                    string pattern = @"за\s[0-9]{2}[.][0-9]{2}[.][0-9]{4}р\.";
                    string newLine = Regex.Replace(textBox1.Text, pattern, "  за ##.##.#### ");

                    int n = dataGridView1.Rows.Add();
                   dataGridView1.Rows[n].Cells[0].Value = textBox2.Text; // 
                   dataGridView1.Rows[n].Cells[1].Value = ederpo.Text; // 
                   dataGridView1.Rows[n].Cells[2].Value = newLine; // 
                }
                else
                {
                    string pattern = @"за\s[0-9]{2}[.][0-9]{2}[.][0-9]{4}р\.";
                    string newLine = Regex.Replace(textBox1.Text, pattern, "  за ##.##.#### ");
                    dataGridView1.CurrentRow.Cells[0].Value = textBox2.Text; // 
                    dataGridView1.CurrentRow.Cells[1].Value = ederpo.Text; // 
                    dataGridView1.CurrentRow.Cells[2].Value = newLine; // 
                    edit = !edit;
                }

            if (textBox1.Text.Length <= 160)
            {
                button1.Text = "Додати";
                ederpo.Text = string.Empty;
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
                dataGridView1.Refresh();
            }
                
        }

        public void loadXml()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows.Clear();
            }
            //if (dataGridView1.Rows.Count > 0) //если в таблице больше нуля строк
            //{
            //    MessageBox.Show("Очистите поле перед загрузкой нового файла.", "Ошибка.");
            //}
            //else
            {
                if (File.Exists(path)) //
                {
                    DataSet ds = new DataSet(); 
                    ds.ReadXml(path);
                    try
                    {
                     foreach (DataRow item in ds.Tables["Employee"].Rows)
                     {
                                int n = dataGridView1.Rows.Add(); 
                                dataGridView1.Rows[n].Cells[0].Value = item["NAME"]; 
                                dataGridView1.Rows[n].Cells[1].Value = item["ERDPO"]; 
                                dataGridView1.Rows[n].Cells[2].Value = item["Comment"]; 

                     }
                    }catch(Exception) { }
                    
                }
                else
                {
                    MessageBox.Show("XML файл не найден.", "Ошибка.");
                }
            }
        }

    


        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Xml.saveXml(dataGridView1, path);
            this.DialogResult = DialogResult.OK;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if (dataGridView1.Rows.Count > 0)
            //{
            //    dataGridView1.Rows.Clear();
            //}
            List<Cargo> CSV_Struct = new List<Cargo>();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"export.csv");
            CSV_Struct = Cargo.ReadFile(path);

            //Заполняем listView из нашей структуры
            for (int i = 0; i <= CSV_Struct.Count - 1; i++)
            {
                if (i != 0)
                {
                    //MessageBox.Show(i.ToString());
                    

                    int n = dataGridView1.Rows.Add();
                    dataGridView1.Rows[n].Cells[0].Value = CSV_Struct[i].Name.Replace("@","\"");
                    dataGridView1.Rows[n].Cells[1].Value = CSV_Struct[i].List_price;
                    dataGridView1.Rows[n].Cells[2].Value = CSV_Struct[i].RRahunok.ToString();
                    dataGridView1.Rows[n].Cells[3].Value = CSV_Struct[i].MyPrice;
                }

            }
        }
        public class Cargo
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string List_price { get; set; }
            public string MyPrice { get; set; }
            public string RRahunok { get; set; }
            public override string ToString()
            {
                return Name + " " + List_price +" " + MyPrice;
            }


            //Метод для получения частей из строки
            public void piece(string line)
            {

                string[] parts = line.Split(';');  //Разделитель в CSV файле.

                ID = parts[0].Replace("\"", "");
                Name = parts[19].Replace("", "");
                List_price = parts[20].Replace("", "");
                // Regex regexDate = new Regex(@"[за ][0-9]{2}[.][0-9]{2}[.][0-9]{2}[р.]");
               // string pattern = @"за\s[0-9]{2}[.][0-9]{2}[.][0-9]{4}р\.";
               // string text = parts[23].Replace("\"", "");
                //string yes = Regex.Replace(text, pattern, "  за ##.##.#### ");

                //MyPrice = yes;
                MyPrice = parts[23]; 
            }
            public void exportPrplat(string line)
            {

                string[] parts = line.Split(';');  //Разделитель в CSV файле.

                ID = parts[0];
                Name = parts[10];
                List_price = parts[9];
                RRahunok = parts[8];
                // Regex regexDate = new Regex(@"[за ][0-9]{2}[.][0-9]{2}[.][0-9]{2}[р.]");
                string pattern = @"за\s?[0-9]{2}[.][0-9]{2}[.][0-9]{4}р.";
                string text = parts[15];
                string yes = Regex.Replace(text, pattern, "  за ##.##.#### ");

                MyPrice = yes;
            }
            public static List<Cargo> ReadFile(string filename)
            {
                
                List<Cargo> res = new List<Cargo>();
                using (StreamReader sr = new StreamReader(filename, Encoding.GetEncoding(1251)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        
                            Cargo p = new Cargo();
                            p.exportPrplat(line);
                            res.Add(p);
                            //MessageBox.Show(p.ToString());
                        
                    }
                }

               
                return res;
            }
        }
        public void RemoveDuplicate()
        {
            for (int currentRow = 0; currentRow < dataGridView1.Rows.Count - 1; currentRow++)
            {
                DataGridViewRow rowToCompare = dataGridView1.Rows[currentRow];

                for (int otherRow = currentRow + 1; otherRow < dataGridView1.Rows.Count; otherRow++)
                {
                    DataGridViewRow row = dataGridView1.Rows[otherRow];

                    bool duplicateRow = true;

                    for (int cellIndex = 0; cellIndex < row.Cells.Count; cellIndex++)
                    {
                        rowToCompare.Cells[3].Value = Aval.shortText(rowToCompare.Cells[3].Value.ToString());
                        string pattern = @"за\s?[0-9]{2}[.][0-9]{2}[.][0-9]{4}р.";

                        rowToCompare.Cells[3].Value = Regex.Replace(rowToCompare.Cells[3].Value.ToString(), pattern, "  за ##.##.#### ");

                        //rowToCompare.Cells[2].Value = rowToCompare.Cells[2].Value.ToString().Replace("  ", @" ");
                        //rowToCompare.Cells[2].Value = rowToCompare.Cells[2].Value.ToString().Replace("утримання", "утрим.").Replace("будинків", "буд.").Replace("утриман.", "утрим.").Replace("управління", "управл.");

                        if (!rowToCompare.Cells[1].Value.Equals(row.Cells[1].Value) && !rowToCompare.Cells[2].Value.Equals(row.Cells[2].Value) )
                        {
                            //MessageBox.Show(rowToCompare.Cells[cellIndex+1].Value.ToString() + "   -  " +row.Cells[cellIndex+1].Value);

                            duplicateRow = false;
                            break;
                            
                        }

                    }

                    if (duplicateRow)
                    {
                        dataGridView1.Rows.Remove(row);
                        otherRow--;
                    }
                }
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(dataGridView1.CurrentRow.Cells[2].Value.ToString()))
                {
                    dataGridView1.CurrentRow.Cells[2].Value = "null";
                }
                dataGridView1.CurrentRow.Cells[3].Value = Aval.shortText(dataGridView1.CurrentRow.Cells[2].Value.ToString());
                textBox2.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                ederpo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                textBox1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            }
            catch (Exception) { }
            
            button1.Text = "Редагувати";
            
            edit = !edit;
            
        }

      

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                textBox2.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                ederpo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
              
            textBox1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            }
            catch (Exception )
            {
                
            }
            
            button1.Text = "Редагувати";
            edit = !edit;
           
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            button1.Text = "Додати";
            ederpo.Text = string.Empty;
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int id = dataGridView1.CurrentCell.RowIndex;
            try
            {
                if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "" || dataGridView1.CurrentRow.Cells[2].Value.ToString() == "")
                {
                    MessageBox.Show("Заповніть всі поля.", "Помилка.");
                    dataGridView1.CurrentRow.Cells[2].Value = "null";
                }
                else if (textBox1.Text.Length > 160)
                {


                    MessageBox.Show("Перевищено мінімальну кількість символів (160) - " + textBox1.Text.Length, "Помилка.");
                }
                dataGridView1.CurrentRow.Cells[3].Value =
                    Aval.shortText(dataGridView1.CurrentRow.Cells[2].Value.ToString());
                
            }
            catch (Exception)
            {

            }
           
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }
    }
}

