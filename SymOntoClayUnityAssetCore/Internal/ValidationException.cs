using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class ValidationException: Exception
    {
        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
