using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ThreadExecutorValue : Value
    {
        public ThreadExecutorValue(IThreadExecutor threadExecutor) 
        {
            throw new NotImplementedException("AE8296C1-3563-48A2-A50F-48BEE25267ED");
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ThreadExecutorValue;

        IThreadExecutor ThreadExecutor
    }
}
