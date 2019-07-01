using System;

using System.Drawing;

using System.Windows.Forms;

namespace SoftGenConverter
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            this.Size = new Size(558, 408);
        }

        public void InfoSettings()
        {
            axWindowsMediaPlayer1.Visible = false;
            this.Size = new Size(558, 408);
            this.CenterToScreen();
        }
        void VplayerSettings()
        {
            this.Size = new Size(960, 500);
            axWindowsMediaPlayer1.Visible = true;
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            this.CenterToScreen();


        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string video1 =
                axWindowsMediaPlayer1.URL = "D:" + @"\" + "Файли для ковертації" + @"\" +
                                            "openFile.MP4";
            VplayerSettings();
            


        }

        private void Button3_Click(object sender, EventArgs e)
        {
            InfoSettings();
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
            InfoSettings();
            richTextBox1.Text = "Файл який містить у собі список платіжних доручень";

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            InfoSettings();
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
            InfoSettings();
            richTextBox1.Text = "Программа зберігає файли для імпорту у два етапи, спочатку відкриється вікно збереження файлу" +
                                " для УкрГаз банку де вам буде необхідно обрати місце збереження файлу і після того як программа запише" +
                                " файл відкриється вікно збереження файлу для Райфайзен Банку Аваль";
        }
    }
}
