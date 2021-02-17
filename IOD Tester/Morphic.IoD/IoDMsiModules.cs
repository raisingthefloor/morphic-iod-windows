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

        public bool verbose = false;

        public async Task<IoDStatus> RunAsync()
        {
            if (active) return IoDStatus.ProgramInUse;
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
                    InstallLogModes.PropertyDump |
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

        public double getProgress()
        {
            return 100.0 * progressValue;
        }

        public IoDStatus Uninstall()
        {
            if (active) return IoDStatus.ProgramInUse;
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
                    InstallLogModes.PropertyDump |
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
        private ExternalUIRecordHandler handler;
        private IoDStatus status;

        private double progressValue;
        private int progressTotal;
        private int progressCompleted;
        private int progressStep;
        private bool progressMove;
        private bool enableActionData;
        private int progressPhase;
        private double progressWeight = 0.5;

        MessageResult UIMonitor(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            MessageResult result = new MessageResult();
            switch (messageType)
            {
                case InstallMessage.ActionData:
                    if(this.enableActionData)
                    {
                        if(this.progressMove)
                        {
                            this.progressCompleted += this.progressStep;
                        }
                        else
                        {
                            this.progressCompleted -= this.progressStep;
                        }
                        this.updateProgress();
                    }
                    break;
                case InstallMessage.ActionStart:
                    this.enableActionData = false;
                    break;
                case InstallMessage.CommonData:

                    break;
                case InstallMessage.Error:
                    Console.WriteLine(messageRecord.ToString());
                    break;
                case InstallMessage.FatalExit:
                    Console.WriteLine(messageRecord.ToString());
                    if (status <= IoDStatus.MiscFailure) status = IoDStatus.MiscFailure;
                    break;
                case InstallMessage.FilesInUse:
                    Console.WriteLine("Processes are open that need to be shut down");
                    result = MessageResult.Yes; //not sure about this but it shouldn't come up
                    break;
                case InstallMessage.Info:

                    break;
                case InstallMessage.Initialize:

                    break;
                case InstallMessage.InstallEnd:

                    break;
                case InstallMessage.InstallStart:

                    break;
                case InstallMessage.OutOfDiskSpace:
                    Console.WriteLine("Insufficient disk space");
                    if (status <= IoDStatus.MiscFailure) status = IoDStatus.NoSpace;
                    break;
                case InstallMessage.Progress:
                    TrackProgress(messageRecord);
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
                    Console.WriteLine(messageRecord.ToString());
                    break;
            }
            return result;
        }

        private void TrackProgress(Record messageRecord)
        {
            if (messageRecord == null || messageRecord.FieldCount == 0)
            {
                return;
            }

            int fieldCount = messageRecord.FieldCount;
            int progressType = messageRecord.GetInteger(1);
            string progressTypeString = String.Empty;
            switch (progressType)
            {
                case 0: //progress reset
                    if (fieldCount < 4)
                    {
                        return;
                    }
                    this.progressPhase++;
                    this.progressTotal = messageRecord.GetInteger(2);
                    if (this.progressPhase == 1)
                    {
                        //SHENANIGANS by Installer team
                        this.progressTotal += 50;
                    }
                    this.progressMove = (messageRecord.GetInteger(3) == 0);
                    this.progressCompleted = (this.progressMove ? 0 : this.progressTotal);
                    this.enableActionData = false;
                    this.updateProgress();
                    break;
                case 1:
                    if (fieldCount < 3)
                    {
                        return;
                    }
                    if (messageRecord.GetInteger(3) == 0)
                    {
                        this.enableActionData = false;
                    }
                    else
                    {
                        this.enableActionData = true;
                        this.progressStep = messageRecord.GetInteger(2);
                    }
                    break;
                case 2:
                    if (fieldCount < 2 || this.progressTotal == 0 || this.progressPhase == 0)
                    {
                        return;
                    }
                    if (this.progressMove)
                    {
                        this.progressCompleted += messageRecord.GetInteger(2);
                    }
                    else
                    {
                        this.progressTotal -= messageRecord.GetInteger(2);
                    }
                    this.updateProgress();
                    break;
                case 3:
                    this.progressTotal += messageRecord.GetInteger(2);
                    break;
            }
        }

        private void updateProgress()
        {
            if (this.progressPhase < 1 || this.progressTotal == 0)
            {
                this.progressValue = 0;
            }
            else if (this.progressPhase == 1)
            {
                this.progressValue = this.progressWeight * Math.Min(this.progressCompleted, this.progressTotal) / this.progressTotal;
            }
            else if (this.progressPhase == 2)
            {
                this.progressValue = this.progressWeight + (1 - this.progressWeight) * Math.Min(this.progressCompleted, this.progressTotal) / this.progressTotal;
            }
            else
            {
                this.progressValue = 1;
            }

            if (this.verbose)
            {
                Console.WriteLine((100 * this.progressValue).ToString() + "%");
            }
        }
    }
}
