/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchStorageContext: BaseLoggedComponent, ILogicalSearchStorageContext
    {
        public LogicalSearchStorageContext(IMainStorageContext mainStorageContext, ILocalCodeExecutionContext localCodeExecutionContext, RuleInstance queryExpression)
            : base(mainStorageContext.Logger)
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
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly Value _obligationModality;
        private readonly bool _hasObligationModality;
        private readonly Value _selfObligationModality;
        private readonly bool _hasSelfObligationModality;
        private readonly LogicalValueModalityResolver _logicalValueModalityResolver;

        /// <inheritdoc/>
        public IList<T> Filter<T>(IMonitorLogger logger, IList<T> source, bool enableModalitiesControll) 
            where T : ILogicalSearchItem
        {
            return Filter(logger, source, enableModalitiesControll, null);
        }

        /// <inheritdoc/>
        public IList<T> Filter<T>(IMonitorLogger logger, IList<T> source, IDictionary<RuleInstance, IItemWithModalities> additionalModalities) 
            where T : ILogicalSearchItem
        {
            return Filter(logger, source, true, additionalModalities);
        }

        /// <inheritdoc/>
        public IList<T> Filter<T>(IMonitorLogger logger, IList<T> source, bool enableModalitiesControll, IDictionary<RuleInstance, IItemWithModalities> additionalModalities)
            where T : ILogicalSearchItem
        {
            if (source.IsNullOrEmpty())
            {
                return source;
            }

            source = _logicalValueModalityResolver.FilterByTypeOfAccess(logger, source, _localCodeExecutionContext, true);

            if (enableModalitiesControll)
            {
                if (_hasObligationModality)
                {
                    if(additionalModalities == null)
                    {
                        source = source.Where(p => _logicalValueModalityResolver.IsFit(logger, p.ObligationModality, _obligationModality, _localCodeExecutionContext)).ToList();
                    }
                    else
                    {
                        source = source.Where(p => IsFit(logger, p, p.ObligationModality, _obligationModality, _localCodeExecutionContext, KindOfCheckedModality.ObligationModality, additionalModalities)).ToList();
                    }                    
                }

                if (_hasSelfObligationModality)
                {
                    if (additionalModalities == null)
                    {
                        source = source.Where(p => _logicalValueModalityResolver.IsFit(logger, p.SelfObligationModality, _selfObligationModality, _localCodeExecutionContext)).ToList();
                    }
                    else
                    {
                        source = source.Where(p => IsFit(logger, p, p.SelfObligationModality, _selfObligationModality, _localCodeExecutionContext, KindOfCheckedModality.SelfObligationModality, additionalModalities)).ToList();
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

        private bool IsFit(IMonitorLogger logger, ILogicalSearchItem item, Value modalityValue, Value queryModalityValue, ILocalCodeExecutionContext localCodeExecutionContext, KindOfCheckedModality kindOfCheckedModality, IDictionary<RuleInstance, IItemWithModalities> additionalModalities)
        {
            var originalRuleInstance = item.RuleInstance.Original;

            if (additionalModalities.ContainsKey(originalRuleInstance))
            {
                var additionalPart = additionalModalities[originalRuleInstance];

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
                    return _logicalValueModalityResolver.IsFit(logger, modalityValue, queryModalityValue, _localCodeExecutionContext);
                }

                return _logicalValueModalityResolver.IsFit(logger, additionalModalityValue, queryModalityValue, _localCodeExecutionContext);
            }

            return _logicalValueModalityResolver.IsFit(logger, modalityValue, queryModalityValue, _localCodeExecutionContext);
        }
    }
}
