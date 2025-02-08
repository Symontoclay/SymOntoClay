using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class StrategicTaskInstanceStorage : RealStorage
    {
        public StrategicTaskInstanceStorage(RealStorageSettings settings)
            : base(KindOfStorage.StrategicTaskInstance, settings)
        {
        }
    }
}
