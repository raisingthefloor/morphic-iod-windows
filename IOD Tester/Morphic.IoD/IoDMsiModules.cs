using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Threading.Tasks;

namespace Morphic.IoD
{
    public class IoDMsiInstaller : IoDModule
    {
        public IoDMsiInstaller(string msiFile)
        {
            active = false;
            msiFilepath = msiFile;
            handler = UIMonitor;
            status = IoDStatus.OK;
        }

        public async Task<IoDStatus> RunAsync()
        {
            if (active) return IoDStatus.InUse;
            active = true;
            status = IoDStatus.OK;
            try
            {
                Installer.SetInternalUI(InstallUIOptions.Silent);
                Installer.SetExternalUI(handler,
                    InstallLogModes.ActionData |
                    InstallLogModes.ActionStart |
                    InstallLogModes.CommonData |
                    InstallLogModes.Error |
                    InstallLogModes.ExtraDebug |
                    InstallLogModes.FatalExit |
                    InstallLogModes.FilesInUse |
                    InstallLogModes.Info |
                    InstallLogModes.Initialize |
                    InstallLogModes.OutOfDiskSpace |
                    InstallLogModes.Progress |
                    //InstallLogModes.PropertyDump |
                    InstallLogModes.ResolveSource |
                    InstallLogModes.RMFilesInUse |
                    InstallLogModes.ShowDialog |
                    InstallLogModes.Terminate |
                    InstallLogModes.User |
                    InstallLogModes.Verbose |
                    InstallLogModes.Warning
                    );
                Installer.InstallProduct(msiFilepath, "");
            }
            catch (BadQuerySyntaxException)
            {
                if (status <= IoDStatus.MiscFailure)
                {
                    status = IoDStatus.BadParams;
                }
            }
            catch (InstallCanceledException)
            {
                if (status <= IoDStatus.MiscFailure)
                {
                    status = IoDStatus.ManualHalt;
                }
            }
            catch (InvalidHandleException)
            {
                if (status <= IoDStatus.MiscFailure)
                {
                    status = IoDStatus.BadParams;
                }
            }
            catch (InstallerException)
            {
                if (status <= IoDStatus.MiscFailure)
                {
                    status = IoDStatus.MiscFailure;
                }
            }
            active = false;
            return status;
        }

        public IoDStatus Uninstall()
        {
            if (active) return IoDStatus.InUse;
            active = true;
            status = IoDStatus.OK;
            try
            {
                Installer.SetInternalUI(InstallUIOptions.Silent);
                Installer.SetExternalUI(handler,
                    InstallLogModes.ActionData |
                    InstallLogModes.ActionStart |
                    InstallLogModes.CommonData |
                    InstallLogModes.Error |
                    InstallLogModes.ExtraDebug |
                    InstallLogModes.FatalExit |
                    InstallLogModes.FilesInUse |
                    InstallLogModes.Info |
                    InstallLogModes.Initialize |
                    InstallLogModes.OutOfDiskSpace |
                    InstallLogModes.Progress |
                    //InstallLogModes.PropertyDump |
                    InstallLogModes.ResolveSource |
                    InstallLogModes.RMFilesInUse |
                    InstallLogModes.ShowDialog |
                    InstallLogModes.Terminate |
                    InstallLogModes.User |
                    InstallLogModes.Verbose |
                    InstallLogModes.Warning
                    );
                Installer.InstallProduct(msiFilepath, "REMOVE=ALL");
            }
            catch (BadQuerySyntaxException)
            {
                if (status <= IoDStatus.MiscFailure)
                {
                    status = IoDStatus.BadParams;
                }
            }
            catch (InstallCanceledException)
            {
                if (status <= IoDStatus.MiscFailure)
                {
                    status = IoDStatus.ManualHalt;
                }
            }
            catch (InvalidHandleException)
            {
                if (status <= IoDStatus.MiscFailure)
                {
                    status = IoDStatus.BadParams;
                }
            }
            catch (InstallerException)
            {
                if (status <= IoDStatus.MiscFailure)
                {
                    status = IoDStatus.MiscFailure;
                }
            }
            active = false;
            return status;
        }

        private bool active;
        private string msiFilepath;
        private ExternalUIHandler handler;
        private IoDStatus status;

        MessageResult UIMonitor(InstallMessage messageType, string message, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            MessageResult result = new MessageResult();
            switch (messageType)
            {
                case InstallMessage.ActionData:

                    break;
                case InstallMessage.ActionStart:

                    break;
                case InstallMessage.CommonData:

                    break;
                case InstallMessage.Error:
                    Console.WriteLine(message);
                    break;
                case InstallMessage.FatalExit:
                    Console.WriteLine(message);
                    if (status <= IoDStatus.MiscFailure) status = IoDStatus.MiscFailure;
                    break;
                case InstallMessage.FilesInUse:
                    Console.WriteLine("Processes are open that need to be shut down");
                    result = MessageResult.Yes; //not sure about this but it shouldn't come up
                    break;
                case InstallMessage.Info:

                    break;
                case InstallMessage.Initialize:
                    Console.WriteLine("Initializing");
                    break;
                case InstallMessage.InstallEnd:
                    Console.WriteLine("Ending installation");
                    break;
                case InstallMessage.InstallStart:
                    Console.WriteLine("Beginning installation");
                    break;
                case InstallMessage.OutOfDiskSpace:
                    Console.WriteLine("Insufficient disk space");
                    if (status <= IoDStatus.MiscFailure) status = IoDStatus.NoSpace;
                    break;
                case InstallMessage.Progress:

                    break;
                case InstallMessage.ResolveSource:

                    break;
                case InstallMessage.RMFilesInUse:

                    break;
                case InstallMessage.ShowDialog:

                    break;
                case InstallMessage.Terminate:

                    break;
                case InstallMessage.User:

                    break;
                case InstallMessage.Warning:
                    Console.WriteLine(message);
                    break;
            }
            return result;
        }
    }
}
