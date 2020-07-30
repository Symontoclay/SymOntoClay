using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Helpers
{
    public class TstMainStorageContext : MainStorageContext
    {
        public TstMainStorageContext()
            : base(new LoggerImpementation())
        {
            EngineContextHelper.BaseInitMainStorageContext(this, new StandaloneStorageSettings(), KindOfStorage.Host);
        }
    }
}
