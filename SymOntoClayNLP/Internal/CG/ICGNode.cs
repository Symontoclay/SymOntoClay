using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.CG
{
    public interface ICGNode : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        KindOfCGNode Kind { get; }
        string Name { get; set; }
        IList<ICGNode> ChildrenNodes { get; }
        IList<ICGNode> InputNodes { get; }
        IList<ICGNode> OutputNodes { get; }
    }
}
