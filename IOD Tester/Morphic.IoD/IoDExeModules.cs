using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.IoD
{
    public class IoDExeLauncher : IoDModule
    {
        public IoDExeLauncher(string filename, string arguments)
        {
            this.filename = filename;
            this.arguments = arguments;
        }

        private string filename;
        private string arguments;

        public async Task<IoDStatus> Run()
        {
            try
            {
                using (Process exe = new Process())
                {
                    exe.StartInfo.FileName = filename;
                    exe.StartInfo.Arguments = arguments;
                    exe.StartInfo.UseShellExecute = true;
                    exe.StartInfo.CreateNoWindow = true;

                    exe.Start();
                    exe.WaitForExit();
                    return IoDStatus.OK;
                }
            }
            catch
            {
                return IoDStatus.MiscFailure;
            }
        }
    }
}
