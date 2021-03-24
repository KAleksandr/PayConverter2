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
        private Font myfont = new Font("Times New Roman", 12.0f, FontStyle.Regular, GraphicsUnit.Point);
        private string nameFile1 = "openFile.avi";
        private string nameFile2 = "payBase.avi";
        private string nameFile3 = "editBase.MP4";
        private string nameFile4 = "saveFile.MP4";
        private string url1 = "https://github.com/KAleksandr/PayConverter2/blob/master/manual/openFile.avi?raw=true";
        private string url2 = "https://github.com/KAleksandr/PayConverter2/blob/master/manual/payBase.avi?raw=true";
        private string url3 = "https://github.com/KAleksandr/PayConverter2/blob/master/manual/editBase.MP4?raw=true";
        private string url4 = "https://github.com/KAleksandr/PayConverter2/blob/master/manual/saveFile.MP4?raw=true";

        public Form3()
        {
            InitializeComponent();
            // this.Size = new Size(600, 352);
        }


        private void Form3_Load(object sender, EventArgs e)
        {
            richTextBox1.Font = myfont;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            play(url1, nameFile1);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;

            richTextBox1.Text = "База шаблонів зберігає усю раніше введену інформацію" +
                                " по платникам та призначенням платежів для автоматичної підстановки цих данних " +
                                "у файл імпорту для банківських онлайн систем таких як Райфайзен Банк Аваль та УкрГаз Банк" +
                                ", також після того як ви вже завантажили файл для конвертації у цьому вікні ви можете " +
                                "відредагувати існуючу інформацію або додати нову.";
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Файл який містить у собі список платіжних доручень";
            pictureBox1.Visible = false;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Редагування здійснюється подвійним натиском лівої кнопки мишки по необхідній " +
                                "комірці таблиці, після чого поле стане доступним для зміни, " +
                                "якщо у комірці є запис 'null' це значить що у базі шаблонів платежів" +
                                " відсутні дані по цьому платнику і їх необхідно заповнити, після чого " +
                                "інформація буде збережена у базі шаблонів платежів і коли наступного разу " +
                                "будуть платежі по цьому платнику то программа " +
                                "автоматично заповнить це поле. ";
            pictureBox1.Visible = false;
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            richTextBox1.Text =
                "Программа зберігає файли для імпорту у два етапи, спочатку відкриється вікно збереження файлу" +
                " для УкрГаз банку де вам буде необхідно обрати місце збереження файлу і після того як программа запише" +
                " файл відкриється вікно збереження файлу для Райфайзен Банку Аваль";
            pictureBox1.Visible = false;
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            play(url2, nameFile2);
        }

        public void DownloadFile(Uri adress, string fileName)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072; //TLS 1.2 в .net Framework 4.0 додати
            using (var wc = new WebClient())
            {
                //wc.DownloadProgressChanged += (s, te) => { progressBar1.Value = te.ProgressPercentage; };

                wc.DownloadFileAsync(adress, fileName);
            }
        }

        public void play(string url, string nameFile)
        {
            //string directory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}" + "\\manual";
            var directory = $@"{AppDomain.CurrentDomain.BaseDirectory}" + "\\manual";
            var exists = Directory.Exists(directory);
            if (!exists) Directory.CreateDirectory(directory);

            var name = Path.Combine(directory, nameFile);
            if (File.Exists(name) && new FileInfo(name).Length != 0)
                Process.Start(name);
            else
                try
                {
                    DownloadFile(new Uri(url), name);
                    Thread.Sleep(400);
                    play(url, nameFile);
                }
                catch (WebException)
                {
                    MessageBox.Show(
                        "З´єднання з інтрнетом відсутнє," + Environment.NewLine + "неможливо завантажити файл довідки.",
                        "Помилка з´єднання", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            play(url3, nameFile3);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            play(url4, nameFile4);
        }
    }
}