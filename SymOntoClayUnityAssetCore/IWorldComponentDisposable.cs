using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IWorldComponentDisposable: IDisposable
    {
        bool IsDisposed { get; }
    }
}
