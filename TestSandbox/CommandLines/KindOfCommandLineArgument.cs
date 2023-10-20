using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.CommandLines
{
    public enum KindOfCommandLineArgument
    {
        Flag,
        SingleValue,
        List,
        SingleValueOrList
    }
}
