using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Management.Deployment;

namespace Morphic.IoD
{
    public class IoDMsiXInstaller
    {
        public IoDMsiXInstaller(string filepath, int waitTimer = 1000)
        {
            try
            {
                path = new Uri(filepath);
            }
            catch
            {
                badparams = true;
            }
            percentProgress = 0.0;
            verbose = false;
            maxWait = waitTimer;
        }

        public enum InstallError
        {
            MiscFailure,
            ProgramInUse,
            BadParams,
            ManualHalt
        }

        public async Task<IMorphicResult<bool, InstallError>> InstallAsync()
        {
            error = false;
            done = false;
            status = InstallError.ProgramInUse;
            if(badparams)
            {
                return new MorphicError<bool, InstallError>(InstallError.BadParams);
            }
            PackageManager pm = new PackageManager();
            var package = pm.AddPackageAsync(path, null, DeploymentOptions.ForceApplicationShutdown);
            package.Progress = (message, progress) =>
            {
                percentProgress = progress.percentage;
                if (verbose)
                {
                    Console.WriteLine(percentProgress.ToString() + "%");
                }
                //I can also get whether it's queued or processing from the progress object, if needed
            };
            package.Completed = (info, endStatus) =>
            {
                switch (endStatus)
                {
                    case Windows.Foundation.AsyncStatus.Canceled:
                        status = InstallError.ManualHalt;
                        error = true;
                        Console.WriteLine("Error Code: " + info.ErrorCode);
                        break;
                    case Windows.Foundation.AsyncStatus.Completed:
                        break;
                    case Windows.Foundation.AsyncStatus.Error:
                        status = InstallError.MiscFailure;
                        error = true;
                        Console.WriteLine("Error Code: " + info.ErrorCode);
                        break;
                    case Windows.Foundation.AsyncStatus.Started:
                        status = InstallError.MiscFailure; //not sure how this would happen
                        error = true;
                        Console.WriteLine("Error Code: " + info.ErrorCode);
                        break;
                }
                done = true;
            };
            while (!done)
            {
                await Task.Delay(10);
            }
            if(error)
            {
                return new MorphicError<bool, InstallError>(status);
            }
            else
            {
                return new MorphicSuccess<bool, InstallError>(true);
            }
        }

        public double getProgress()
        {
            return percentProgress;
        }

        public bool verbose;
        private bool done;
        private bool error;
        private InstallError status;
        private double percentProgress;
        private Uri path;
        private bool badparams = false;
        private int maxWait;
        private int currWait;
    }
}
