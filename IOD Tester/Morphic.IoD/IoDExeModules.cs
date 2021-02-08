using System;
using System.Diagnostics;
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
                    //exe.StartInfo.RedirectStandardOutput = true;
                    //exe.StartInfo.RedirectStandardError = true;

                    exe.Start();
                    exe.WaitForExit();
                    string? line = exe.StandardOutput.ReadLine();
                    while(line != null)
                    {
                        Console.WriteLine(line);
                        line = exe.StandardOutput.ReadLine();
                    }
                    line = exe.StandardError.ReadLine();
                    while(line != null)
                    {
                        Console.WriteLine(line);
                        line = exe.StandardError.ReadLine();
                    }

                    if (exe.ExitCode == 0)
                    {
                        return IoDStatus.OK;
                    }
                    else
                    {
                        return IoDStatus.MiscFailure;
                    }
                }
            }
            catch
            {
                return IoDStatus.MiscFailure;
            }
        }
    }
}
