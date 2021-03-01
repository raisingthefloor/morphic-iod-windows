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

            string[,] names = new string[,]
            {
                {"AppInstallerFileBuilder", "AppInstallerFileBuilder.msix" },
                {"EarTrumpet", "EarTrumpet.appxbundle" },
                {"Files", "Files.msixbundle" },
                {"ModernFlyouts", "ModernFlyouts.msixbundle" },
                {"MSIX Commander", "MSIX Commander.msix" },
                {"MSIX Hero", "MSIX Hero.msix" },
                {"Notepads", "Notepads.msixbundle" },
                {"Picard", "Picard.msix" },
                {"PowerShell", "PowerShell.msix" },
                {"PowerToys", "PowerToys.msix" },
                {"Presence Light", "PresenceLight.appxbundle" },
                {"Strix Music", "StrixMusic.appxbundle" },
                {"Windows Terminal", "WindowsTerminal.msixbundle" },
                {"XML Notepad", "XMLNotepad.msixbundle" },
            };

            for(int i = 0; i < names.GetLength(0); ++i)
            {
                Console.WriteLine(names[i, 0] + ":");

                msixpath = Path.Combine(basepath, "Installers", "MSIX", names[i, 1]);
                msixInstall = new IoDMsiXInstaller(msixpath);
                msixInstall.verbose = true;

                var status = await msixInstall.InstallAsync();

                if (status.IsSuccess)
                {
                    Console.WriteLine("Install Successful");
                }
                else
                {
                    Console.WriteLine("Installer Failed, Error");
                }
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
