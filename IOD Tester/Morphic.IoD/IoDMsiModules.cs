﻿using Microsoft.Deployment.WindowsInstaller;
using Morphic.Core;
using System;

namespace Morphic.IoD
{
    public class IoDMsiInstaller
    {
        public IoDMsiInstaller(string msiFile)
        {
            active = false;
            msiFilepath = msiFile;
            handler = UIMonitor;
        }

        public bool verbose = false;

        public enum InstallError
        {
            ProgramInUse,
            BadParams,
            ManualHalt,
            MiscFailure,
            OutOfSpace
        }
        public IMorphicResult<bool, InstallError> Install()
        {
            if (active) return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.ProgramInUse);
            active = true;
            fail = false;
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
                active = false;
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.BadParams);
            }
            catch (InstallCanceledException)
            {
                active = false;
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.ManualHalt);
            }
            catch (InvalidHandleException)
            {
                active = false;
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.BadParams);
            }
            catch (InstallerException)
            {
                active = false;
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.MiscFailure);
            }
            active = false;
            if(fail)
            {
                return IMorphicResult<bool, InstallError>.ErrorResult(failstate);
            }
            else
            {
                return IMorphicResult<bool, InstallError>.SuccessResult(true);
            }
        }

        public double getProgress()
        {
            return 100.0 * progressValue;
        }

        public IMorphicResult<bool, InstallError> Uninstall()
        {
            if (active) return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.ProgramInUse);
            active = true;
            fail = false;
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
                active = false;
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.BadParams);
            }
            catch (InstallCanceledException)
            {
                active = false;
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.ManualHalt);
            }
            catch (InvalidHandleException)
            {
                active = false;
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.BadParams);
            }
            catch (InstallerException)
            {
                active = false;
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.MiscFailure);
            }
            active = false;
            if (fail)
            {
                return IMorphicResult<bool, InstallError>.ErrorResult(failstate);
            }
            else
            {
                return IMorphicResult<bool, InstallError>.SuccessResult(true);
            }
        }

        private bool active;
        private string msiFilepath;
        private ExternalUIRecordHandler handler;
        private bool fail;
        private InstallError failstate;

        private double progressValue;
        private int progressTotal;
        private int progressCompleted;
        private int progressStep;
        private bool progressMove;
        private bool enableActionData;
        private int progressPhase;
        private double progressWeight = 0.5;

        public class ProgressEventArgs : EventArgs
        {
            public double progressVal;  //the % value of progress for the installation
            public ProgressEventArgs(double progress) { this.progressVal = progress; }
        }

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
                    if (!fail)
                    {
                        fail = true;
                        failstate = InstallError.MiscFailure;
                    }
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
                    fail = true;
                    failstate = InstallError.OutOfSpace;
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

        private void TrackProgress(Record message)
        {
            if (message == null || message.FieldCount == 0)
            {
                return;
            }
            switch (message.GetInteger(1))
            {
                case 0: //progress reset
                    if (message.FieldCount < 4)
                    {
                        return;
                    }
                    this.progressPhase++;
                    this.progressTotal = message.GetInteger(2);
                    if (this.progressPhase == 1)
                    {
                        //SHENANIGANS by Installer team
                        this.progressTotal += 50;
                    }
                    this.progressMove = (message.GetInteger(3) == 0);
                    this.progressCompleted = (this.progressMove ? 0 : this.progressTotal);
                    this.enableActionData = false;
                    this.updateProgress();
                    break;
                case 1:
                    if (message.FieldCount < 3)
                    {
                        return;
                    }
                    if (message.GetInteger(3) == 0)
                    {
                        this.enableActionData = false;
                    }
                    else
                    {
                        this.enableActionData = true;
                        this.progressStep = message.GetInteger(2);
                    }
                    break;
                case 2:
                    if (message.FieldCount < 2 || this.progressTotal == 0 || this.progressPhase == 0)
                    {
                        return;
                    }
                    if (this.progressMove)
                    {
                        this.progressCompleted += message.GetInteger(2);
                    }
                    else
                    {
                        this.progressTotal -= message.GetInteger(2);
                    }
                    this.updateProgress();
                    break;
                case 3:
                    this.progressTotal += message.GetInteger(2);
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
