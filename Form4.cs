using SoftGenConverter.Entity;
using SoftGenConverter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace SoftGenConverter
{
    public partial class Form4 : Form
    {
        private string FilterText { get; set; } = "";
        private readonly string tableName = "PurposeOfPayment";
        private string SortText { get; set; } = "";
        private Image editBtn = Resources.Form2EditLine_32; //
        private Image saveBtn = Resources.form2Add_32;
        private bool Save = true;
        public Form4()
        {
            InitializeComponent();
            InitDataSource();
            advancedDataGridView1.Columns["ID"].HeaderText = "ІД";
            advancedDataGridView1.Columns["NAME"].HeaderText = "Найменування отримувача";
            advancedDataGridView1.Columns["NAME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            advancedDataGridView1.Columns["PURPOSE"].HeaderText = "Призначення платежу за умовчанням";
            advancedDataGridView1.Columns["PURPOSE"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        }
        public Form4(List<PurposeOfPayment> data)
        {

            InitializeComponent();
            advancedDataGridView1.DataSource = data;

            advancedDataGridView1.Columns["ID"].HeaderText = "ІД";
            advancedDataGridView1.Columns["NAME"].HeaderText = "Найменування отримувача";
            advancedDataGridView1.Columns["NAME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            advancedDataGridView1.Columns["PURPOSE"].HeaderText = "Призначення платежу за умовчанням";
            advancedDataGridView1.Columns["PURPOSE"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        }

        private void advancedDataGridView1_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {

            FilterText = advancedDataGridView1.FilterString;
            if (advancedDataGridView1 == null)
            {
                InitDataSource();
            }
            InitDataSource(FilterText, SortText);



        }

        private void advancedDataGridView1_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            SortText = advancedDataGridView1.SortString;

            InitDataSource(FilterText, SortText);

        }

        private void advancedDataGridView1_Scroll(object sender, ScrollEventArgs e)
        {

            try
            {
                advancedDataGridView1.CurrentRow.Selected = false;
            }
            catch (NullReferenceException)
            {
            }
            Clear();
            ChangeImageAdd();

        }

        private void ADD_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(NAME.Text) && !string.IsNullOrEmpty(PURPOSE.Text))
            {


                if (Save)//Зберігаємо в базу
                {
                    PurposeOfPayment_.InsertData(new PurposeOfPayment() { NAME = NAME.Text.Trim(), PURPOSE = PURPOSE.Text.Trim() }, out long id);
                    InitDataSource();
                    Clear();
                }
                else//Оновлюємо
                {
                    Int32.TryParse(ID.Text, out int id);
                    PurposeOfPayment_.UpdatePurpose(new PurposeOfPayment() { ID = id, NAME = NAME.Text.Trim(), PURPOSE = PURPOSE.Text.Trim() });
                    InitDataSource();
                    Clear();
                }
            }
            else
            {
                MessageBox.Show("Заповніть всі поля.", "Помилка.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ChangeImageAdd()
        {
            ADD.Image = Save ? saveBtn : editBtn;
            //DELETE.Visible = !Save;
        }
        private void Clear()
        {
            ID.Text = string.Empty;
            NAME.Text = string.Empty;
            PURPOSE.Text = string.Empty;
            Save = true;
            ChangeImageAdd();
        }
        private void InitDataSource()
        {
            PurposeOfPayment_.DeleteDublicate();
            advancedDataGridView1.DataSource = Db.SelectTable<PurposeOfPayment>(tableName);

        }
        private void InitDataSource(string filterText, string sortText)
        {
            advancedDataGridView1.DataSource = Db.SelectTable<PurposeOfPayment>(tableName, filterText, sortText);

        }

        private void advancedDataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ID.Text = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                NAME.Text = advancedDataGridView1.CurrentRow.Cells[1].Value.ToString();
                PURPOSE.Text = advancedDataGridView1.CurrentRow.Cells[2].Value.ToString();


            }
            catch (NullReferenceException)
            {
            }
            DELETE.Visible = true;
            ADD.Image = editBtn;
            Save = false;
        }

        private void DELETE_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ID.Text))
            {
                Int32.TryParse(ID.Text, out int id);
                Db.DeleteById(tableName, id);

                InitDataSource();
                Clear();
            }
            else
            {
                List<int> idDs = new List<int>();
               for(int i=0; i< advancedDataGridView1.Rows.Count; i++)
                {
                    DataGridViewRow startingBalanceRow = advancedDataGridView1.Rows[i];
                    if (advancedDataGridView1.SelectedRows.Contains(startingBalanceRow))
                    {
                        Int32.TryParse(advancedDataGridView1.Rows[i].Cells[0].Value.ToString(), out int id);
                        idDs.Add(id);
                        
                    }
                }
                if (idDs.Count > 0)
                {
                    DialogResult dialogResult = MessageBox.Show($"Видалити записи з ІД ({string.Join(",", idDs)})?", "Видалення!", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Db.DeleteById(tableName, idDs);
                        InitDataSource();
                        Clear();
                    }
                    
                }
               

            }

        }

        private void advancedDataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                PurposeOfPayment_.UpdatePurpose(new PurposeOfPayment() { ID = e.RowIndex, NAME = advancedDataGridView1.CurrentRow.Cells[1].Value.ToString().Trim(), PURPOSE = advancedDataGridView1.CurrentRow.Cells[2].Value.ToString().Trim() });
                InitDataSource();
                Clear();
            }


        }

        private void advancedDataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (e.RowIndex != -1)
            {

                //Db.DeleteById(tableName, e.RowIndex);

                //InitDataSource();
                //Clear();
            }
        }

        private void advancedDataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Clear();
           // MessageBox.Show("YEPS");
        }
    }
}
