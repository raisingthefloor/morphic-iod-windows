using Morphic.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Morphic.IoD
{
    public class IoDExeLauncher
    {
        public IoDExeLauncher(string filename, string arguments)
        {
            this.filename = filename;
            this.arguments = arguments;
        }

        private string filename;
        private string arguments;

        public enum InstallError
        {
            MiscError
        }
        public async Task<IMorphicResult<bool, InstallError>> Install()
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

#nullable enable
                    exe.Start();
                    exe.WaitForExit();
                    /*
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
                    }*/
#nullable disable
                    if (exe.ExitCode == 0)
                    {
                        return IMorphicResult<bool, InstallError>.SuccessResult(true);
                    }
                    else
                    {
                        return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.MiscError);
                    }
                }
            }
            catch
            {
                return IMorphicResult<bool, InstallError>.ErrorResult(InstallError.MiscError);
            }
        }
    }
}
