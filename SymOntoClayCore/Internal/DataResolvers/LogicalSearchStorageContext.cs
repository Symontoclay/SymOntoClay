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

        /// <inheritdoc/>
        public IList<T> Filter<T>(IList<T> source, bool enableModalitiesControll) 
            where T : ILogicalSearchItem
        {
            return Filter(source, enableModalitiesControll, null);
        }

        /// <inheritdoc/>
        public IList<T> Filter<T>(IList<T> source, IDictionary<RuleInstance, IItemWithModalities> additionalModalities) 
            where T : ILogicalSearchItem
        {
            return Filter(source, true, additionalModalities);
        }

        /// <inheritdoc/>
        public IList<T> Filter<T>(IList<T> source, bool enableModalitiesControll, IDictionary<RuleInstance, IItemWithModalities> additionalModalities)
            where T : ILogicalSearchItem
        {
            if (source.IsNullOrEmpty())
            {
                return source;
            }

            source = BaseResolver.FilterByTypeOfAccess(source, _mainStorageContext, _localCodeExecutionContext, true);

            if (enableModalitiesControll)
            {
                if (_hasObligationModality)
                {
                    if(additionalModalities == null)
                    {
                        source = source.Where(p => _logicalValueModalityResolver.IsFit(p.ObligationModality, _obligationModality, _localCodeExecutionContext)).ToList();
                    }
                    else
                    {
                        source = source.Where(p => IsFit(p, p.ObligationModality, _obligationModality, _localCodeExecutionContext, KindOfCheckedModality.ObligationModality, additionalModalities)).ToList();
                    }                    
                }

                if (_hasSelfObligationModality)
                {
                    if (additionalModalities == null)
                    {
                        source = source.Where(p => _logicalValueModalityResolver.IsFit(p.SelfObligationModality, _selfObligationModality, _localCodeExecutionContext)).ToList();
                    }
                    else
                    {
                        source = source.Where(p => IsFit(p, p.SelfObligationModality, _selfObligationModality, _localCodeExecutionContext, KindOfCheckedModality.SelfObligationModality, additionalModalities)).ToList();
                    }                    
                }
            }

            return source;
        }

        private enum KindOfCheckedModality
        {
            ObligationModality,
            SelfObligationModality
        }

        private bool IsFit(ILogicalSearchItem item, Value modalityValue, Value queryModalityValue, LocalCodeExecutionContext localCodeExecutionContext, KindOfCheckedModality kindOfCheckedModality, IDictionary<RuleInstance, IItemWithModalities> additionalModalities)
        {
            if(additionalModalities.ContainsKey(item.RuleInstance))
            {
                var additionalPart = additionalModalities[item.RuleInstance];

                Value additionalModalityValue = null;

                switch (kindOfCheckedModality)
                {
                    case KindOfCheckedModality.ObligationModality:
                        additionalModalityValue = additionalPart.ObligationModality;
                        break;

                    case KindOfCheckedModality.SelfObligationModality:
                        additionalModalityValue = additionalPart.SelfObligationModality;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfCheckedModality), kindOfCheckedModality, null);
                }

                if(additionalModalityValue == null)
                {
                    return _logicalValueModalityResolver.IsFit(modalityValue, queryModalityValue, _localCodeExecutionContext);
                }

                return _logicalValueModalityResolver.IsFit(additionalModalityValue, queryModalityValue, _localCodeExecutionContext);
            }

            return _logicalValueModalityResolver.IsFit(modalityValue, queryModalityValue, _localCodeExecutionContext);
        }
    }
}
