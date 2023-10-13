using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CLI.Helpers
{
    public interface IInheritableConfiguration: IObjectToString
    {
        string ParentCfg { get; }
        void Write(IInheritableConfiguration source);
    }
}
