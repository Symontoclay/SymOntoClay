using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class ValidateException: Exception
    {
        public ValidateException(string message)
            : base(message)
        {
        }
    }
}
