using Morphic.Core;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Management.Deployment;

namespace Morphic.IoD
{
    public class IoDMsiXInstaller
    {
        public IoDMsiXInstaller(int waitTimer = 1000)
        {
            verbose = false;
            maxWait = waitTimer;
        }

        public class ProgressEventArgs : EventArgs
        {
            public double progressVal;  //the % value of progress for the installation
            public DeploymentProgressState progressState;   //whether the installation is queued or processing

            public ProgressEventArgs(double progress, DeploymentProgressState state) { this.progressVal = progress; this.progressState = state; }
        }

        public enum MSIXErrorType
        {
            BadParams,  //INTERNAL ERROR: the parameters given are faulty
            ManualHalt, //the user manually stopped the install
            PackageError,   //something is wrong with the package
            OSError,    //something is going on with the target system that prevents installation
            OSReset,    //something is going on, resetting the PC may fix it
            RetryPossible,  //an error was thrown that may resolve if we wait and retry
            OutOfSpace, //there's not enough space on the drive
            AlreadyExists, //there's a version of this program already installed (but not this version)
            MiscFailure
        }

        public enum MSIXErrorCode
        {
            PackageAlreadyExists,
            InstallOpenPackageFailed,
            InstallInvalidPackage,
            PackageUpdating,
            DeploymentBlockedByPolicy,
            InstallPackageNotFound,
            InstallOutOfDiskSpace,
            InstallNetworkFailure,
            InstallRegistrationFailure,
            BadFormat,
            InstallDeregistrationFailure,
            InstallCancel,
            InstallFailed,
            RemoveFailed,
            NeedsRemediation,
            InstallPrerequisiteFailed,
            PackageRepositoryCorrupted,
            InstallPolicyFailure,
            PackagesInUse,
            RecoveryFileCorrupt,
            InvalidStagedSignature,
            DeletingExistingApplicationdataStoreFailed,
            InstallPackageDowngrade,
            SystemNeedsRemediation,
            AppxIntegrityFailureExternal,
            ResiliencyFileCorrupt,
            InstallResolveDependencyFailed,
            InvalidArg,
            InstallFirewallServiceNotRunning,
            TrustNoSignature,
            CertUntrustedRoot,
            CertChaining,
            AppxInvalidSipClientData,
            AppxPackagingInternal,
            AppxInterleavingNotAllowed,
            AppxRelationshipsNotAllowed,
            AppxMissingRequiredFile,
            AppxInvalidManifest,
            AppxInvalidBlockmap,
            AppxCorruptContent,
            AppxBlockHashInvalid,
            AppxRequestedRangeTooLarge,
            Unknown
        }

        public struct MSIXErrorForm
        {
            public MSIXErrorType type;
            public MSIXErrorCode errorCode;
            public String verboseLog;
        }

        public async Task<IMorphicResult<bool, MSIXErrorForm>> InstallAsync(string filepath, EventHandler<ProgressEventArgs> progressHandler, System.Collections.Generic.IEnumerable<Uri>? dependencies = null)
        {
            var error = false;
            var done = false;
            var form = new MSIXErrorForm();
            Uri path;
            try
            {
                path = new Uri(filepath);
            }
            catch
            {
                form.type = MSIXErrorType.BadParams;
                return new MorphicError<bool, MSIXErrorForm>(form);
            }
            PackageManager pm = new PackageManager();
            var package = pm.AddPackageAsync(path, dependencies, DeploymentOptions.ForceApplicationShutdown);
            package.Progress = (message, progress) =>
            {
                progressHandler.Invoke(this, new ProgressEventArgs(progress.percentage, progress.state));
                if (verbose)
                {
                    Console.WriteLine(progress.percentage.ToString() + "%");
                }
            };
            package.Completed = (info, endStatus) =>
            {
                System.Runtime.InteropServices.COMException? err;
                switch (endStatus)
                {
                    case Windows.Foundation.AsyncStatus.Canceled:
                        error = true;
                        form.type = MSIXErrorType.ManualHalt;
                        err = info.ErrorCode as System.Runtime.InteropServices.COMException;
                        if (err != null)
                        {
                            form.errorCode = ParseCode((UInt32)err.ErrorCode);
                        }
                        form.verboseLog = info.GetResults().ErrorText;
                        if (verbose)
                        {
                            Console.WriteLine("MANUAL HALT, Error Code: " + info.ErrorCode + ": " + info.GetResults().ErrorText);
                        }
                        break;
                    case Windows.Foundation.AsyncStatus.Completed:
                        break;
                    case Windows.Foundation.AsyncStatus.Error:
                        error = true;
                        err = info.ErrorCode as System.Runtime.InteropServices.COMException;
                        if (err != null)
                        {
                            form.errorCode = ParseCode((UInt32)err.ErrorCode);
                        }
                        form.verboseLog = info.GetResults().ErrorText;
                        form.type = InterpretErrorCode(form.errorCode);
                        if (verbose)
                        {
                            Console.WriteLine("Error: " + info.GetResults().ExtendedErrorCode + ": " + info.GetResults().ErrorText);
                        }
                        break;
                    case Windows.Foundation.AsyncStatus.Started:
                        error = true;
                        form.type = MSIXErrorType.MiscFailure; //not sure how this would happen but hey
                        err = info.ErrorCode as System.Runtime.InteropServices.COMException;
                        if (err != null)
                        {
                            form.errorCode = ParseCode((UInt32)err.ErrorCode);
                        }
                        form.verboseLog = info.GetResults().ErrorText;
                        if (verbose)
                        {
                            Console.WriteLine("Strange Error: " + info.GetResults().ExtendedErrorCode + ": " + info.GetResults().ErrorText);
                        }
                        break;
                }
                done = true;
            };
            while (!done)
            {
                await Task.Delay(10);
            }
            if (error)
            {
                return new MorphicError<bool, MSIXErrorForm>(form);
            }
            else
            {
                return new MorphicSuccess<bool, MSIXErrorForm>(true);
            }
        }

        public async Task<IMorphicResult<bool, MSIXErrorForm>> UninstallAsync(string packageName, EventHandler<ProgressEventArgs> progressHandler)
        {
            var error = false;
            var done = false;
            var form = new MSIXErrorForm();
            PackageManager pm = new PackageManager();
            var package = pm.RemovePackageAsync(packageName);
            package.Progress = (message, progress) =>
            {
                progressHandler.Invoke(this, new ProgressEventArgs(progress.percentage, progress.state));
                if (verbose)
                {
                    Console.WriteLine(progress.percentage.ToString() + "%");
                }
            };
            package.Completed = (info, endStatus) =>
            {
                System.Runtime.InteropServices.COMException? err;
                switch (endStatus)
                {
                    case Windows.Foundation.AsyncStatus.Canceled:
                        error = true;
                        form.type = MSIXErrorType.ManualHalt;
                        err = info.ErrorCode as System.Runtime.InteropServices.COMException;
                        if (err != null)
                        {
                            form.errorCode = ParseCode((UInt32)err.ErrorCode);
                        }
                        form.verboseLog = info.GetResults().ErrorText;
                        if (verbose)
                        {
                            Console.WriteLine("MANUAL HALT, Error Code: " + info.ErrorCode + ": " + info.GetResults().ErrorText);
                        }
                        break;
                    case Windows.Foundation.AsyncStatus.Completed:
                        break;
                    case Windows.Foundation.AsyncStatus.Error:
                        error = true;
                        err = info.ErrorCode as System.Runtime.InteropServices.COMException;
                        if (err != null)
                        {
                            form.errorCode = ParseCode((UInt32)err.ErrorCode);
                        }
                        form.verboseLog = info.GetResults().ErrorText;
                        form.type = InterpretErrorCode(form.errorCode);
                        if (verbose)
                        {
                            Console.WriteLine("Error: " + info.GetResults().ExtendedErrorCode + ": " + info.GetResults().ErrorText);
                        }
                        break;
                    case Windows.Foundation.AsyncStatus.Started:
                        error = true;
                        form.type = MSIXErrorType.MiscFailure; //not sure how this would happen but hey
                        err = info.ErrorCode as System.Runtime.InteropServices.COMException;
                        if (err != null)
                        {
                            form.errorCode = ParseCode((UInt32)err.ErrorCode);
                        }
                        form.verboseLog = info.GetResults().ErrorText;
                        if (verbose)
                        {
                            Console.WriteLine("Strange Error: " + info.GetResults().ExtendedErrorCode + ": " + info.GetResults().ErrorText);
                        }
                        break;
                }
                done = true;
            };
            while (!done)
            {
                await Task.Delay(10);
            }
            if (error)
            {
                return new MorphicError<bool, MSIXErrorForm>(form);
            }
            else
            {
                return new MorphicSuccess<bool, MSIXErrorForm>(true);
            }
        }

        public bool verbose;
        private int maxWait;
        private int currWait;

        private MSIXErrorCode ParseCode(UInt32 code)
        {
            //if error codes fire but aren't here, look here https://docs.microsoft.com/en-us/windows/win32/com/com-error-codes
            switch(code)
            {
                case 0x80070057:
                    return MSIXErrorCode.InvalidArg;
                case 0x80073CFB:
                    return MSIXErrorCode.PackageAlreadyExists;
                case 0x80073CF0:
                    return MSIXErrorCode.InstallOpenPackageFailed;
                case 0x80073CF2:
                    return MSIXErrorCode.InstallInvalidPackage;
                case 0x80073D00:
                    return MSIXErrorCode.PackageUpdating;
                case 0x80073D01:
                    return MSIXErrorCode.DeploymentBlockedByPolicy;
                case 0x80073CF1:
                    return MSIXErrorCode.InstallPackageNotFound;
                case 0x80073CF4:
                    return MSIXErrorCode.InstallOutOfDiskSpace;
                case 0x80073CF5:
                    return MSIXErrorCode.InstallNetworkFailure;
                case 0x80073CF6:
                    return MSIXErrorCode.InstallRegistrationFailure;
                case 0x800700B:
                    return MSIXErrorCode.BadFormat;
                case 0x80073CF7:
                    return MSIXErrorCode.InstallDeregistrationFailure;
                case 0x80073CF8:
                    return MSIXErrorCode.InstallCancel;
                case 0x80073CF9:
                    return MSIXErrorCode.InstallFailed;
                case 0x80073CFA:
                    return MSIXErrorCode.RemoveFailed;
                case 0x80073CFC:
                    return MSIXErrorCode.NeedsRemediation;
                case 0x80073CFD:
                    return MSIXErrorCode.InstallPrerequisiteFailed;
                case 0x80073CFE:
                    return MSIXErrorCode.PackageRepositoryCorrupted;
                case 0x80073CFF:
                    return MSIXErrorCode.InstallPolicyFailure;
                case 0x80073D02:
                    return MSIXErrorCode.PackagesInUse;
                case 0x80073D03:
                    return MSIXErrorCode.RecoveryFileCorrupt;
                case 0x80073D04:
                    return MSIXErrorCode.InvalidStagedSignature;
                case 0x80073D05:
                    return MSIXErrorCode.DeletingExistingApplicationdataStoreFailed;
                case 0x80073D06:
                    return MSIXErrorCode.InstallPackageDowngrade;
                case 0x80073D07:
                    return MSIXErrorCode.SystemNeedsRemediation;
                case 0x80073D08:
                    return MSIXErrorCode.AppxIntegrityFailureExternal;
                case 0x80073D09:
                    return MSIXErrorCode.ResiliencyFileCorrupt;
                case 0x80073CF3:
                    return MSIXErrorCode.InstallResolveDependencyFailed;
                case 0x80073D0A:
                    return MSIXErrorCode.InstallFirewallServiceNotRunning;
                case 0x80080200:
                    return MSIXErrorCode.AppxPackagingInternal;//The packaging API has encountered an internal error.
                case 0x80080201:
                    return MSIXErrorCode.AppxInterleavingNotAllowed;//The file is not a valid package because its contents are interleaved.
                case 0x80080202:
                    return MSIXErrorCode.AppxRelationshipsNotAllowed;//The file is not a valid package because it contains OPC relationships.
                case 0x80080203:
                    return MSIXErrorCode.AppxMissingRequiredFile;//The file is not a valid package because it is missing a manifest or block map, or missing a signature file when the code integrity file is present.
                case 0x80080204:
                    return MSIXErrorCode.AppxInvalidManifest;//The package's manifest is invalid.
                case 0x80080205:
                    return MSIXErrorCode.AppxInvalidBlockmap;//The package's block map is invalid.
                case 0x80080206:
                    return MSIXErrorCode.AppxCorruptContent;//The package's content cannot be read because it is corrupt.
                case 0x80080207:
                    return MSIXErrorCode.AppxBlockHashInvalid;//The computed hash value of the block does not match the one stored in the block map.
                case 0x80080208:
                    return MSIXErrorCode.AppxRequestedRangeTooLarge;//The requested byte range is over 4GB when translated to byte range of blocks.
                case 0x80080209:
                    return MSIXErrorCode.AppxInvalidSipClientData;//The SIP_SUBJECTINFO structure used to sign the package didn't contain the required data.
                case 0x800B0100:
                    return MSIXErrorCode.TrustNoSignature;
                case 0x800B0109:
                    return MSIXErrorCode.CertUntrustedRoot;
                case 0x800B010A:
                    return MSIXErrorCode.CertChaining;
                default:
                    Console.WriteLine("UNTRACKED ERROR: 0x" + code.ToString("X8"));
                    return MSIXErrorCode.Unknown;
            }
        }

        //This function converts the windows error codes into internal error categories.
        private MSIXErrorType InterpretErrorCode(MSIXErrorCode code)
        {
            switch(code)
            {
                case MSIXErrorCode.AppxBlockHashInvalid:
                case MSIXErrorCode.AppxCorruptContent:
                case MSIXErrorCode.AppxInterleavingNotAllowed:
                case MSIXErrorCode.AppxInvalidBlockmap:
                case MSIXErrorCode.AppxInvalidManifest:
                case MSIXErrorCode.AppxInvalidSipClientData:
                case MSIXErrorCode.AppxMissingRequiredFile:
                case MSIXErrorCode.AppxRelationshipsNotAllowed:
                case MSIXErrorCode.BadFormat:
                case MSIXErrorCode.InstallInvalidPackage:
                case MSIXErrorCode.InstallOpenPackageFailed:
                case MSIXErrorCode.InvalidArg:
                case MSIXErrorCode.InvalidStagedSignature:
                case MSIXErrorCode.TrustNoSignature:
                    return MSIXErrorType.PackageError;      //PACKAGE ERROR
                case MSIXErrorCode.AppxIntegrityFailureExternal:
                case MSIXErrorCode.AppxPackagingInternal:
                case MSIXErrorCode.AppxRequestedRangeTooLarge:
                case MSIXErrorCode.CertChaining:
                case MSIXErrorCode.CertUntrustedRoot:
                case MSIXErrorCode.DeletingExistingApplicationdataStoreFailed:
                case MSIXErrorCode.DeploymentBlockedByPolicy:
                case MSIXErrorCode.InstallDeregistrationFailure:
                case MSIXErrorCode.InstallPackageDowngrade:
                case MSIXErrorCode.InstallPackageNotFound:
                case MSIXErrorCode.InstallPolicyFailure:
                case MSIXErrorCode.InstallPrerequisiteFailed:
                case MSIXErrorCode.InstallRegistrationFailure:
                case MSIXErrorCode.InstallResolveDependencyFailed:
                case MSIXErrorCode.RecoveryFileCorrupt:
                case MSIXErrorCode.ResiliencyFileCorrupt:
                    return MSIXErrorType.OSError;           //OS ERROR
                case MSIXErrorCode.NeedsRemediation:
                case MSIXErrorCode.PackageRepositoryCorrupted:
                    return MSIXErrorType.OSReset;           //OS RESET
                case MSIXErrorCode.InstallFirewallServiceNotRunning:
                case MSIXErrorCode.InstallNetworkFailure:
                case MSIXErrorCode.PackagesInUse:
                case MSIXErrorCode.PackageUpdating:
                case MSIXErrorCode.SystemNeedsRemediation:
                    return MSIXErrorType.RetryPossible;     //RETRY POSSIBLE
                case MSIXErrorCode.InstallOutOfDiskSpace:
                    return MSIXErrorType.OutOfSpace;        //OUT OF SPACE
                case MSIXErrorCode.PackageAlreadyExists:
                    return MSIXErrorType.AlreadyExists;
                case MSIXErrorCode.InstallCancel:
                    return MSIXErrorType.ManualHalt;        //MANUAL HALT
                case MSIXErrorCode.InstallFailed:
                case MSIXErrorCode.RemoveFailed:
                case MSIXErrorCode.Unknown:
                default:
                    return MSIXErrorType.MiscFailure;       //MISC ERROR
            }
        }
    }
}
