using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public interface IObjectToHumanizedString
    {
        string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll);
        string ToHumanizedString(DebugHelperOptions options);
    }
}
