using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SoftGenConverter
{
    class MyDataGrid
    {
        public static void StyleDataGridView(DataGridView dgv, bool isReadonly = true)
        {
            try
            {
                // Setting the style of the DataGridView control
                dgv.RowHeadersVisible = true;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold, GraphicsUnit.Point);
                dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.ControlDark;
                dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.DefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Regular, GraphicsUnit.Point);
                dgv.DefaultCellStyle.BackColor = Color.Empty;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dgv.AllowUserToAddRows = false;
                dgv.ReadOnly = isReadonly;
                //dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
                System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle
                {
                    BackColor = Color.LightBlue
                };
                dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            }
            catch (Exception)
            {
            }
        }
        public static string shortText(string str)
        {
            string pattern = @"будин\S+";
            string pattern2 = @"комунал\S+";
            string pattern3 = @"послуг\S+";
            string pattern4 = @"утриман\S+";
            string pattern5 = @"управл\S+";

            str = Regex.Replace(str.Trim(), @"[^\S\r\n]+", " ");
            str = Regex.Replace(str, pattern, "буд. ");
            str = Regex.Replace(str, pattern2, "комун. ");
            str = Regex.Replace(str, pattern3, "посл. ");
            str = Regex.Replace(str, pattern4, "утрим. ");
            str = Regex.Replace(str, pattern5, "управл. ");

            return str;
        }
        public static void Filter(DataGridView dataGridView1, string foundText, int[] col)
        {
            string textF = foundText.Trim().ToLower();

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1[col[0], i].FormattedValue.ToString().ToLower().
                        Contains(textF) || dataGridView1[col[1], i].FormattedValue.ToString().ToLower().
                        Contains(textF) || dataGridView1[col[2], i].FormattedValue.ToString().ToLower().
                        Contains(textF) || dataGridView1[col[3], i].FormattedValue.ToString().ToLower().
                        Contains(textF))
                {
                    //MessageBox.Show(dataGridView1[1, i].FormattedValue.ToString());
                    dataGridView1.Rows[i].Selected = true;
                    dataGridView1.Rows[i].Visible = true;
                }
                else
                {
                    dataGridView1.Rows[i].Visible = false;
                    dataGridView1.Rows[i].Selected = false;

                }
            }

            if (string.IsNullOrEmpty(foundText))
            {
                dataGridView1.ClearSelection();
            }
        }
        public static string convertDate(string text)
        {
            string pattern = @"за\s?[0-9]{2}[.][0-9]{2}[.][0-9]{4}\s*р?.?";

            return Regex.Replace(text, pattern, "  за ##.##.#### ");
        }
    }
}
