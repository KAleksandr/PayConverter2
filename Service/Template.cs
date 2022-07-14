using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SoftGenConverter.Service
{
    internal class Template
    {
        public static void GetExcel(List<Entity.Oschad> oschads, string nameFile, ProgressBar progressBar)
        {

            //progBar.Maximum = 2;


            string RunningPath = AppDomain.CurrentDomain.BaseDirectory + "Resources";
            byte[] fl = Properties.Resources.Template;
            string fileName = "Template.xlsx";


            string startupPath = System.IO.Directory.GetCurrentDirectory();

            FileInfo template = new FileInfo(RunningPath + @"\" + fileName);

            if (!template.Exists)
            {  //Делаем проверку - если Template.xlsx отсутствует - выходим по красной ветке

                MessageBox.Show("Упс! Файл Excel-шаблона {0} відсутній в каталозі проєкту", fileName);

            }
            else
            {

               
                CreateNewFile(template, nameFile, oschads, progressBar);

                if (File.Exists(nameFile))
                {
                    MessageBox.Show($"Файл сформовано! {nameFile}");
                }
                
                progressBar.Value = 1;
                progressBar.Visible = false;
            }
        }

        private static void CreateNewFile(FileInfo template, string saveName, List<Entity.Oschad> oschads, ProgressBar progressBar)
        {
            using (ExcelPackage exPack = new ExcelPackage(template, true))
            {

                ExcelWorksheet ws0 = exPack.Workbook.Worksheets[0];
                SetTemplate(ws0, oschads, progressBar);

                //exPack.Workbook.Worksheets.Delete(ws0);
                Byte[] bin = exPack.GetAsByteArray();

                if (File.Exists(saveName))
                {
                    saveName += DateTime.Now.ToLongDateString();
                }
                File.WriteAllBytes(saveName, bin);



            }
        }

        private static void SetTemplate(ExcelWorksheet ws, List<Entity.Oschad> oschads, ProgressBar progressBar1)
        {
            progressBar1.Visible = true;
            ModifyProgressBarColor.SetState(progressBar1, 3);
            progressBar1.Minimum = 1;
            progressBar1.Maximum = oschads.Count + 1;
            progressBar1.Value = 1;
            progressBar1.Step = 1;
            int start = 3;


            oschads.ForEach(dr => {
                ws.Cells[start, 1].Value = dr.Ndoc;
                ws.Cells[start, 2].Value = dr.Dt;
                ws.Cells[start, 3].Value = dr.Dv;
                ws.Cells[start, 4].Value = dr.Acccli;
                ws.Cells[start, 5].Value = dr.Acccor;
                ws.Cells[start, 6].Value = dr.Okpocor;
                ws.Cells[start, 7].Value = dr.Namecor;
                ws.Cells[start, 8].Value = dr.Summa;
                ws.Cells[start, 9].Value = dr.Val;
                ws.Cells[start, 10].Value = dr.Nazn;
                ws.Cells[start, 11].Value = dr.Cod_cor;
                ws.Cells[start, 12].Value = dr.Add_req;



                //ws.Cells[start, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
               
                ws.Cells[start, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[start, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
               
                start++;
                progressBar1.PerformStep();
            });

            
        }

   
        public static List<Entity.Oschad> ConvertTableToOschad(DataGridView dataGridView2, int docnum,string rahunok, bool anotherPayCh)
        {
            int codeVal = 980;
            int countryCode = 804;
            List<Entity.Oschad> oschads = new List<Entity.Oschad>();
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                int summa = Convert.ToInt32(row.Cells[8].Value.ToString().Replace(".", ""));
                string naznPl = anotherPayCh ? string.Join(" ", row.Cells[13].Value.ToString().Trim(), row.Cells[11].Value.ToString().Trim()) : row.Cells[11].Value.ToString().Trim();
                Entity.Oschad oschad = new Entity.Oschad()
                {
                    Ndoc    = docnum.ToString(),//1 ndoc
                    Dt      = DateTime.Now.Date, //2 dt  
                    Dv      = DateTime.Now.Date, //3 dv
                    Acccli  = rahunok,//row.Cells[6].Value.ToString(), //4 acccli!!!!!
                    Acccor  = row.Cells[7].Value.ToString(), //5 acccor
                    Okpocor = row.Cells[12].Value.ToString(), //6 okpocor
                    Namecor = row.Cells[10].Value.ToString(),  //7 namecor
                    Summa   = summa, //8 summa
                    Val     = codeVal, //9 val
                    Nazn    = naznPl, //10 nazn
                    Cod_cor = countryCode, //cod_cor 11
                    Add_req = "" //add_req 12
                };
                oschads.Add(oschad);
                docnum++;
            }
            return oschads;

        }

    }
}
