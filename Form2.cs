using System;
using System.Collections.Generic;

using System.Data;

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
           Aval.StyleDataGridView(dataGridView1, true);
            RemoveDuplicate();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ederpo.Text == "" || textBox1.Text == "")
                {
                        MessageBox.Show("Заповніть всі поля.", "Помилка.");
                }
            else 
            if (!edit)
            {
                
               int n = dataGridView1.Rows.Add();
               dataGridView1.Rows[n].Cells[0].Value = textBox2.Text; // 
               dataGridView1.Rows[n].Cells[1].Value = ederpo.Text; // 
               dataGridView1.Rows[n].Cells[2].Value = textBox1.Text; // 
            }
            else
            {
                
                dataGridView1.CurrentRow.Cells[0].Value = textBox2.Text; // 
                dataGridView1.CurrentRow.Cells[1].Value = ederpo.Text; // 
                dataGridView1.CurrentRow.Cells[2].Value = textBox1.Text; // 
                edit = !edit;
            }
            button1.Text = "Додати";
            ederpo.Text = string.Empty;
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
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
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Cargo> CSV_Struct = new List<Cargo>();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"12.csv");
            CSV_Struct = Cargo.ReadFile(path);

            //Заполняем listView из нашей структуры
            for (int i = 0; i <= CSV_Struct.Count - 1; i++)
            {
                if (i != 0)
                {
                    //MessageBox.Show(i.ToString());
                    

                    int n = dataGridView1.Rows.Add();
                    dataGridView1.Rows[n].Cells[0].Value = CSV_Struct[i].Name;
                    dataGridView1.Rows[n].Cells[1].Value = CSV_Struct[i].List_price;
                    dataGridView1.Rows[n].Cells[2].Value = CSV_Struct[i].MyPrice;
                }

            }
        }
        public class Cargo
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string List_price { get; set; }
            public string MyPrice { get; set; }


            //Метод для получения частей из строки
            public void piece(string line)
            {

                string[] parts = line.Split(';');  //Разделитель в CSV файле.

                ID = parts[0].Replace("\"", "");
                Name = parts[19].Replace("\"", "");
                List_price = parts[20].Replace("\"", "");
                // Regex regexDate = new Regex(@"[за ][0-9]{2}[.][0-9]{2}[.][0-9]{2}[р.]");
                string pattern = @"за\s[0-9]{2}[.][0-9]{2}[.][0-9]{4}р\.";
                string text = parts[23].Replace("\"", "");
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
                        p.piece(line);
                        res.Add(p);
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
                        if (!rowToCompare.Cells[1].Value.Equals(row.Cells[1].Value) && !rowToCompare.Cells[0].Value.Equals(row.Cells[0].Value))
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
            textBox2.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            ederpo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            button1.Text = "Редагувати";
            edit = !edit; 

        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            button1.Text = "Додати";
            ederpo.Text = string.Empty;
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
        }
        
    }
}

