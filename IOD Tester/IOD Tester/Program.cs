using Morphic.IoD;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Msix;

namespace IOD_Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var msipath = Path.Combine(basepath, "exe contents", "setup.msi");
            var exepath = Path.Combine(basepath, "J2021.2011.16.400-any.exe");

            IoDStatus status;

            Console.WriteLine("Installing Programs");

            //IoDSystemRestorePoint.SetStartPoint("ayy");

            Console.WriteLine("MSI Install:");

            //var msipath = Path.Combine(basepath, "Installers", "MSI", "CertDump.msi");
            var msiInstall = new IoDMsiInstaller(msipath);
            msiInstall.verbose = true;  //comment out to get the progress monitor to shut up

            status = await msiInstall.Run();

            if (status == IoDStatus.OK)
            {
                Console.WriteLine("Install Successful");
            }
            else
            {
                Console.WriteLine("Installer Failed, Error " + status.inEnglish());
            }

            Console.WriteLine("EXE Install:");

            //var exepath = Path.Combine(basepath, "J2021.2011.16.400-any.exe");
            var exeInstall = new IoDExeLauncher(exepath, "/Silent");

            status = await exeInstall.Run();

            if (status == IoDStatus.OK)
            {
                Console.WriteLine("Install Successful");
            }
            else
            {
                Console.WriteLine("Installer Failed, Error: " + status.inEnglish());
            }

            //IoDSystemRestorePoint.SetEndPoint("lmao");

            //Console.WriteLine("Uninstalling");

            //msiInstall.Uninstall();

            Console.WriteLine("Complete!");

            //IoDSystemRestorePoint.LoadPoint("ayy");

            return;
        }
    }
}
