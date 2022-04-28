using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace SoftGenConverter
{
    internal class Update
    {
        private Version localVersion = new Version(Application.ProductVersion);
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"version.xml");
        private string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PayConverter.update");
        private string path3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"updater.exe");
        private string path4 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Template.xlsx");
        private string pCUpdate = "PayConverter.update";
        private Version remoteVersion;
        private string updater = "updater.exe";

        private string url = "https://raw.githubusercontent.com/KAleksandr/PayConverter2/master/version.xml";
        private string url2 = "https://github.com/KAleksandr/PayConverter2/blob/master/PayConverter.exe?raw=true";
        private string url3 = "https://github.com/KAleksandr/PayConverter2/blob/master/updater.exe?raw=true";
        private string url4 = "https://github.com/KAleksandr/PayConverter2/blob/master/Resources/Template.xlsx?raw=true";


        public void DownloadTemplate() {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2 в .net Framework 4.0 додати
            if (!File.Exists(path4))
            {
                DownloadFile(new Uri(url4), path4);
                Thread.Sleep(300);
            
                //MessageBox.Show("Шаблон завантажено!");
            }
        }
        public void Download()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072; //TLS 1.2 в .net Framework 4.0 додати
            if (File.Exists(updater)) File.Delete(updater);
            var doc = new XmlDocument();
            try
            {
                DownloadFile(new Uri(url), path);
                Thread.Sleep(300);

                var exists = File.Exists(path);

                if (exists)
                {
                    doc.Load(path);
                    remoteVersion = new Version(doc.GetElementsByTagName("version")[0].InnerText);

                    File.Delete("version.xml");
                }
                else
                {
                    remoteVersion = localVersion;
                }


                if (localVersion < remoteVersion)
                {
                    DownloadFile(new Uri(url2), pCUpdate);
                    DownloadFile(new Uri(url3), updater);

                    Thread.Sleep(300);


                    if (File.Exists(updater) && File.Exists(pCUpdate) &&
                        new Version(FileVersionInfo.GetVersionInfo(pCUpdate).FileVersion) >
                        new Version(Application.ProductVersion))
                    {
                        MessageBox.Show("Виявлено нову версію (" +
                                        doc.GetElementsByTagName("myprogram")[0].InnerText + ")" +
                                        Environment.NewLine +
                                        "Додаток буде автоматично оновлено.",
                            Application.ProductName + " v" + Application.ProductVersion, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        checkUpdates();
                    }
                    else
                    {
                        if (File.Exists(pCUpdate)) File.Delete(pCUpdate);
                        if (File.Exists(updater)) File.Delete(updater);
                    }
                }
            }
            catch (WebException)
            {
            }
        }

        public void DownloadFile(Uri adress, string fileName)
        {
            using (var wc = new WebClient())
            {
                //wc.DownloadProgressChanged += (s, te) => { progressBar1.Value = te.ProgressPercentage; };

                wc.DownloadFile(adress, fileName);
            }
        }

        public void checkUpdates()
        {
            try
            {
                if (File.Exists(pCUpdate) && remoteVersion > localVersion)
                {
                    Process.Start(updater, "PayConverter.exe  PayConverter.update");
                    Thread.Sleep(200);
                    Process.GetCurrentProcess().CloseMainWindow();
                    Thread.Sleep(600);
                    Application.Exit();
                }
                else
                {
                    if (File.Exists(pCUpdate)) File.Delete(pCUpdate);
                    //Download();
                }
            }
            catch (Exception)
            {
                if (File.Exists(pCUpdate)) File.Delete(pCUpdate);
                if (File.Exists(updater)) File.Delete(updater);
                //Download();
            }
        }
    }
}