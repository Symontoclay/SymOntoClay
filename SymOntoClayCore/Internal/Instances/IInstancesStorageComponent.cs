using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public interface IInstancesStorageComponent
    {
        void ActivateMainEntity();
        void AppendProcessInfo(IProcessInfo processInfo);
        void AppendAndTryStartProcessInfo(IProcessInfo processInfo);

#if DEBUG
        void PrintProcessesList();
#endif
    }
}
