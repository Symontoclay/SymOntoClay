using System;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class UnresolvedException : Exception
    {
        public UnresolvedException(string message)
            : base(message)
        {
        }
    }
}
