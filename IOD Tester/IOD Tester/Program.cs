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
        static async void Main(string[] args)
        {
            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var msipath = Path.Combine(basepath, "exe contents", "setup.msi");
            var exepath = Path.Combine(basepath, "J2021.2011.16.400-any.exe");

            var msiInstall = new IoDMsiInstaller(msipath);
            var exeInstall = new IoDExeLauncher(exepath, "\\Silent");

            Console.WriteLine("Installing Programs");

            await msiInstall.Run();

            await exeInstall.Run();

            Console.WriteLine("Install Complete!");

            //Console.WriteLine("Uninstalling");

            //msiInstall.Uninstall();

            Console.WriteLine("Complete!");

            return;
        }
    }
}
