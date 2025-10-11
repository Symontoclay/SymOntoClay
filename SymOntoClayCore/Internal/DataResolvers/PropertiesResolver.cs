using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
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

        public ValueCallResult SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return SetPropertyValue(logger, propertyName, value, localCodeExecutionContext, callMode, DefaultOptions);
        }

        public ValueCallResult SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode, ResolverOptions options)
        {
#if DEBUG
            Info("BAEC7F05-B0F0-42C2-BFEB-43B5AC51B208", $"propertyName = {propertyName}");
            Info("54CD61AD-5D26-4C26-9751-AE243769B0A4", $"value = {value}");
            Info("E981235C-808D-4F6B-AF59-97213B85599D", $"callMode = {callMode}");
#endif

            var property = Resolve(logger, propertyName, localCodeExecutionContext, callMode, options);

#if DEBUG
            Info("1B9FF0A5-D834-409F-A555-4E447E8C71DE", $"property = {property}");
#endif

            switch(callMode)
            {
                case CallMode.PreConstructor:
                    if (property == null)
                    {
                        property = CreateAndSaveGlobalProperty(logger, propertyName, localCodeExecutionContext);
                        return property.SetValue(logger, value);                        
                    }
                    else
                    {
                        return property.SetValue(logger, value);
                    }

                case CallMode.Default:
                    if (property == null)
                    {
                        property = CreateAndSaveGlobalProperty(logger, propertyName, localCodeExecutionContext);
                        return property.SetValue(logger, value);
                    }
                    else
                    {
                        var kindOfProperty = property.KindOfProperty;

                        switch (kindOfProperty)
                        {
                            case KindOfProperty.Auto:
                                return property.SetValue(logger, value);

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfProperty), kindOfProperty, null);
                        }
                    }
                case CallMode.HtnPlanner:
                    if(property == null)
                    {
                        property = CreateAndSaveHtnPlannerProperty(logger, propertyName, localCodeExecutionContext);
                        return property.SetValue(logger, value);
                    }
                    else
                    {
                        if(property.IsReal)
                        {
                            property = CreateAndSaveHtnPlannerProperty(logger, propertyName, localCodeExecutionContext);
                            return property.SetValue(logger, value);
                        }

                        var kindOfProperty = property.KindOfProperty;

                        switch (kindOfProperty)
                        {
                            case KindOfProperty.Auto:
                                return property.SetValue(logger, value);

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfProperty), kindOfProperty, null);
                        }
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(callMode), callMode, null);
            }
        }

        private PropertyInstance CreateAndSaveGlobalProperty(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Info("A7608C40-FC0C-4454-A99A-2F14FBB8375C", $"propertyName = {propertyName}");
#endif

            var globalInstance = _context.InstancesStorage.MainEntity;
            var globalPropertyStorage = _context.Storage.GlobalStorage.PropertyStorage;

            var property = new Property()
            {
                TypeOfAccess = TypeOfAccess.Public,
                Name = propertyName,
                TypesList = new List<StrongIdentifierValue>() { _context.CommonNamesStorage.AnyTypeName },
                Holder = globalInstance.Name,
                KindOfProperty = KindOfProperty.Auto
            };

            var propertyInstance = new PropertyInstance(property, globalInstance, true, globalInstance.EngineContext);

            globalPropertyStorage.Append(Logger, propertyInstance);

            return propertyInstance;
        }

        private PropertyInstance CreateAndSaveHtnPlannerProperty(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Info("95A2CC46-C7D1-43FB-8EC7-D156EB53392A", $"propertyName = {propertyName}");
            Info("36E75738-8B9F-4B0A-B123-76209B0C23A2", $"localCodeExecutionContext.GetType().Name = {localCodeExecutionContext.GetType().Name}");
            localCodeExecutionContext.DbgPrintContextChain(logger, "F8FBE6EB-75C1-4D22-A30E-080B5A76D759");
#endif

            var nonRealLocalCodeExecutionContext = localCodeExecutionContext.GetFirstNonRealItemFromChain();

#if DEBUG
            Info("3C1AF218-7A67-419B-88FC-D59254EA3691", $"nonRealLocalCodeExecutionContext = {nonRealLocalCodeExecutionContext}");
#endif

            if(nonRealLocalCodeExecutionContext == null)
            {
                throw new NullReferenceException($"642A0B5A-5491-4408-AE1F-D41FB350AAC3: {nameof(nonRealLocalCodeExecutionContext)}");
            }

            var globalInstance = _context.InstancesStorage.MainEntity;
            var propertyStorage = nonRealLocalCodeExecutionContext.Storage.PropertyStorage;

#if DEBUG
            Info("F6F7DD0B-5531-4A90-97D2-0C8EBA102D7B", $"propertyStorage.GetType().Name = {propertyStorage.GetType().Name}");
#endif

            var property = new Property()
            {
                TypeOfAccess = TypeOfAccess.Public,
                Name = propertyName,
                TypesList = new List<StrongIdentifierValue>() { _context.CommonNamesStorage.AnyTypeName },
                Holder = globalInstance.Name,
                KindOfProperty = KindOfProperty.Auto
            };

            var propertyInstance = new PropertyInstance(property, globalInstance, true, globalInstance.EngineContext);

            propertyStorage.Append(Logger, propertyInstance);

            return propertyInstance;
        }

        public ValueCallResult GetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return GetPropertyValue(logger, propertyName, instance, localCodeExecutionContext, callMode, DefaultOptions);
        }

        public ValueCallResult GetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode, ResolverOptions options)
        {
#if DEBUG
            //Info("AF338CE5-9FF4-455A-91E2-98F429B1EA30", $"propertyName = {propertyName}");
#endif

            var property = Resolve(logger, propertyName, localCodeExecutionContext, callMode, options);

#if DEBUG
            //Info("D7C11381-C110-411C-A2F4-3A704359E2F8", $"property?.KindOfProperty = {property?.KindOfProperty}");
#endif

            if (property == null)
            {
                var value = ResolveImplicitProperty(logger, propertyName, instance, localCodeExecutionContext, options);

#if DEBUG
                //Info("C383590A-6041-4175-8CF4-F478E552C5E6", $"value = {value}");
#endif

                if (value == null)
                {
                    return new ValueCallResult(NullValue.Instance);
                }
                else
                {
                    return new ValueCallResult(value);
                }
            }

            return ConvertPropertyInstanceToCallResult(property);

            //throw new NotImplementedException("3F5085DB-DF7C-4864-8E92-03EFB497FC6A");
        }

        public ValueCallResult ConvertPropertyInstanceToCallResult(PropertyInstance property)
        {
            var kindOfProperty = property.KindOfProperty;

            switch (kindOfProperty)
            {
                case KindOfProperty.Auto:
                    return new ValueCallResult(property.GetValue());

                case KindOfProperty.Readonly:
                case KindOfProperty.GetSet:
                    {
                        var executable = property.GetMethodExecutable;

                        return new ValueCallResult()
                        {
                            KindOfResult = KindOfCallResult.NeedExecuteGetProperty,
                            Executable = executable
                        };
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfProperty), kindOfProperty, null);
            }
        }

        public PropertyInstance Resolve(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return Resolve(logger, propertyName, localCodeExecutionContext, callMode, DefaultOptions);
        }

        public PropertyInstance Resolve(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode, ResolverOptions options)
        {
#if DEBUG
            Info("163564B6-A6A5-424B-9974-DB506035843F", $"propertyName = {propertyName}");
            Info("0E85B012-8A78-4778-9E82-10B71B63B8E5", $"callMode = {callMode}");
            //localCodeExecutionContext.DbgPrintContextChain(logger, "DD93963A-03FD-417C-A8F9-CE904652F289");
#endif

            var result = EnumerateLocalCodeExecutionContext<PropertyInstance>(logger, localCodeExecutionContext, (ctx) => {
                return NResolve(logger, propertyName, ctx, callMode, options);
            });

            return result;
        }

        public PropertyInstance NResolve(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode, ResolverOptions options)
        {
#if DEBUG
            Info("F003D1F2-A299-411F-932C-7C226A0D13CC", $"propertyName = {propertyName}");
            Info("4B199295-FC1B-4EF2-A3EE-97841F44B2EE", $"callMode = {callMode}");
#endif

            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.Property);

#if DEBUG
            Info("C6A2F71F-4B86-4418-B806-82114687C6B4", $"storagesList.Count = {storagesList.Count}");
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

            filteredList = OrderAndDistinctByInheritance(logger, filteredList, options);

            if (callMode == CallMode.HtnPlanner)
            {
                filteredList = filteredList.OrderBy(p => p.ResultItem.IsReal).ToList();
            }

            return filteredList.FirstOrDefault()?.ResultItem;
        }

        public List<PropertyInstance> GetReadOnlyPropertiesList(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return GetReadOnlyPropertiesList(logger, propertyName, localCodeExecutionContext, callMode, DefaultOptions);
        }

        public List<PropertyInstance> GetReadOnlyPropertiesList(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode, ResolverOptions options)
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

        public List<LogicalQueryNode> GetReadOnlyPropertyAsVirtualRelationsList(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return GetReadOnlyPropertyAsVirtualRelationsList(logger, propertyName, localCodeExecutionContext, callMode, DefaultOptions);
        }

        public List<LogicalQueryNode> GetReadOnlyPropertyAsVirtualRelationsList(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode, ResolverOptions options)
        {
            if(_codeExecutorComponent == null)
            {
                return null;
            }

            var readonlyPropertiesList = GetReadOnlyPropertiesList(logger, propertyName, localCodeExecutionContext, callMode, options);

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
#if DEBUG
            //Info("5DF95D7D-537A-415B-B47E-52F49FE0B470", $"instance == null = {instance == null}");
            //Info("401862BC-2E0C-49E4-B864-7669414EEF1D", $"instance.Name = {instance?.Name}");
            //localCodeExecutionContext.DbgPrintContextChain(logger, "26E87670-1296-4DBE-A0AC-1DB21666E4FF");
#endif

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

                var foundItems = items.SelectMany(p => p.ResultOfVarOfQueryToRelationList).Where(p => p.NameOfVar == _targetLogicalVarName).Select(p => p.FoundExpression).ToList();

#if DEBUG
                //Info("6ED61BD6-88F0-4146-9958-E2A685BE9012", $"foundItems = {foundItems.WriteListToString()}");
#endif

                if (foundItems.Count == 1)
                {
                    var foundLogicalQueryNode = foundItems.Single();

                    return LogicalQueryNodeHelper.ToValue(foundLogicalQueryNode);
                }
                else
                {
                    var foundLogicalQueryNode = foundItems.Select(p => (p.RuleInstance?.TimeStamp ?? 0, p)).OrderByDescending(p => p.Item1).First().p;

#if DEBUG
                    //Info("6AF8610C-6CDA-424C-BC3C-06DC67220A62", $"foundLogicalQueryNode = {foundLogicalQueryNode}");
#endif

                    return LogicalQueryNodeHelper.ToValue(foundLogicalQueryNode);
                }
            }
            else
            {
                return null;
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
#if DEBUG
                //Info("FBC42265-F5E8-4275-B0DE-A25E1035BCBA", $"storageItem.Storage = {storageItem.Storage}");
#endif

                var itemsList = storageItem.Storage.PropertyStorage.GetPropertyDirectly(logger, name, weightedInheritanceItems);

#if DEBUG
                //Info("07EAFBC3-7345-4E18-9B85-75D207ADB26D", $"itemsList?.Count = {itemsList?.Count}");
#endif

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
