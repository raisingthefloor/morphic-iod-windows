using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Management.Deployment;

namespace Morphic.IoD
{
    public class IoDMsiXInstaller : IoDModule
    {
        public IoDMsiXInstaller(string filepath)
        {
            try
            {
                path = new Uri(filepath);
            }
            catch
            {
                badparams = true;
            }
            status = IoDStatus.OK;
            percentProgress = 0.0;
            verbose = false;
        }

        public async Task<IoDStatus> RunAsync()
        {
            done = false;
            status = IoDStatus.ProgramInUse;
            if(badparams)
            {
                return IoDStatus.BadParams;
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
                        status = IoDStatus.ManualHalt;
                        Console.WriteLine("Error Code: " + info.ErrorCode);
                        break;
                    case Windows.Foundation.AsyncStatus.Completed:
                        status = IoDStatus.OK;
                        break;
                    case Windows.Foundation.AsyncStatus.Error:
                        status = IoDStatus.MiscFailure;
                        Console.WriteLine("Error Code: " + info.ErrorCode);
                        break;
                    case Windows.Foundation.AsyncStatus.Started:
                        status = IoDStatus.MiscFailure; //not sure how this would happen
                        Console.WriteLine("Error Code: " + info.ErrorCode);
                        break;
                }
                done = true;
            };
            while (!done)
            {
                await Task.Delay(10);
            }
            return status;
        }

        public double getProgress()
        {
            return percentProgress;
        }

        public bool verbose;
        private bool done;
        private IoDStatus status;
        private double percentProgress;
        private Uri path;
        private bool badparams = false;
    }
}
