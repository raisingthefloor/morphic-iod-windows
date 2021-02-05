using System;
using System.Threading.Tasks;

namespace Morphic.IoD
{
    public enum IoDStatus
    {
        OK,
        MiscFailure,
        InUse,
        ManualHalt,
        BadParams,
        FileLocked,
        NoSpace
    }

    interface IoDModule
    {
        public Task<IoDStatus> Run();
    }
}
