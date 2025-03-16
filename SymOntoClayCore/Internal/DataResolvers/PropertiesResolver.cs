using Newtonsoft.Json;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class PropertiesResolver : BaseResolver
    {
        public PropertiesResolver(IMainStorageContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _codeExecutorComponent = _context.CodeExecutor;
            _standardCoreFactsBuilder = _context.StandardFactsBuilder;
            _logicalSearchResolver = _context.DataResolversFactory.GetLogicalSearchResolver();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();

            _targetLogicalVarName = _context.CommonNamesStorage.TargetLogicalVarName;
        }

        private ICodeExecutorComponent _codeExecutorComponent;
        private IStandardCoreFactsBuilder _standardCoreFactsBuilder;
        private LogicalSearchResolver _logicalSearchResolver;
        private StrongIdentifierValue _targetLogicalVarName;

        public ResolverOptions DefaultOptions { get; private set; } = ResolverOptions.GetDefaultOptions();

        public CallResult SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return SetPropertyValue(logger, propertyName, value, localCodeExecutionContext, callMode, DefaultOptions);
        }

        public CallResult SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode, ResolverOptions options)
        {
#if DEBUG
            //Info("BAEC7F05-B0F0-42C2-BFEB-43B5AC51B208", $"propertyName = {propertyName}");
            //Info("54CD61AD-5D26-4C26-9751-AE243769B0A4", $"value = {value}");
            //Info("E981235C-808D-4F6B-AF59-97213B85599D", $"callMode = {callMode}");
#endif

            var property = Resolve(logger, propertyName, localCodeExecutionContext);

#if DEBUG
            //Info("1B9FF0A5-D834-409F-A555-4E447E8C71DE", $"property = {property}");
#endif

            switch(callMode)
            {
                case CallMode.PreConstructor:
                    if (property == null)
                    {
                        throw new NotImplementedException("C1DA392F-6F4F-4DB3-8DFA-AE49E3CD0354");
                    }
                    else
                    {
                        return property.SetValue(logger, value, localCodeExecutionContext);
                    }

                case CallMode.Default:
                    if (property == null)
                    {
                        throw new NotImplementedException("05DDD3F9-E954-464B-B60C-1C520F45CC5C");
                    }
                    else
                    {
                        var kindOfProperty = property.KindOfProperty;

                        switch (kindOfProperty)
                        {
                            case KindOfProperty.Auto:
                                return property.SetValue(logger, value, localCodeExecutionContext);

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfProperty), kindOfProperty, null);
                        }
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(callMode), callMode, null);
            }
        }

        public PropertyInstance Resolve(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, propertyName, localCodeExecutionContext, DefaultOptions);
        }

        public PropertyInstance Resolve(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("F003D1F2-A299-411F-932C-7C226A0D13CC", $"propertyName = {propertyName}");
#endif

            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.Property);

#if DEBUG
            //Info("C6A2F71F-4B86-4418-B806-82114687C6B4", $"storagesList.Count = {storagesList.Count}");
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Info("35C3DF0D-8B07-4F45-8741-1526A7AE5C64", $"weightedInheritanceItems.Count = {weightedInheritanceItems.Count}");
#endif

            var rawList = GetRawPropertiesList(logger, propertyName, storagesList, weightedInheritanceItems);

#if DEBUG
            //Info("971B9D45-54A7-4D6C-8939-66FCE52F227D", $"rawList.Count = {rawList.Count}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = FilterCodeItems(logger, rawList, localCodeExecutionContext.Holder, localCodeExecutionContext);

#if DEBUG
            //Info("A5B248B1-2917-4940-9B9A-37DF79146444", $"filteredList.Count = {filteredList.Count}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            var minStorageDistance = filteredList.Min(p => p.StorageDistance);

            filteredList = filteredList.Where(p => p.StorageDistance == minStorageDistance).ToList();

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return OrderAndDistinctByInheritance(logger, filteredList, options).FirstOrDefault()?.ResultItem;
        }

        public List<PropertyInstance> GetReadOnlyPropertiesList(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetReadOnlyPropertiesList(logger, propertyName, localCodeExecutionContext, DefaultOptions);
        }

        public List<PropertyInstance> GetReadOnlyPropertiesList(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("8B0595DD-33EA-48EB-8AAC-428911BC6001", $"propertyName = {propertyName}");
#endif

            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.Property);

#if DEBUG
            //Info("4C93F0C7-4978-42E4-9E50-84D712A276CA", $"storagesList.Count = {storagesList.Count}");
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Info("AA78B437-AA60-4900-AA81-771387B650A7", $"weightedInheritanceItems.Count = {weightedInheritanceItems.Count}");
#endif

            var rawList = GetRawPropertiesList(logger, propertyName, storagesList, weightedInheritanceItems).Where(p => p.ResultItem.KindOfProperty == KindOfProperty.Readonly).ToList();

#if DEBUG
            //Info("47A0DA6A-C8B9-473C-9CCB-FA92B2853FE9", $"rawList.Count = {rawList.Count}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = FilterCodeItems(logger, rawList, localCodeExecutionContext.Holder, localCodeExecutionContext);

#if DEBUG
            //Info("04357540-F863-4D7E-B7DC-3786CD3D5F8C", $"filteredList.Count = {filteredList.Count}");
#endif

            return filteredList.Select(p => p.ResultItem).ToList();
        }

        public List<LogicalQueryNode> GetReadOnlyPropertyAsVirtualRelationsList(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetReadOnlyPropertyAsVirtualRelationsList(logger, propertyName, localCodeExecutionContext, DefaultOptions);
        }

        public List<LogicalQueryNode> GetReadOnlyPropertyAsVirtualRelationsList(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(_codeExecutorComponent == null)
            {
                return null;
            }

            var readonlyPropertiesList = GetReadOnlyPropertiesList(logger, propertyName, localCodeExecutionContext, options);

#if DEBUG
            //Info("8803E3C2-E5A6-4C40-964E-B0F0A7D67BAA", $"readonlyPropertiesList?.Count = {readonlyPropertiesList?.Count}");
#endif

            if((readonlyPropertiesList?.Count ?? 0) == 0)
            {
                return null;
            }

            var result = new List<LogicalQueryNode>();

            foreach(var propInstance in readonlyPropertiesList)
            {
                var propertyValue = _codeExecutorComponent.CallExecutableSync(logger, propInstance.GetMethodExecutable, null, localCodeExecutionContext, CallMode.Default);

#if DEBUG
                //Info("57E80E8A-8AF3-486D-A5E9-57B2AB6E8E2A", $"propertyValue = {propertyValue}");
#endif

                var relation = _standardCoreFactsBuilder.BuildPropertyVirtualRelationInstance(propertyName, propInstance.Instance.Name, propertyValue);

                result.Add(relation);
            }

#if DEBUG
            //Info("54AB3CB3-9719-40ED-95B0-3E0135534F62", $"result = {result.WriteListToToHumanizedString()}");
#endif

            return result;
        }

        public Value ResolveImplicitProperty(IMonitorLogger logger, StrongIdentifierValue propertyName, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveImplicitProperty(logger, propertyName, instance, localCodeExecutionContext, DefaultOptions);
        }

        public Value ResolveImplicitProperty(IMonitorLogger logger, StrongIdentifierValue propertyName, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var searchOptions = new LogicalSearchOptions();
            searchOptions.ResolveVirtualRelationsFromPropetyHook = false;
            searchOptions.QueryExpression = _standardCoreFactsBuilder.BuildImplicitPropertyQueryInstance(propertyName, instance.Name);
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;

#if DEBUG
            //Info("EFD16AF0-7849-4552-AE5C-C72650148AE0", $"searchOptions.QueryExpression = {searchOptions.QueryExpression.ToHumanizedString()}");
#endif

            var searchResult = _logicalSearchResolver.Run(Logger, searchOptions);

#if DEBUG
            //Info("2D009976-5734-4A07-83B7-4601D25F52FC", $"searchResult = {searchResult}");
#endif

            if (searchResult.IsSuccess)
            {
                var items = searchResult.Items;

                if (items.Count == 1)
                {
                    var foundLogicalQueryNode = items.SelectMany(p => p.ResultOfVarOfQueryToRelationList).Where(p => p.NameOfVar == _targetLogicalVarName).Single().FoundExpression;

                    return LogicalQueryNodeHelper.ToValue(foundLogicalQueryNode);
                }
                else
                {
                    throw new NotImplementedException("43AA7A7B-F43E-48BE-ACA0-FAF5C69CAD59");
                }
            }
            else
            {
                return NullValue.Instance;
            }
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<PropertyInstance>> GetRawPropertiesList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<PropertyInstance>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<PropertyInstance>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.PropertyStorage.GetPropertyDirectly(logger, name, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<PropertyInstance>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
