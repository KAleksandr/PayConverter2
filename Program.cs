using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftGenConverter
{

    static class Program
    {
        //private static string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverterData.xml");
        //static string strData = Properties.Resources.PayConverterData;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Xml.isExistsFile(path2, strData);
            //Thread.Sleep(300);
            Application.Run(new Form1());
        }
    }
}
