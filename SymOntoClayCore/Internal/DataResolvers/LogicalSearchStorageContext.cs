using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchStorageContext: ILogicalSearchStorageContext
    {
        public LogicalSearchStorageContext(IMainStorageContext mainStorageContext, LocalCodeExecutionContext localCodeExecutionContext, RuleInstance queryExpression)
        {
            _mainStorageContext = mainStorageContext;
            _localCodeExecutionContext = localCodeExecutionContext;

            _obligationModality = queryExpression.ObligationModality;
            _hasObligationModality = _obligationModality != null && _obligationModality.KindOfValue != KindOfValue.NullValue;
            _selfObligationModality = queryExpression.SelfObligationModality;
            _hasSelfObligationModality = _selfObligationModality != null && _selfObligationModality.KindOfValue != KindOfValue.NullValue;

            _logicalValueModalityResolver = mainStorageContext.DataResolversFactory.GetLogicalValueModalityResolver();
        }

        private readonly IMainStorageContext _mainStorageContext; 
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly Value _obligationModality;
        private readonly bool _hasObligationModality;
        private readonly Value _selfObligationModality;
        private readonly bool _hasSelfObligationModality;
        private readonly LogicalValueModalityResolver _logicalValueModalityResolver;

        public IList<T> Filter<T>(IList<T> source) 
            where T : ILogicalSearchItem
        {
            if (source.IsNullOrEmpty())
            {
                return source;
            }

            source = BaseResolver.FilterByTypeOfAccess(source, _mainStorageContext, _localCodeExecutionContext, true);

            if(_hasObligationModality)
            {
                source = source.Where(p => _logicalValueModalityResolver.IsFit(p.ObligationModality, _obligationModality)).ToList();
            }

            if(_hasSelfObligationModality)
            {
                source = source.Where(p => _logicalValueModalityResolver.IsFit(p.SelfObligationModality, _selfObligationModality)).ToList();
            }

            return source;
        }
    }
}
