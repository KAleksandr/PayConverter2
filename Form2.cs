using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SoftGenConverter.Properties;

namespace SoftGenConverter
{
    public partial class Form2 : Form
    {
        private BindingSource baseB = new BindingSource();
        private bool edit;
        private Image editBtn = Resources.Form2EditLine_32; //
        private Image saveBtn = Resources.form2Add_32;

        public Form2()
        {
            InitializeComponent();

            Xml.loadXml(dataGridView1, path);

            MyDataGrid.StyleDataGridView(dataGridView1, false);
            try
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            }
            catch (NullReferenceException)
            {
            }

            RemoveDuplicate();
            Xml.ReWriteFile(path);
            Xml.saveXml(dataGridView1, path);
            baseB.DataSource = dataGridView1.DataSource;

            SetDoubleBuffered(dataGridView1, true);
        }


        public Form2(string paths)
        {
            //MessageBox.Show(paths);
            InitializeComponent();
            this.paths = paths;
            Xml.loadXml(dataGridView1, paths);

            MyDataGrid.StyleDataGridView(dataGridView1, false);
            try
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            }
            catch (NullReferenceException)
            {
            }

            RemoveDuplicate();
            baseB.DataSource = dataGridView1.DataSource;

            SetDoubleBuffered(dataGridView1, true);
        }

        public string
            path { get; set; } //= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterData.xml");

        public string paths { get; set; }

        private void SetDoubleBuffered(Control c, bool value)
        {
            var pi = typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic);
            if (pi != null) pi.SetValue(c, value, null);
        }

        public void loadXml()
        {
            if (dataGridView1.Rows.Count > 0) dataGridView1.Rows.Clear();

            {
                if (File.Exists(path)) //
                {
                    var ds = new DataSet();
                    ds.ReadXml(path);
                    try
                    {
                        foreach (DataRow item in ds.Tables["Employee"].Rows)
                        {
                            var n = dataGridView1.Rows.Add();
                            dataGridView1.Rows[n].Cells[0].Value = item["NAME"];
                            dataGridView1.Rows[n].Cells[1].Value = item["ERDPO"];
                            dataGridView1.Rows[n].Cells[2].Value = item["Comment"];
                        }
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
                else
                {
                    MessageBox.Show("XML файл не найден.", "Ошибка.");
                }
            }
        }


        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Xml.saveXml(dataGridView1, paths);
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var CSV_Struct = new List<Cargo>();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"export.csv");
            CSV_Struct = Cargo.ReadFile(path);

            //Заполняем listView из нашей структуры
            for (var i = 0; i <= CSV_Struct.Count - 1; i++)
                if (i != 0)
                {
                    var n = dataGridView1.Rows.Add();
                    dataGridView1.Rows[n].Cells[0].Value = CSV_Struct[i].Name.Replace("@", "\"");
                    dataGridView1.Rows[n].Cells[1].Value = CSV_Struct[i].List_price;
                    dataGridView1.Rows[n].Cells[2].Value = CSV_Struct[i].RRahunok;
                    dataGridView1.Rows[n].Cells[3].Value = CSV_Struct[i].MyPrice;
                }
        }

        public void RemoveDuplicate()
        {
            for (var currentRow = 0; currentRow < dataGridView1.Rows.Count - 1; currentRow++)
            {
                var rowToCompare = dataGridView1.Rows[currentRow];

                for (var otherRow = currentRow + 1; otherRow < dataGridView1.Rows.Count; otherRow++)
                {
                    var row = dataGridView1.Rows[otherRow];

                    var duplicateRow = true;

                    for (var cellIndex = 0; cellIndex < row.Cells.Count; cellIndex++)
                        // if (!rowToCompare.Cells[2].Value.Equals(row.Cells[2].Value) & row.Cells[3].Value.ToString().Length > 51)
                        // if (!rowToCompare.Cells[0].Value.Equals(row.Cells[0].Value))
                        // {
                        //     if (!rowToCompare.Cells[1].Value.Equals(row.Cells[1].Value))
                        //     {
                        //         if (!rowToCompare.Cells[2].Value.Equals(row.Cells[2].Value))
                        //         {
                        //           
                        //             if (!rowToCompare.Cells[3].Value.Equals(row.Cells[3].Value))
                        //             {
                        //                 duplicateRow = false;
                        //                 break;
                        //             }
                        //         }
                        //     }
                        // }
                        if (!rowToCompare.Cells[2].Value.Equals(row.Cells[2].Value))
                        {
                            duplicateRow = false;
                            break;
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
                if (string.IsNullOrEmpty(dataGridView1.CurrentRow.Cells[3].Value.ToString()))
                    dataGridView1.CurrentRow.Cells[3].Value = "null";
                dataGridView1.CurrentRow.Cells[3].Value =
                    MyDataGrid.shortText(dataGridView1.CurrentRow.Cells[3].Value.ToString());
                textBox2.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                ederpo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                textBox1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                textBox3.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            }
            catch (NullReferenceException)
            {
            }

            button1.Image = editBtn;
            edit = !edit;
        }

        public void fillFieldsD()
        {
            textBox2.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            ederpo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();

            textBox1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
        }

        public void fillFieldsDg()
        {
            var id = dataGridView1.CurrentRow.Index - 1;
            if (id < 0) id = 0;

            textBox2.Text = dataGridView1.Rows[id].Cells[0].Value.ToString();
            ederpo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                fillFieldsD();
            }
            catch (NullReferenceException)
            {
            }

            button1.Image = editBtn;
            edit = true;
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                dataGridView1.CurrentRow.Selected = false;
            }
            catch (NullReferenceException)
            {
            }

            edit = false;

            button1.Image = saveBtn;
            ederpo.Text = string.Empty;
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
        }


        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds =
                new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox3.Text = MyDataGrid.shortText(textBox3.Text);

            if (ederpo.Text == "" || textBox3.Text == "" || textBox1.Text == "")
            {
                MessageBox.Show("Заповніть всі поля.", "Порожнє поле.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (textBox3.Text.Length > 160)
            {
                MessageBox.Show("Перевищено мінімальну кількість символів (160) - " + textBox3.Text.Length, "Помилка.",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var newLine = MyDataGrid.convertDate(textBox3.Text);

                if (!edit)
                {
                    var n = dataGridView1.Rows.Add();
                    dataGridView1.Rows[n].Cells[0].Value = textBox2.Text; // 
                    dataGridView1.Rows[n].Cells[1].Value = ederpo.Text; // 
                    dataGridView1.Rows[n].Cells[2].Value = textBox1.Text;
                    dataGridView1.Rows[n].Cells[3].Value = newLine; // 
                }
                else
                {
                    dataGridView1.CurrentRow.Cells[0].Value = textBox2.Text; // 
                    dataGridView1.CurrentRow.Cells[1].Value = ederpo.Text; // 
                    dataGridView1.CurrentRow.Cells[2].Value = textBox1.Text; // 
                    dataGridView1.CurrentRow.Cells[3].Value = newLine; // 
                    edit = !edit;
                }
            }

            if (textBox1.Text.Length <= 160)
            {
                button1.Image = saveBtn;
                ederpo.Text = string.Empty;
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
                textBox3.Text = string.Empty;
                dataGridView1.Refresh();
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            int[] col = {0, 1, 2, 3};
            MyDataGrid.Filter(dataGridView1, textBox4.Text, col);
        }


        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow.Cells[3].Value.ToString() == "")
            {
                MessageBox.Show("Заповніть всі поля.", "Помилка.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dataGridView1.CurrentRow.Cells[3].Value = "null";
            }
            else if (dataGridView1.CurrentRow.Cells[3].ToString().Length > 160)
            {
                MessageBox.Show("Перевищено мінімальну кількість символів (160) - " + textBox1.Text.Length, "Помилка.",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            dataGridView1.CurrentRow.Cells[3].Value =
                MyDataGrid.shortText(dataGridView1.CurrentRow.Cells[3].Value.ToString());
            dataGridView1.CurrentRow.Cells[3].Value =
                MyDataGrid.convertDate(dataGridView1.CurrentRow.Cells[3].Value.ToString());
        }

        private void Form2_Load(object sender, EventArgs e)
        {
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
                return Name + " " + List_price + " " + MyPrice;
            }


            public void exportPrplat(string line)
            {
                var parts = line.Split(';'); //Разделитель в CSV файле.

                ID = parts[0];
                Name = parts[10];
                List_price = parts[9];
                RRahunok = parts[8];

                var text = parts[15];

                MyPrice = MyDataGrid.convertDate(text);
            }

            public static List<Cargo> ReadFile(string filename)
            {
                var res = new List<Cargo>();
                using (var sr = new StreamReader(filename, Encoding.GetEncoding(1251)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var p = new Cargo();
                        p.exportPrplat(line);
                        res.Add(p);
                        //MessageBox.Show(p.ToString());
                    }
                }


                return res;
            }
        }
    }
}