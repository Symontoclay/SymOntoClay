using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IPlatformHostListener
    {
        IPlatformCommandInfo GetCommandInfo(IPlatformCommand command);
        IPlatformCommandCallResult CallCommand(IPlatformCommand command);
    }
}
