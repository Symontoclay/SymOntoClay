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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

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
            _standardCoreFactsBuilder = _context.StandardFactsBuilder;
        }

        private readonly object _lockObj = new object();
        private readonly IMainStorageContext _context;
        private readonly ILogicalStorage _publicFactsStorage;
        private readonly IInheritanceStorage _publicInheritanceStorage;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly ResolverOptions _resolverOptions;
        private IStandardCoreFactsBuilder _standardCoreFactsBuilder;
        private List<StrongIdentifierValue> _foundInheritanceKeysList = new List<StrongIdentifierValue>();
        private ILocalCodeExecutionContext _localCodeExecutionContext;
        private ILogicQueryParseAndCache _logicQueryParseAndCache;

        private StrongIdentifierValue _selfName;
        private string _selfNameForFacts;
        private Dictionary<string, string> _factsIdDict = new Dictionary<string, string>();
        private Dictionary<string, InheritanceItem> _inheritanceItemsDict = new Dictionary<string, InheritanceItem>();

        /// <inheritdoc/>
        public void ProcessChangeInheritance(IMonitorLogger logger, StrongIdentifierValue subName, StrongIdentifierValue superName)
        {
            lock(_lockObj)
            {
#if DEBUG

                if (subName.NameValue == "app")
                {
                    throw new NotImplementedException("75EDA0A0-AB44-4B4C-9CAC-428FBFFCDEA0");
                }
#endif

                if (_foundInheritanceKeysList.Any())
                {

                    if(subName != _selfName && !_foundInheritanceKeysList.Contains(subName))
                    {
                        Recalculate(logger);
                        return;
                    }

                    if (superName != _selfName && !_foundInheritanceKeysList.Contains(superName))
                    {
                        Recalculate(logger);
                        return;
                    }

                    return;
                }

                Recalculate(logger);
            }
        }

        private void Recalculate(IMonitorLogger logger)
        {
            if(_localCodeExecutionContext == null)
            {
                var commonNamesStorage = _context.CommonNamesStorage;

                _selfName = commonNamesStorage.SelfName;
                _selfNameForFacts = NameHelper.ShieldString(_selfName.NameValue);

                _logicQueryParseAndCache = _context.LogicQueryParseAndCache;

                _localCodeExecutionContext = new LocalCodeExecutionContext(true)
                {
                    Storage = _context.Storage.GlobalStorage,
                    Holder = commonNamesStorage.DefaultHolder
                };
            }

            var weightedInheritanceItemsList = _inheritanceResolver.GetWeightedInheritanceItems(logger, _selfName, _localCodeExecutionContext, _resolverOptions).Where(p => !p.OriginalItem?.IsSystemDefined ?? false);

            if (!weightedInheritanceItemsList.Any())
            {
                _foundInheritanceKeysList.Clear();
                return;
            }

            var inheritanceItemsDict = weightedInheritanceItemsList.Where(p => p.OriginalItem != null).ToDictionary(p => p.SuperName, p => p.OriginalItem);

            var idsList = weightedInheritanceItemsList.Select(p => p.SuperName).Distinct().ToList();

            var newIdsList = idsList.Except(_foundInheritanceKeysList);

            if(newIdsList.Any())
            {
                foreach(var id in newIdsList)
                {
                    var name = id.NameValue;

                    var initialAddedInheritanceItem = inheritanceItemsDict[id];

                    var fact = _standardCoreFactsBuilder.BuildDefaultInheritanceFactInstance(_selfNameForFacts, name);

                    _publicFactsStorage.Append(logger, fact);

                    _factsIdDict[name] = fact.Name.NameValue;

                    var addedInheritanceItem = new InheritanceItem();

                    addedInheritanceItem.SuperName = initialAddedInheritanceItem.SuperName;
                    addedInheritanceItem.SubName = _selfName;
                    addedInheritanceItem.Rank = new LogicalValue(1);

                    _inheritanceItemsDict[name] = addedInheritanceItem;

                    _publicInheritanceStorage.SetInheritance(logger, addedInheritanceItem);
                }
            }

            var oldIdList = _foundInheritanceKeysList.Except(idsList);

            if (oldIdList.Any())
            {
                foreach(var id in oldIdList)
                {
                    var name = id.NameValue;

                    _publicFactsStorage.RemoveById(logger, _factsIdDict[name]);
                    _factsIdDict.Remove(name);

                    _publicInheritanceStorage.RemoveInheritance(logger, _inheritanceItemsDict[name]);
                    _inheritanceItemsDict.Remove(name);
                }
            }

            _foundInheritanceKeysList = idsList;
        }
    }
}
