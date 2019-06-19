using System;
using System.IO;
using System.Text;

namespace SoftGenConverter
{
    class ReadIni
    {
        public static Datashit IniRead(Datashit recviz, IniFile myIni)
        {
            string[] recvizs = { };
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"config.ini");
            try
            {

                // config.Text = myIni.Read("bank Aval");
            recviz.name = myIni.Read("Bank");
            recviz.platNumber = Int64.Parse(myIni.Read("PlatNumber"));
            recviz.mfo = myIni.Read("Mfo");
            recviz.rahunok = myIni.Read("Rahunok");
            recviz.datePayment = Convert.ToInt32(myIni.Read("Paydate"));
            recviz.recivPayNum = myIni.Read("PlatReciver");
            recviz.cliBankCode = myIni.Read("bankclentnum");

            // config.Text = myIni.Read("bank2 UkrGaz");
            recviz.name2 = myIni.Read("Bank2");
            recviz.platNumber2 = Int64.Parse(myIni.Read("PlatNumber2"));
            recviz.edrpou = myIni.Read("Edrpou");
            recviz.rahunok2 = myIni.Read("Rahunok2");
            //recviz.state = Int32.Parse(myIni.Read("State"));

            
                recvizs = File.ReadAllLines(path, Encoding.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }


            //                try
            //                {
            //                    if (recvizs.Length >= 5)
            //                    {
            //                        recviz.name = string.IsNullOrEmpty(recvizs[0]) ? "" : recvizs[0];
            //                        recviz.platNumber = string.IsNullOrEmpty(recvizs[1]) ? 0 : Convert.ToInt32(recvizs[1]);
            //                        recviz.mfo = string.IsNullOrEmpty(recvizs[2]) ? 0 : Convert.ToInt32(recvizs[2]);
            //                        recviz.rahunok = string.IsNullOrEmpty(recvizs[3]) ? "0" : (recvizs[3]);
            //                        recviz.datePayment = string.IsNullOrEmpty(recvizs[4]) ? 0 : Convert.ToInt32(recvizs[4]);
            //                    }
            //                }
            //                catch (Exception e)
            //                {
            //
            //                }
//            try
//            {
//                if (recvizs.Length >= 9)
//                {
//                    recviz.name2 = myIni.Read("Bank2");
//                    recviz.platNumber2 = Int32.Parse(myIni.Read("PlatNumber2"));
//                    recviz.edrpou = myIni.Read("Edrpou");
//                    recviz.rahunok2 = recviz.rahunok = myIni.Read("Rahunok2");
//                }
//            }
//            catch (Exception e)
//            {
//
//            }

            return recviz;
        }

    }
}
