using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IProcessInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        string Id { get; }
        ProcessStatus Status { get; set; }
        IList<int> Devices { get; }
        void Start();
        void Cancel();
    }
}
