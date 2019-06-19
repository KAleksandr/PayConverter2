using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftGenConverter
{
    class ExportToExcel
    {
        public static void saveExcel(SaveFileDialog saveDialog, DataGridView dataGridView1, Datashit recviz)
        {
            long numDoc = recviz.platNumber2;
            string senderRah = recviz.rahunok2;
            string edrpou = recviz.ToString();
            
            // Creating a Excel object.
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;


            //worksheet.Cells.Style;
            try
            {

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "ExportedFromDatGrid";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;

                //Loop through each row and read value from each column.
                // worksheet.Cells["D:D"].NumberFormat = "@";
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        
                        // Excel index starts from 1,1. As first Row would have the Column headers, adding a condition check.
                        switch (cellColumnIndex)
                        {
                            case 6:
                            
                                    if (cellRowIndex == 1) worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Columns[j + 1].HeaderText;
                                    else worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i].Cells[j + 1].Value.ToString();
                                    break;
                            case 7:
                                if (cellRowIndex == 1) worksheet.Cells[cellRowIndex, cellColumnIndex] = "Номер платежу";
                                else worksheet.Cells[cellRowIndex, cellColumnIndex] = numDoc++;
                                break;
                            case 8:
                                if (cellRowIndex == 1) {
                                    worksheet.Cells[cellRowIndex, cellColumnIndex].NumberFormat = "@";
                                    worksheet.Cells[cellRowIndex, cellColumnIndex] = "ЄРДПО платника";}
                                else worksheet.Cells[cellRowIndex, cellColumnIndex] = recviz.edrpou;
                                break;
                            case 9:
                                if (cellRowIndex == 1)
                                {
                                    worksheet.Cells[cellRowIndex, cellColumnIndex] = "Рахунок платника";
                                    worksheet.Cells[cellRowIndex, cellColumnIndex].NumberFormat = "@";
                                }
                                else worksheet.Cells[cellRowIndex, cellColumnIndex] = recviz.rahunok2;
                                break;

                            case 10:
                            case 11:
                                break;
                            default:
                                if (cellRowIndex == 1) worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Columns[j].HeaderText;
                                else
                                {
                                    worksheet.Cells[cellRowIndex, cellColumnIndex].NumberFormat = "@";
                                    worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                                }
                                break;
                        }

                       
                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }


                //                SaveFileDialog saveDialog = new SaveFileDialog();
                //                saveDialog.Filter = "Excel files(2003)| *.xls|Excel Files(2007+)|*.xlsx"; ;
                //                saveDialog.FilterIndex = 2;

                //if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveDialog.FileName);
                    MessageBox.Show("Експорт завершено");
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
