using System;

namespace SymOntoClay.Core.Internal.DataResolvers.Exceptions
{
    public class UnresolvedException : Exception
    {
        public UnresolvedException(string message)
            : base(message)
        {
        }
    }
}
