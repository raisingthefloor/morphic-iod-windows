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

            /*
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
            */

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
                {"Torrex", "Torrex.msix" }
            };

            msixInstall = new IoDMsiXInstaller();
            ProgressBars bars = new ProgressBars();
            int currBar = 0;

            for (int i = 0; i < names.GetLength(0); ++i)
            {
                bars.Add(names[i, 0]);
            }
            bars.Write();

            for (int i = 0; i < names.GetLength(0); ++i)
            {
                msixpath = Path.Combine(basepath, "Installers", "MSIX", names[i, 1]);
                msixInstall.verbose = false;

                EventHandler<IoDMsiXInstaller.ProgressEventArgs> handler = new EventHandler<IoDMsiXInstaller.ProgressEventArgs>((object? caller, IoDMsiXInstaller.ProgressEventArgs args) =>
                {
                    bars.Update(i, args.progressVal, args.progressState == Windows.Management.Deployment.DeploymentProgressState.Processing);
                });

                var status = await msixInstall.InstallAsync(msixpath, handler);

                if (status.IsSuccess)
                {
                    Console.WriteLine("Install Successful");
                }
                else
                {
                    Console.WriteLine("Installer Failed, Error " + status.Error.type.ToString() + " (windows code " + status.Error.errorCode.ToString() + ")");
                    Console.WriteLine(status.Error.verboseLog);
                }

                currBar++;
            }

            string[,] unames = new string[,]
            {
                {"AppInstallerFileBuilder", "AppInstallerFileBuilder_1.2020.221.0_x86__8wekyb3d8bbwe" },
                {"EarTrumpet", "40459File-New-Project.EarTrumpet_2.1.8.0_x86__725pr5jq8wr8a" },
                {"Files", "49306atecsolution.FilesUWP_0.21.1.0_x64__et10x9a9vyk8t" },
                {"ModernFlyouts", "32669SamG.ModernFlyouts_0.6.0.0_neutral__pcy8vm99wrpcg" },
                {"MSIX Commander", "PascalBerger.MSIXCommander_1.0.7.5_x64__ajjbhed1xnq88" },
                {"MSIX Hero", "MSIXHero_0.7.1.0_neutral__zxq1da1qqbeze" },
                {"Notepads", "19282JackieLiu.Notepads-Beta_1.3.8.0_x64__echhpq9pdbte8" },
                {"Picard", "MetaBrainzFoundationInc.org.musicbrainz.Picard_2.4.40000.0_x64__6cfbg5p5jt8h8" },
                {"PowerShell", "Microsoft.PowerShellPreview_7.0.2.0_x64__8wekyb3d8bbwe" },
                {"PowerToys", "Microsoft.PowerToys_0.15.2.0_x64__8wekyb3d8bbwe" },
                {"Presence Light", "37828IsaacLevin.197278F15330A_3.5.15.0_x64__jvewcxq8vj8qt" },
                {"Strix Music", "59553ArloG.StrixMusicBeta_1.4.7.0_x64__gzh7hvbrgycb4" },
                {"Windows Terminal", "Microsoft.WindowsTerminal_0.11.1333.0_x64__8wekyb3d8bbwe" },
                {"XML Notepad", "5632ff08-aa93-439a-b09f-677eb3664250_2.8.0.25_neutral__b2j8apzf1bbh6" },
                {"Torrex", "womp womp" }
            };

            msixInstall = new IoDMsiXInstaller();
            ProgressBars ubars = new ProgressBars();
            currBar = 0;

            for (int i = 0; i < unames.GetLength(0); ++i)
            {
                ubars.Add(unames[i, 0]);
            }
            ubars.Write();

            for (int i = 0; i < unames.GetLength(0); ++i)
            {
                msixInstall.verbose = false;

                EventHandler<IoDMsiXInstaller.ProgressEventArgs> uhandler = new EventHandler<IoDMsiXInstaller.ProgressEventArgs>((object? caller, IoDMsiXInstaller.ProgressEventArgs args) =>
                {
                    ubars.Update(i, args.progressVal, args.progressState == Windows.Management.Deployment.DeploymentProgressState.Processing);
                });

                var status = await msixInstall.UninstallAsync(unames[i, 1], uhandler);

                if (status.IsSuccess)
                {
                    Console.WriteLine("Uninstall Successful");
                }
                else
                {
                    Console.WriteLine("Uninstaller Failed, Error " + status.Error.type.ToString() + " (windows code " + status.Error.errorCode.ToString() + ")");
                    Console.WriteLine(status.Error.verboseLog);
                }

                currBar++;
            }

            //IoDSystemRestorePoint.SetStartPoint("Morphic After MSIXs");

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

            /*Console.WriteLine("Activating System Restore");

            if (IoDSystemRestorePoint.Restore("Morphic Start Point"))
            {
                Console.WriteLine("Restore Activation Successful, Restart Computer to Activate System Restore");
            }
            else
            {
                Console.WriteLine("System Restore Activation Failed.");
            }*/

            Console.WriteLine("Complete!");

            Console.WriteLine("Press Enter to Exit:");

            Console.ReadKey();

            return;
        }
    }
}
