using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Provides small improving of mechanism for releasing unmanaged resources by adding additional members.
    /// </summary>
    public interface IWorldComponentDisposable: IDisposable
    {
        /// <summary>
        /// Returns <c>true</c> if the instance was disposed, otherwise returns <c>false</c>.
        /// </summary>
        bool IsDisposed { get; }
    }
}
