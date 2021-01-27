using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class InheritancePublicFactsReplicator : BaseComponent, IInheritancePublicFactsReplicator
    {
        public InheritancePublicFactsReplicator(IMainStorageContext context, RealStorage publicFactsStorage)
            : base(context.Logger)
        {
            _context = context;
            _publicFactsStorage = publicFactsStorage.LogicalStorage;
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();
            _resolverOptions = new ResolverOptions() { AddTopType = false };
        }

        private readonly object _lockObj = new object();
        private readonly IMainStorageContext _context;
        private readonly ILogicalStorage _publicFactsStorage;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly ResolverOptions _resolverOptions;
        private List<ulong> _foundInheritanceKeysList = new List<ulong>();
        private LocalCodeExecutionContext _localCodeExecutionContext;
        private IEntityDictionary _dictionary;
        private ILogicQueryParseAndCache _logicQueryParseAndCache;
        private ulong _sefNameKey;
        private string _selfNameForFacts;
        private Dictionary<ulong, string> _factsIdDict = new Dictionary<ulong, string>();

        /// <inheritdoc/>
        public void ProcessChangeInheritance(StrongIdentifierValue subName, StrongIdentifierValue superName)
        {
            lock(_lockObj)
            {
#if DEBUG
                //Log($"subName = {subName}");
                //Log($"superName = {superName}");
#endif

                if (_foundInheritanceKeysList.Any())
                {
                    var subNameKey = subName.GetIndexed(_context).NameKey;
                    var superNameKey = superName.GetIndexed(_context).NameKey;

#if DEBUG
                    //Log($"subNameKey = {subNameKey}");
                    //Log($"superNameKey = {superNameKey}");
#endif

                    if(subNameKey != _sefNameKey && !_foundInheritanceKeysList.Contains(subNameKey))
                    {
                        Recalculate();
                        return;
                    }

                    if (superNameKey != _sefNameKey && !_foundInheritanceKeysList.Contains(superNameKey))
                    {
                        Recalculate();
                        return;
                    }

                    return;
                }

                Recalculate();
            }
        }

        private void Recalculate()
        {
#if DEBUG
            //Log($"_context.Id = {_context.Id}");
            //Log($"_context.CommonNamesStorage.SelfName = {_context.CommonNamesStorage.SelfName}");
#endif

            if(_localCodeExecutionContext == null)
            {
                _sefNameKey = _context.CommonNamesStorage.IndexedSelfName.NameKey;
                _selfNameForFacts = _context.CommonNamesStorage.SelfName.NameValue;
                _dictionary = _context.Dictionary;
                _logicQueryParseAndCache = _context.LogicQueryParseAndCache;

                _localCodeExecutionContext = new LocalCodeExecutionContext()
                {
                    Storage = _context.Storage.GlobalStorage
                };
            }

#if DEBUG
            //Log($"_sefNameKey = {_sefNameKey}");
            //Log($"_selfNameForFacts = {_selfNameForFacts}");
#endif

            var weightedInheritanceItemsList = _inheritanceResolver.GetWeightedInheritanceItems(_sefNameKey, _localCodeExecutionContext, _resolverOptions);

#if DEBUG
            //Log($"weightedInheritanceItemsList = {weightedInheritanceItemsList.WriteListToString()}");
#endif

            if(!weightedInheritanceItemsList.Any())
            {
                _foundInheritanceKeysList.Clear();
                return;
            }

            var idsList = weightedInheritanceItemsList.Select(p => p.SuperNameKey).Distinct().ToList();

#if DEBUG
            //Log($"idsList = {JsonConvert.SerializeObject(idsList)}");
#endif

            var newIdsList = idsList.Except(_foundInheritanceKeysList);

#if DEBUG
            //Log($"newIdsList = {JsonConvert.SerializeObject(newIdsList)}");
#endif

            if(newIdsList.Any())
            {
                foreach(var id in newIdsList)
                {
#if DEBUG
                    //Log($"id = {id}");
#endif

                    var name = _dictionary.GetName(id);

#if DEBUG
                    //Log($"name = {name}");
#endif

                    var factStr = $"{{: >:{{ {name}({_selfNameForFacts}) }} :}}";

#if DEBUG
                    Log($"factStr = {factStr}");
#endif

                    var fact = _logicQueryParseAndCache.GetLogicRuleOrFact(factStr);

#if DEBUG
                    //Log($"fact = {fact}");
#endif

                    _publicFactsStorage.Append(fact);

                    _factsIdDict[id] = fact.Name.NameValue;
                }
            }

            var oldIdList = _foundInheritanceKeysList.Except(idsList);

#if DEBUG
            //Log($"oldIdList = {JsonConvert.SerializeObject(oldIdList)}");
#endif

            if (oldIdList.Any())
            {
                foreach(var id in oldIdList)
                {
                    _publicFactsStorage.RemoveById(_factsIdDict[id]);
                    _factsIdDict.Remove(id);
                }
            }

            _foundInheritanceKeysList = idsList;
        }
    }
}
