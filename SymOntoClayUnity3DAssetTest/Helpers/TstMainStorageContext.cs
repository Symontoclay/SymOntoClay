using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.Helpers
{
    public class TstMainStorageContext: MainStorageContext
    {
        public TstMainStorageContext()
            : base(new EmptyLogger())
        {
            EngineContextHelper.BaseInitMainStorageContext(this, new StandaloneStorageSettings(), KindOfStorage.Host);
        }
    }
}
