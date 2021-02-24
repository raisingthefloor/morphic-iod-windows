using Morphic.IoD;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Management;

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

            Console.WriteLine("Setting Restore Point:");

            IoDSystemRestorePoint.EnableMultiPointRestore();

            if (IoDSystemRestorePoint.SetStartPoint("Morphic Start Point"))
            {
                Console.WriteLine("Restore Point Set Successfully.");
            }
            else
            {
                Console.WriteLine("Restore Point Failed. Please Try Again.");

                Console.WriteLine("Press Enter to Exit:");

                Console.ReadKey();

                return;
            }

            Console.WriteLine("Installing Programs:");

            /*

            Console.WriteLine("MSI Install:");

            //var msipath = Path.Combine(basepath, "Installers", "MSI", "CertDump.msi");
            var msiInstall = new IoDMsiInstaller(msipath);
            msiInstall.verbose = true;  //comment out to get the progress monitor to shut up

            status = await msiInstall.RunAsync();

            if (status == IoDStatus.OK)
            {
                Console.WriteLine("Install Successful");
            }
            else
            {
                Console.WriteLine("Installer Failed, Error " + status.inEnglish());
            }*/

            string msixpath;
            IoDMsiXInstaller msixInstall;

            Console.WriteLine("MSIX Tests:");

            Console.WriteLine("ModernFlyouts:");

            msixpath = Path.Combine(basepath, "Installers", "MSIX", "ModernFlyouts.msixbundle");
            msixInstall = new IoDMsiXInstaller(msixpath);
            msixInstall.verbose = true;

            status = await msixInstall.RunAsync();

            if (status == IoDStatus.OK)
            {
                Console.WriteLine("Install Successful");
            }
            else
            {
                Console.WriteLine("Installer Failed, Error " + status.inEnglish());
            }

            IoDSystemRestorePoint.SetStartPoint("Morphic Pre-Notepads");

            Console.WriteLine("Notepads:");

            msixpath = Path.Combine(basepath, "Installers", "MSIX", "Notepads.msixbundle");
            msixInstall = new IoDMsiXInstaller(msixpath);
            msixInstall.verbose = true;

            status = await msixInstall.RunAsync();

            if (status == IoDStatus.OK)
            {
                Console.WriteLine("Install Successful");
            }
            else
            {
                Console.WriteLine("Installer Failed, Error " + status.inEnglish());
            }

            IoDSystemRestorePoint.SetStartPoint("Morphic Pre-Torrex");

            Console.WriteLine("Torrex:");

            msixpath = Path.Combine(basepath, "Installers", "MSIX", "Torrex.msix");
            msixInstall = new IoDMsiXInstaller(msixpath);
            msixInstall.verbose = true;

            status = await msixInstall.RunAsync();

            if (status == IoDStatus.OK)
            {
                Console.WriteLine("Install Successful");
            }
            else
            {
                Console.WriteLine("Installer Failed, Error " + status.inEnglish());
            }

            IoDSystemRestorePoint.SetStartPoint("Morphic After MSIXs");

            /*

            Console.WriteLine("EXE Install:");

            //var exepath = Path.Combine(basepath, "J2021.2011.16.400-any.exe");
            var exeInstall = new IoDExeLauncher(exepath, "/Silent");

            status = await exeInstall.RunAsync();

            if (status == IoDStatus.OK)
            {
                Console.WriteLine("Install Successful");
            }
            else
            {
                Console.WriteLine("Installer Failed, Error: " + status.inEnglish());
            }*/

            //IoDSystemRestorePoint.SetEndPoint("lmao");

            //Console.WriteLine("Uninstalling");

            //msiInstall.Uninstall();

            Console.WriteLine("Activating System Restore");

            if (IoDSystemRestorePoint.Restore("Morphic Start Point"))
            {
                Console.WriteLine("Restore Activation Successful, Restart Computer to Activate System Restore");
            }
            else
            {
                Console.WriteLine("System Restore Activation Failed.");
            }

            Console.WriteLine("Complete!");

            Console.WriteLine("Press Enter to Exit:");

            Console.ReadKey();

            return;
        }
    }
}
