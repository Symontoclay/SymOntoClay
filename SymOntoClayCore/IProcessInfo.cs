using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core
{
    public delegate void ProcessInfoEvent(IProcessInfo sender);

    public interface IProcessInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        string Id { get; }
        ProcessStatus Status { get; set; }
        bool IsFinished { get; }
        IReadOnlyList<int> Devices { get; }
        void Start();
        void Cancel();
        event ProcessInfoEvent OnFinish;
        float Priority { get; }
        float GlobalPriority { get; }
        IProcessInfo ParentProcessInfo { get; set; }
        IReadOnlyList<IProcessInfo> GetChildrenProcessInfoList { get; }
        void AddChild(IProcessInfo processInfo);
        void RemoveChild(IProcessInfo processInfo);
    }
}
