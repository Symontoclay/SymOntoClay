/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ConditionalEntityValue : LoggedValue, IEntity, INavTarget
    {
        public ConditionalEntityValue(EntityConditionExpressionNode entityConditionExpression, RuleInstance logicalQuery, StrongIdentifierValue name, IEngineContext context, LocalCodeExecutionContext localContext)
            : base(context.Logger)
        {
            _context = context;
            _conditionalEntityHostSupport = context.ConditionalEntityHostSupport;
            _localContext = localContext;
            _worldPublicFactsStorage = context.Storage.WorldPublicFactsStorage;

            Expression = entityConditionExpression;
            LogicalQuery = logicalQuery;
            Name = name;

            var dataResolversFactory = context.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, new List<IStorage> { localContext.Storage, _worldPublicFactsStorage });
            _storage = new LocalStorage(localStorageSettings);
            _localCodeExecutionContext.Storage = _storage;

            _searchOptions = new LogicalSearchOptions();
            _searchOptions.QueryExpression = logicalQuery;
            _searchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;
        }
     
        public EntityConditionExpressionNode Expression { get; private set; }
        public StrongIdentifierValue Name { get; private set; }
        public RuleInstance LogicalQuery { get; private set; }

        private IEngineContext _context;
        private IConditionalEntityHostSupport _conditionalEntityHostSupport;
        private LocalCodeExecutionContext _localContext;
        private IStorage _worldPublicFactsStorage;
        private readonly LogicalSearchResolver _searcher;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly IStorage _storage;
        private LogicalSearchOptions _searchOptions;

        /// <inheritdoc/>
        public int InstanceId
        {
            get
            {
                CheckForUpdates();

                return _instanceId;
            }
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                CheckForUpdates();

                return _id;
            }
        }

        /// <inheritdoc/>
        public string IdForFacts
        {
            get
            {
                CheckForUpdates();

                return _idForFacts;
            }
        }

        /// <inheritdoc/>
        public Vector3? Position
        {
            get
            {
                CheckForUpdates();

                return _position;
            }
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            get
            {
                return _isEmpty;
            }
        }

        /// <inheritdoc/>
        public void Specify(params EntityConstraints[] constraints)
        {
            _constraints = constraints;
            _onceStorage = null;
            _specifiedOnce = false;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void SpecifyOnce(params EntityConstraints[] constraints)
        {
            _constraints = constraints;
            _onceStorage = null;
            _specifiedOnce = true;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void SpecifyOnce(IStorage storage)
        {
            _constraints = null;
            _specifiedOnce = true;
            _onceStorage = storage;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void ResolveIfNeeds()
        {
            CheckForUpdates();
        }

        /// <inheritdoc/>
        public void Resolve()
        {
            NResolve();
        }

        private bool _needUpdate = true;
        private bool _specifiedOnce;
        private IStorage _onceStorage;
        private int _instanceId;
        private string _id;
        private string _idForFacts;
        private Vector3? _position;
        private bool _isEmpty = true;
        private EntityConstraints[] _constraints;

        private struct FilteredItem
        {
            public StrongIdentifierValue Id { get; set; }
            public int InstanceId { get; set; }
            public Vector3? Position { get; set; }
        }

        private void CheckForUpdates()
        {
            if (!_needUpdate)
            {
                return;
            }

            NResolve();
        }

        private void NResolve()
        {
            try
            {
#if DEBUG
                //Log("Begin");
#endif

                _needUpdate = false;

#if DEBUG
                //Log($"constraints = {JsonConvert.SerializeObject(_constraints?.Select(p => p.ToString()))}");
#endif

#if DEBUG
                //Log($"_searchOptions = {_searchOptions}");
#endif

                var searchOptions = _searchOptions;

                if (_onceStorage != null)
                {
#if DEBUG
                    //Log("_onceStorage != null");
                    //_onceStorage.DbgPrintFactsAndRules();
#endif

                    searchOptions = new LogicalSearchOptions();
                    searchOptions.QueryExpression = LogicalQuery;
                    searchOptions.TargetStorage = _onceStorage;
                    searchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;  
                }
                
                var searchResult = _searcher.Run(searchOptions);

#if DEBUG
                //Log($"searchResult = {searchResult}");
                //Log($"_condition = {DebugHelperForRuleInstance.ToString(LogicalQuery)}");
                //Log($"searchResult = {DebugHelperForLogicalSearchResult.ToString(searchResult)}");
                //_worldPublicFactsStorage.DbgPrintFactsAndRules();
#endif

                if (searchResult.IsSuccess)
                {
                    if (searchResult.Items.Count == 0)
                    {
                        ResetCurrEntity();
                    }
                    else
                    {
                        ProcessResultWithItems(searchResult);
                    }
                }
                else
                {
                    ResetCurrEntity();
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
                Error(e);
            }
        }

        private void ProcessResultWithItems(LogicalSearchResult searchResult)
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
            //Log($"foundIdsList = {JsonConvert.SerializeObject(foundIdsList?.Select(p => p.NameValue))}");
#endif

            if (!foundIdsList.Any())
            {
                ResetCurrEntity();
                return;
            }

            if (_constraints.IsNullOrEmpty())
            {
                SetCurrEntity(foundIdsList.First());
                return;
            }

            var filteredItemsList = new List<FilteredItem>();

            float? minDistance = null;

            foreach (var foundId in foundIdsList)
            {
#if DEBUG
                //Log($"foundId = {foundId}");
#endif

                var instanceId = _conditionalEntityHostSupport.GetInstanceId(foundId);

#if DEBUG
                //Log($"instanceId = {instanceId}");
#endif

                if (instanceId == 0)
                {
                    continue;
                }

                var isFit = true;

                float? currentMinDistance = null;
                Vector3? currentPosition = null;
                var clearFilteredItemsList = false;

                foreach (var constraint in _constraints)
                {
                    if(!isFit)
                    {
                        break;
                    }

#if DEBUG
                    //Log($"constraint = {constraint}");
#endif

                    switch (constraint)
                    {
                        case EntityConstraints.CanBeTaken:
                            if(!_conditionalEntityHostSupport.CanBeTaken(instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.OnlyVisible:
                            if(!_conditionalEntityHostSupport.IsVisible(instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.OnlyInvisible:
                            if (_conditionalEntityHostSupport.IsVisible(instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.Nearest:
                            var distanceToAndPosition = _conditionalEntityHostSupport.DistanceToAndPosition(instanceId);

                            var distance = distanceToAndPosition.Item1;

                            if (distance.HasValue)
                            {
                                currentPosition = distanceToAndPosition.Item2;

                                if (minDistance.HasValue)
                                {
                                    if(minDistance == distance)
                                    {
                                        currentMinDistance = distance;
                                    }
                                    else
                                    {
                                        if(minDistance < distance)
                                        {
                                            currentMinDistance = distance;
                                            
                                            clearFilteredItemsList = true;
                                        }
                                        else
                                        {
                                            isFit = false;
                                        }
                                    }
                                }
                                else
                                {
                                    currentMinDistance = distance;
                                }

                                break;
                            }

                            isFit = false;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
                    }
                }

#if DEBUG
                //Log($"isFit = {isFit}");
#endif

                if (!isFit)
                {
                    continue;
                }

                if (clearFilteredItemsList)
                {
                    filteredItemsList.Clear();
                }

                if(currentMinDistance.HasValue && currentMinDistance < minDistance)
                {
                    minDistance = currentMinDistance;
                }

                filteredItemsList.Add(new FilteredItem() 
                { 
                    Id = foundId, 
                    InstanceId = instanceId,
                    Position = currentPosition
                });
            }

            if(!filteredItemsList.Any())
            {
                ResetCurrEntity();
                return;
            }

            var targetFilteredItem = filteredItemsList.First();

            SetCurrEntity(targetFilteredItem.Id, targetFilteredItem.InstanceId, targetFilteredItem.Position);
        }

        private void SetCurrEntity(StrongIdentifierValue id)
        {
            var instanceId = _conditionalEntityHostSupport.GetInstanceId(id);

            if(instanceId == 0)
            {
                ResetCurrEntity();
                return;
            }

            SetCurrEntity(id, instanceId, null);
        }

        private void SetCurrEntity(StrongIdentifierValue id, int instanceId, Vector3? position)
        {
            if (!position.HasValue)
            {
                position = _conditionalEntityHostSupport.GetPosition(instanceId);
            }

            _id = id.NameValue;
            _idForFacts = _id;
            _instanceId = instanceId;
            _position = position;
            _isEmpty = false;
        }

        private void ResetCurrEntity()
        {
            _id = string.Empty;
            _idForFacts = string.Empty;
            _instanceId = 0;
            _position = null;
            _isEmpty = true;
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException();
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
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new ConditionalEntityValue(Expression?.Clone(cloneContext), LogicalQuery?.Clone(cloneContext), Name?.Clone(cloneContext), _context, _localContext);
            cloneContext[this] = result;

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        KindOfNavTarget INavTarget.Kind => KindOfNavTarget.ByEntity;

        /// <inheritdoc/>
        float INavTarget.Distance => 0f;

        /// <inheritdoc/>
        float INavTarget.HorizontalAngle => 0f;

        /// <inheritdoc/>
        Vector3 INavTarget.AbcoluteCoordinates => Vector3.Zero;

        /// <inheritdoc/>
        IEntity INavTarget.Entity => this;

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
    }
}
