using System;
using System.Collections.Generic;
using System.Text;

namespace Morphic.IoD
{
    public enum InstallStatus
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
        public InstallStatus Install();
        public InstallStatus Uninstall();
    }
}
