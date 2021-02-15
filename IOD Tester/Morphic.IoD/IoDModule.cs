using System;
using System.Threading.Tasks;

namespace Morphic.IoD
{
    public enum IoDStatus
    {
        OK,
        MiscFailure,
        ProgramInUse,
        ManualHalt,
        BadParams,
        FileLocked,
        NoSpace
    }

    public static class IoDStatusExtension
    {
        public static string inEnglish(this IoDStatus status)
        {
            switch (status)
            {
                case IoDStatus.OK:
                    return "OK";
                case IoDStatus.MiscFailure:
                    return "Miscellaneous Error";
                case IoDStatus.ProgramInUse:
                    return "Installer In Use";
                case IoDStatus.ManualHalt:
                    return "Installer was manually cancelled";
                case IoDStatus.BadParams:
                    return "Parameters of installer were invalid";
                case IoDStatus.FileLocked:
                    return "File location in use or locked";
                case IoDStatus.NoSpace:
                    return "Insufficient free space";
            }
            return "???";
        }
    }

    interface IoDModule
    {
        public Task<IoDStatus> RunAsync();
        public double getProgress();
    }
}
