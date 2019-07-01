using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SoftGenConverter
{
    public partial class Form3 : Form
    { 
        private string url1 = "https://github.com/KAleksandr/PayConverter2/blob/master/manual/openFile.MP4?raw=true";
        private string nameFile1 = "openFile.MP4";
        public Form3()
        {
            InitializeComponent();
            this.Size = new Size(558, 408);
        }

       
       

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            


        }

        private void Button3_Click(object sender, EventArgs e)
        {
            
            richTextBox1.Text = "База шаблонів зберігає усю раніше введену інформацію"+
                                " по платникам та призначенням платежів для автоматичної підстановки цих данних"+
                                "у файл імпорту для банківських онлайн систем таких як Райфайзен Банк Аваль та УкрГаз Банк"+
                                ", також після того як ви вже завантажили файл для конвертації у цьому вікні ви можете" +
                                "відредагувати існуючу інформацію або додати нову.";
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Файл який містить у собі список платіжних доручень";

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Редагування здійснюється подвійним натиском лівої кнопки мишки по необхідній " +
                                "комірці таблиці, після чого поле стане доступним для зміни," +
                                "якщо у комірці є запис 'null' це значить що у базі шаблонів платежів" +
                                " відсутні дані по цьому платнику і їх необхідно заповнити, після чого " +
                                "інформація буде збережена у базі шаблонів платежів і коли наступного разу " +
                                "будуть платежі по цьому платнику то программа" +
                                "автоматично заповнить це поле. ";

        }

        private void Button8_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Программа зберігає файли для імпорту у два етапи, спочатку відкриється вікно збереження файлу" +
                                " для УкрГаз банку де вам буде необхідно обрати місце збереження файлу і після того як программа запише" +
                                " файл відкриється вікно збереження файлу для Райфайзен Банку Аваль";
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            play(url1, nameFile1);
            

        }
        public void DownloadFile(Uri adress, string fileName)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2 в .net Framework 4.0 додати
            using (WebClient wc = new WebClient())
            {

                //wc.DownloadProgressChanged += (s, te) => { progressBar1.Value = te.ProgressPercentage; };

                wc.DownloadFileAsync(adress, fileName);

            }
        }

        public void play(string url, string nameFile)
        {
            //string directory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}" + "\\manual";
            string directory = $@"{AppDomain.CurrentDomain.BaseDirectory}" + "\\manual";
            bool exists = System.IO.Directory.Exists(directory);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(directory);
            }
           
            string name = Path.Combine(directory , nameFile);
            if (File.Exists(name))
            {
                Process.Start(name);
            }
            else
            {
                DownloadFile(new Uri(url), name);
                Thread.Sleep(400);
                play(url, nameFile);
            }
        }

    }
}
