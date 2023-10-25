using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CLI.Helpers
{
    public enum KindOfCommandLineArgument
    {
        Flag,
        SingleValue,
        List,
        SingleValueOrList
    }
}
