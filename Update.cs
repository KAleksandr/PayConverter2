using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SoftGenConverter
{
    class Update
    {
        private string updater = "updater.exe";
        private string pCUpdate = "PayConverter.update";
        private int intVersion = Convert.ToInt32(Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", ""));
        private string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"version.xml");
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"update.exe");
        string url = "https://raw.githubusercontent.com/KAleksandr/testUpdate/master/version.xml";
        string url2 = "https://github.com/KAleksandr/testUpdate/blob/master/progressbar.exe?raw=true";
        string url3 = "https://github.com/KAleksandr/testUpdate/blob/master/updater.exe?raw=true";

        public void Download()
        {
        
            int ver;
            string versn;
            XmlDocument doc = new XmlDocument();
            try
            {
                DownloadFile(new Uri(url), path2);
                

                bool exists = System.IO.Directory.Exists(path2);
                    if (!exists)
                    {
                        doc.Load(path2);
                        versn = doc.GetElementsByTagName("myprogram")[0].InnerText;
                        ver = Convert.ToInt32(versn.Replace(".", ""));
                        File.Delete("version.xml");
                    }
                    else
                    {
                        ver = intVersion;
                        versn =  version ;
                    }



                if (intVersion < ver)
                {
                    DownloadFile(new Uri(url2), pCUpdate);
                    DownloadFile(new Uri(url3), updater);
                   
                    Thread.Sleep(300);

                    if (File.Exists(pCUpdate) && File.Exists(updater))
                    {
                        MessageBox.Show("Виявлено нову версію (" +
                                              doc.GetElementsByTagName("myprogram")[0].InnerText + ")" +
                                              Environment.NewLine +
                                              "Додаток буде автоматично оновлено і перезапуститься.",
                            Application.ProductName + " v" + Application.ProductVersion, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        checkUpdates();

                    }


                }
            }
            catch (System.Net.WebException) { }
        }
    
        public void DownloadFile(Uri adress, string fileName)
        {
            using (WebClient wc = new WebClient())
            {

                //wc.DownloadProgressChanged += (s, te) => { progressBar1.Value = te.ProgressPercentage; };

                wc.DownloadFile(adress, fileName);
            }
        }
    
    public void checkUpdates()
    {
        try
        {
            int newVersion = Convert.ToInt32(new Version(FileVersionInfo.GetVersionInfo("launcher.update").FileVersion)
                .ToString().Replace(".", ""));
            int oldVersion =
                Convert.ToInt32(new Version(Application.ProductVersion)
                    .ToString().Replace(".", ""));

            MessageBox.Show("" + newVersion + " " + oldVersion);

            //if (File.Exists("launcher.update"))
            if (File.Exists("launcher.update") && newVersion > oldVersion)
            {
                Process.Start("updater.exe", "progressbar.exe  launcher.update");
                Process.GetCurrentProcess().CloseMainWindow();
               
            }
            // else
            {
                //if (File.Exists("launcher.update")) { File.Delete("launcher.update"); }
                //Download();
            }

        }
        catch (Exception)
        {
            //if (File.Exists("launcher.update")) { File.Delete("launcher.update"); }
            //Download();
        }
    }
    }
}
