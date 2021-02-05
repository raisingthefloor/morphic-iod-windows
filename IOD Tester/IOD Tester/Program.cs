using Morphic.IoD;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IOD_Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var msipath = Path.Combine(basepath, "exe contents", "setup.msi");
            var exepath = Path.Combine(basepath, "J2021.2011.16.400-any.exe");

            var msiInstall = new IoDMsiInstaller(msipath);
            var exeInstall = new IoDExeLauncher(exepath, "/Silent");

            Console.WriteLine("Installing Programs");

            Console.WriteLine("Program 1:");

            var program1InstallStatus = await msiInstall.Run();
            Console.WriteLine("Program 1 install: " + program1InstallStatus.ToString());

            Console.WriteLine("Program 2:");

            var program2InstallStatus = await exeInstall.Run();
            Console.WriteLine("Program 2 install: " + program2InstallStatus.ToString());

            Console.WriteLine("Install Complete!");

            //Console.WriteLine("Uninstalling");

            //msiInstall.Uninstall();

            Console.WriteLine("Complete!");

            return;
        }
    }
}
