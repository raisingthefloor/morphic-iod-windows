using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Morphic.IoD;
using Microsoft.Deployment.WindowsInstaller;

namespace IOD_Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var msipath = Path.Combine(basepath, "exe contents", "setup.msi");

            var msiInstall = new IoDMSI(msipath);

            Console.WriteLine("Installing Program");

            msiInstall.Install();

            Console.WriteLine("Install Complete!");

            Console.WriteLine("Uninstalling");

            msiInstall.Uninstall();

            Console.WriteLine("Complete!");

            return;
        }
    }
}
