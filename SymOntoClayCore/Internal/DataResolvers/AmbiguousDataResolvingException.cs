using SymOntoClay.Core.Internal.CodeModel;
using System;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class AmbiguousDataResolvingException : Exception
    {
        public AmbiguousDataResolvingException(StrongIdentifierValue name)
            : base($"Ambiguous name resolving '{name.ForResolving?.ToHumanizedLabel()}'.")
        {
        }
    }
}
