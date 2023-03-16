/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
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
            _publicInheritanceStorage = publicFactsStorage.InheritanceStorage;
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();
            _resolverOptions = new ResolverOptions() { AddTopType = false };
        }

        private readonly object _lockObj = new object();
        private readonly IMainStorageContext _context;
        private readonly ILogicalStorage _publicFactsStorage;
        private readonly IInheritanceStorage _publicInheritanceStorage;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly ResolverOptions _resolverOptions;
        private List<StrongIdentifierValue> _foundInheritanceKeysList = new List<StrongIdentifierValue>();
        private ILocalCodeExecutionContext _localCodeExecutionContext;
        private ILogicQueryParseAndCache _logicQueryParseAndCache;
        //private ulong _selfNameKey;
        private StrongIdentifierValue _selfName;
        private string _selfNameForFacts;
        private Dictionary<string, string> _factsIdDict = new Dictionary<string, string>();
        private Dictionary<string, InheritanceItem> _inheritanceItemsDict = new Dictionary<string, InheritanceItem>();

        /// <inheritdoc/>
        public void ProcessChangeInheritance(StrongIdentifierValue subName, StrongIdentifierValue superName)
        {
            lock(_lockObj)
            {
#if DEBUG
                //Log($"subName = {subName}");
                //Log($"superName = {superName}");

                if (subName.NameValue == "app")
                {
                    throw new NotImplementedException();
                }
                //Log("Next");
#endif

                if (_foundInheritanceKeysList.Any())
                {
                    //var subNameKey = subName.GetIndexed(_context).NameKey;
                    //var superNameKey = superName.GetIndexed(_context).NameKey;

#if DEBUG
                    //Log($"subNameKey = {subNameKey}");
                    //Log($"superNameKey = {superNameKey}");
#endif

                    if(subName != _selfName && !_foundInheritanceKeysList.Contains(subName))
                    {
                        Recalculate();
                        return;
                    }

                    if (superName != _selfName && !_foundInheritanceKeysList.Contains(superName))
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
            //Log($"_selfName.NameValue = {_selfName.NameValue}");
#endif

            if(_localCodeExecutionContext == null)
            {
                var commonNamesStorage = _context.CommonNamesStorage;

                _selfName = commonNamesStorage.SelfName;
                _selfNameForFacts = NameHelper.ShieldString(_selfName.NameValue);

                _logicQueryParseAndCache = _context.LogicQueryParseAndCache;

                _localCodeExecutionContext = new LocalCodeExecutionContext()
                {
                    Storage = _context.Storage.GlobalStorage,
                    Holder = commonNamesStorage.DefaultHolder
                };
            }

#if DEBUG
            //Log($"_selfName = {_selfName}");
            //Log($"_selfNameForFacts = {_selfNameForFacts}");
#endif

            var weightedInheritanceItemsList = _inheritanceResolver.GetWeightedInheritanceItems(_selfName, _localCodeExecutionContext, _resolverOptions).Where(p => !p.OriginalItem?.IsSystemDefined ?? false);

#if DEBUG
            //Log($"weightedInheritanceItemsList = {weightedInheritanceItemsList.Select(p => p.OriginalItem).WriteListToString()}");
            //Log($"weightedInheritanceItemsList = {weightedInheritanceItemsList.WriteListToString()}");
            //Log($"_context.Id = {_context.Id}");
#endif

#if DEBUG
            //Log($"weightedInheritanceItemsList (after) = {weightedInheritanceItemsList.Select(p => p.OriginalItem).WriteListToString()}");
            //Log($"_context.Id = {_context.Id}");
#endif

            if (!weightedInheritanceItemsList.Any())
            {
                _foundInheritanceKeysList.Clear();
                return;
            }

            var inheritanceItemsDict = weightedInheritanceItemsList.Where(p => p.OriginalItem != null).ToDictionary(p => p.SuperName, p => p.OriginalItem);

            var idsList = weightedInheritanceItemsList.Select(p => p.SuperName).Distinct().ToList();

#if DEBUG
            //Log($"idsList = {JsonConvert.SerializeObject(idsList.Select(p => p.NameValue))}");
#endif

            var newIdsList = idsList.Except(_foundInheritanceKeysList);

#if DEBUG
            //Log($"newIdsList = {JsonConvert.SerializeObject(newIdsList.Select(p => p.NameValue))}");
#endif

            if(newIdsList.Any())
            {
                foreach(var id in newIdsList)
                {
#if DEBUG
                    //Log($"id = {id}");
#endif

                    var name = id.NameValue;

#if DEBUG
                    //Log($"name = {name}");
#endif

                    var initialAddedInheritanceItem = inheritanceItemsDict[id];

#if DEBUG
                    //Log($"initialAddedInheritanceItem = {initialAddedInheritanceItem}");
#endif

                    var factStr = $"{{: >:{{ is({_selfNameForFacts}, {name}, 1) }} :}}";

#if DEBUG
                    //Log($"factStr = {factStr}");
#endif

                    var fact = _logicQueryParseAndCache.GetLogicRuleOrFact(factStr);

#if DEBUG
                    //Log($"fact = {fact}");
#endif

                    _publicFactsStorage.Append(fact);

                    _factsIdDict[name] = fact.Name.NameValue;

                    var addedInheritanceItem = new InheritanceItem();

                    addedInheritanceItem.SuperName = initialAddedInheritanceItem.SuperName;
                    addedInheritanceItem.SubName = _selfName;
                    addedInheritanceItem.Rank = new LogicalValue(1);

#if DEBUG
                    //Log($"addedInheritanceItem = {addedInheritanceItem}");
#endif

                    _inheritanceItemsDict[name] = addedInheritanceItem;

                    _publicInheritanceStorage.SetInheritance(addedInheritanceItem);
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
                    var name = id.NameValue;

                    _publicFactsStorage.RemoveById(_factsIdDict[name]);
                    _factsIdDict.Remove(name);

                    _publicInheritanceStorage.RemoveInheritance(_inheritanceItemsDict[name]);
                    _inheritanceItemsDict.Remove(name);
                }
            }

            _foundInheritanceKeysList = idsList;
        }
    }
}
