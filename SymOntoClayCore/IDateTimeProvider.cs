using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IDateTimeProvider
    {
        long CurrentTiks { get; }
    }
}
