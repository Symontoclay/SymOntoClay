using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core
{
    public interface IProcessInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        string Id { get; }
        ProcessStatus Status { get; set; }
        IList<int> Devices { get; }
        Task SystemTask { get; }
        void Start();
        void Cancel();
    }
}
