using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchStorageContext: ILogicalSearchStorageContext
    {
        public LogicalSearchStorageContext(IMainStorageContext mainStorageContext, LocalCodeExecutionContext localCodeExecutionContext)
        {
            _mainStorageContext = mainStorageContext;
            _localCodeExecutionContext = localCodeExecutionContext;
        }

        private readonly IMainStorageContext _mainStorageContext; 
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;

        public IList<T> Filter<T>(IList<T> source) 
            where T : ILogicalSearchItem
        {
            if (source.IsNullOrEmpty())
            {
                return source;
            }

            source = BaseResolver.FilterByTypeOfAccess(source, _mainStorageContext, _localCodeExecutionContext, true);

            return source;
        }
    }
}
