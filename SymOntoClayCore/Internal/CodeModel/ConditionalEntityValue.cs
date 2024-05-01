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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.CodeModel.MonitorSerializableObjects;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ConditionalEntityValue : BaseEntityValue
    {
        public ConditionalEntityValue(EntityConditionExpressionNode entityConditionExpression, RuleInstance logicalQuery, StrongIdentifierValue name, IEngineContext context, ILocalCodeExecutionContext localContext)
            : base(context, localContext)
        {
            _context = context;            
            _localContext = localContext;

            Expression = entityConditionExpression;
            LogicalQuery = logicalQuery;
            Name = name;

            var entityConstraintsConcepts = logicalQuery.GetStandaloneConcepts(Logger);

            if(entityConstraintsConcepts.Any())
            {
                var entityConstraintsService = context.ServicesFactory.GetEntityConstraintsService();

                var constraintsList = new List<EntityConstraints>();

                foreach (var entityConstraintConcept in entityConstraintsConcepts)
                {
                    var entityConstraint = entityConstraintsService.ConvertToEntityConstraint(entityConstraintConcept);

                    constraintsList.Add(entityConstraint);
                }

                _constraints = constraintsList.Distinct().ToArray();
            }

            var dataResolversFactory = context.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext(localContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(context, new List<IStorage> { localContext.Storage/*, _worldPublicFactsStorage*/ });
            _storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = _storage;

            _localCodeExecutionContext = localCodeExecutionContext;

            _searchOptions = new LogicalSearchOptions();
            _searchOptions.QueryExpression = logicalQuery;
            _searchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;
        }

        public EntityConditionExpressionNode Expression { get; private set; }
        public StrongIdentifierValue Name { get; private set; }
        public RuleInstance LogicalQuery { get; private set; }

        private IEngineContext _context;
        private ILocalCodeExecutionContext _localContext;
        private readonly LogicalSearchResolver _searcher;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly IStorage _storage;
        private LogicalSearchOptions _searchOptions;

        /// <inheritdoc/>
        public override bool NullValueEquals()
        {
            return false;
        }

        /// <inheritdoc/>
        public override void Resolve(IMonitorLogger logger)
        {
            NResolve(logger);
        }

        /// <inheritdoc/>
        protected override void CheckForUpdates(IMonitorLogger logger)
        {
            if (!_needUpdate)
            {
                return;
            }

            NResolve(logger);
        }

        private void NResolve(IMonitorLogger logger)
        {
            try
            {
                _needUpdate = false;

                var searchOptions = _searchOptions;

                if (_onceStorage != null)
                {
                    searchOptions = new LogicalSearchOptions();
                    searchOptions.QueryExpression = LogicalQuery;
                    searchOptions.TargetStorage = _onceStorage;
                    searchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;  
                }

#if DEBUG
                //Log($"searchOptions.QueryExpression = {searchOptions.QueryExpression.ToHumanizedString()}");
#endif

                var searchResult = _searcher.Run(logger, searchOptions);

#if DEBUG
                //Log($"searchResult.IsSuccess = {searchResult.IsSuccess}");
#endif

                if (searchResult.IsSuccess)
                {
#if DEBUG
                    //Log($"searchResult.Items.Count = {searchResult.Items.Count}");
#endif

                    if (searchResult.Items.Count == 0)
                    {
                        ResetCurrEntity(logger);
                    }
                    else
                    {
                        ProcessResultWithItems(logger, searchResult);
                    }
                }
                else
                {
                    ResetCurrEntity(logger);
                }

                if(_specifiedOnce)
                {
                    _onceStorage = null;
                    _constraints = null;
                    _specifiedOnce = false;
                }
                
            }
            catch (Exception e)
            {
                logger.Error("90A3EEC7-6EAB-4318-82B1-3B285A55BE04", e);
            }
        }

        private void ProcessResultWithItems(IMonitorLogger logger, LogicalSearchResult searchResult)
        {
            var foundIdsList = new List<StrongIdentifierValue>();

            var targetVarName = "$_";

            foreach (var foundResultItem in searchResult.Items)
            {
                foreach(var resultOfVarOfQueryToRelation in foundResultItem.ResultOfVarOfQueryToRelationList)
                {
                    if(resultOfVarOfQueryToRelation.NameOfVar.NameValue == targetVarName && resultOfVarOfQueryToRelation.FoundExpression.Kind == KindOfLogicalQueryNode.Entity)
                    {
                        foundIdsList.Add(resultOfVarOfQueryToRelation.FoundExpression.Name);
                    }
                }
            }

#if DEBUG
            //Log($"foundIdsList = {foundIdsList.WriteListToString()}");
#endif

            ProcessIdsList(logger, foundIdsList);
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ConditionalEntityValue;

        /// <inheritdoc/>
        public override bool IsConditionalEntityValue => true;

        /// <inheritdoc/>
        public override ConditionalEntityValue AsConditionalEntityValue => this;

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return null;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            return ToHumanizedString();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.ConditionalEntityTypeName) };

            LogicalQuery.CheckDirty(options);

            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseFunctionWeight ^ (Name?.GetLongHashCode(options) ?? 0) ^ LongHashCodeWeights.BaseParamWeight ^ LogicalQuery.GetLongHashCode(options);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new ConditionalEntityValue(Expression?.Clone(context), LogicalQuery?.Clone(context), Name?.Clone(context), _context, _localContext);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        public override object ToMonitorSerializableObject(IMonitorLogger logger)
        {
            var result = new ConditionalEntityValueMonitorSerializableObject();
            result.Name = Name?.ToHumanizedString();
            result.Expression = Expression?.ToHumanizedString();
            result.LogicalQuery = LogicalQuery?.ToHumanizedString();

            FillUpBaseEntityValueMonitorSerializableObject(result, logger);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Expression), Expression);
            sb.PrintObjProp(n, nameof(LogicalQuery), LogicalQuery);
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Expression), Expression);
            sb.PrintShortObjProp(n, nameof(LogicalQuery), LogicalQuery);
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);
            sb.PrintBriefObjProp(n, nameof(LogicalQuery), LogicalQuery);
            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            if (Name != null && !Name.IsEmpty)
            {
                sb.Append(Name.NameValue);
            }
            else
            {
                sb.Append("#@");
            }

            if (Expression == null)
            {
                sb.Append(DebugHelperForRuleInstance.ToString(LogicalQuery, options));
            }
            else
            {
                sb.Append("(");
                sb.Append(Expression.ToHumanizedString(options));
                sb.Append(")");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = ToHumanizedString()
            };
        }
    }
}
