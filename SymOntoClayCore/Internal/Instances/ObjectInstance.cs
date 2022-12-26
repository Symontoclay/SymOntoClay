using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ObjectInstance: BaseIndependentInstance
    {
        public ObjectInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage, LocalCodeExecutionContext parentCodeExecutionContext)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, new ObjectStorageFactory(), null)
        {
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.ObjectInstance;
    }
}
