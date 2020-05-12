using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.Helpers
{
    public class EmptyLogger : IEntityLogger
    {
        public void Error(string message)
        {
        }

        public void Log(string message)
        {
        }

        public void Warning(string message)
        {
        }
    }
}
