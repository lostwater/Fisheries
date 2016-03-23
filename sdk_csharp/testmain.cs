using csharp_upload_sdk;
using uts_sdk;
using Gandalf;
using System.IO;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace csharp
{
    class testmain
    {
        /// <summary>
        /// 测试程序主函数
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //save uploading task
            Hashtable htUploads = new Hashtable();
            string filePath = "Y:\\Downloads\\sophie\\6年级.wmv";

            SDSUpload uts = uploadFile(htUploads, filePath);

            System.Console.WriteLine("start uploading...");

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                while (!uts.isFinish())
                {
                    if (cts.IsCancellationRequested)
                        break;

                    string output = String.Format("{0}, {1}", uts.getProgress().ToString("N2"), uts.getRate().ToString("N2"));
                    System.Console.WriteLine(output);
                    System.Threading.Thread.Sleep(500);
                }
            }, cts.Token);

            //just test resume
            while (System.Console.ReadKey().Key == ConsoleKey.Escape)
            {
                cts.Cancel();
                uts.stop();
                uts = null;

                if (System.Console.ReadKey().Key == ConsoleKey.Escape)
                    break;

                uts = uploadFile(htUploads, filePath);
                cts = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    while (!uts.isFinish())
                    {
                        if (cts.IsCancellationRequested)
                            break;

                        string output = String.Format("{0}, {1}", uts.getProgress().ToString("N2"), uts.getRate().ToString("N2"));
                        System.Console.WriteLine(output);
                        System.Threading.Thread.Sleep(500);
                    }
                }, cts.Token);
            }

        }

        public static SDSUpload uploadFile(Hashtable ht, string filePath)
        {
            SDSUpload uts = new SDSUpload("iupo1c7d32", "0921ffc5f89efdade5873bdc9bd99403");
            string fileKey = getFileKey(filePath);

            Gandalf.SDSUpload.delegateCallback cb = new Gandalf.SDSUpload.delegateCallback(callbackUts);

            if (ht.ContainsKey(fileKey))
            {
                string token = uts.tryUpload(filePath, "192.168.253.2", cb, ht[fileKey].ToString());
                if (!String.IsNullOrEmpty(token))
                    ht[fileKey] = token;
            }
            else
            {
                string token = uts.tryUpload(filePath, "192.168.253.2", cb);
                if (!String.IsNullOrEmpty(token))
                    ht[fileKey] = token;
            }

            return uts;
        }

        public static string getFileKey(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return string.Empty;

            DateTime dt = File.GetLastWriteTime(filePath);
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            int ts = (int)(dt - startTime).TotalSeconds;

            FileStream fsFile = new FileStream(filePath, FileMode.Open);
            string fileKey = String.Format("{0}_{1}_{2}", Path.GetExtension(fsFile.Name), fsFile.Length, ts);

            fsFile.Close();

            return fileKey;
        }

        public static void callbackUts(int code, string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
