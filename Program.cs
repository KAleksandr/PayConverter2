using System;
using System.Windows.Forms;

namespace SoftGenConverter
{
    internal static class Program
    {
        //private static string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterData.xml");
        //static string strData = Properties.Resources.PayConverterData;
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Xml.isExistsFile(path2, strData);
            //Thread.Sleep(300);
            Application.Run(new Form1());
        }
    }
}