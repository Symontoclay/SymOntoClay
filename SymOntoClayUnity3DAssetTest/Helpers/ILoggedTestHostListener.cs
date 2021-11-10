using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests.Helpers
{
    public interface ILoggedTestHostListener
    {
        void SetLogger(IEntityLogger logger);
    }
}
