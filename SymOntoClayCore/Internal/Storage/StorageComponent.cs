/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class StorageComponent: BaseComponent, IStorageComponent
    {
        public StorageComponent(IMainStorageContext context, IStandaloneStorage parentStorage, KindOfStorage kindGlobalOfStorage)
            : base(context.Logger)
        {
            _context = context;
            _logicQueryParseAndCache = context.LogicQueryParseAndCache;

#if DEBUG
            //Log($"_logicQueryParseAndCache == null = {_logicQueryParseAndCache == null}");
#endif

            _parentStorage = parentStorage;
            _kindGlobalOfStorage = kindGlobalOfStorage;
        }

        private readonly IMainStorageContext _context;
        private readonly IStandaloneStorage _parentStorage;
        private readonly KindOfStorage _kindGlobalOfStorage;
        private readonly ILogicQueryParseAndCache _logicQueryParseAndCache;

        private RealStorage _globalStorage;
        private RealStorage _publicFactsStorage;
        private RealStorage _selfFactsStorage;
        private RealStorage _perceptedFactsStorage;
        private InheritancePublicFactsReplicator _inheritancePublicFactsReplicator;

        /// <inheritdoc/>
        public IStorage GlobalStorage => _globalStorage;

        /// <inheritdoc/>
        public IStorage PublicFactsStorage => _publicFactsStorage;

        /// <inheritdoc/>
        public IStorage PerceptedFactsStorage => _perceptedFactsStorage;

        //private List<RealStorage> _storagesList;

        public void LoadFromSourceCode()
        {
            //_storagesList = new List<RealStorage>();

            var globalStorageSettings = new RealStorageSettings();

            var parentStoragesList = new List<IStorage>();

            switch (_kindGlobalOfStorage)
            {
                case KindOfStorage.Global:
                    {
                        var publicFactsStorageSettings = new RealStorageSettings();
                        publicFactsStorageSettings.MainStorageContext = _context;

                        _publicFactsStorage = new RealStorage(KindOfStorage.PublicFacts, publicFactsStorageSettings);

                        _inheritancePublicFactsReplicator = new InheritancePublicFactsReplicator(_context, _publicFactsStorage);
                        globalStorageSettings.InheritancePublicFactsReplicator = _inheritancePublicFactsReplicator;

                        //_storagesList.Add(_publicFactsStorage);

                        _selfFactsStorage = new RealStorage(KindOfStorage.PublicFacts, publicFactsStorageSettings);

                        parentStoragesList.Add(_selfFactsStorage);

                        var perceptedFactsStorageSettings = new RealStorageSettings();
                        perceptedFactsStorageSettings.MainStorageContext = _context;

                        _perceptedFactsStorage = new RealStorage(KindOfStorage.PerceptedFacts, perceptedFactsStorageSettings);

                        //_storagesList.Add(_perceptedFactsStorage);

                        parentStoragesList.Add(_perceptedFactsStorage);
                    }
                    break;

                case KindOfStorage.Host:
                    {
                        var publicFactsStorageSettings = new RealStorageSettings();
                        publicFactsStorageSettings.MainStorageContext = _context;

                        _publicFactsStorage = new RealStorage(KindOfStorage.PublicFacts, publicFactsStorageSettings);

                        _inheritancePublicFactsReplicator = new InheritancePublicFactsReplicator(_context, _publicFactsStorage);
                        globalStorageSettings.InheritancePublicFactsReplicator = _inheritancePublicFactsReplicator;

                        //_storagesList.Add(_publicFactsStorage);

                        _selfFactsStorage = new RealStorage(KindOfStorage.PublicFacts, publicFactsStorageSettings);

                        parentStoragesList.Add(_selfFactsStorage);
                    }
                    break;
            }
         
            globalStorageSettings.MainStorageContext = _context;

            if (_parentStorage != null && _parentStorage.Storage != null)
            {
                parentStoragesList.Add(_parentStorage.Storage);
            }

            if(parentStoragesList.Any())
            {
                globalStorageSettings.ParentsStorages = parentStoragesList;
            }

            switch(_kindGlobalOfStorage)
            {
                case KindOfStorage.Global:
                    _globalStorage = new GlobalStorage(globalStorageSettings);
                    break;

                case KindOfStorage.World:
                    _globalStorage = new WorldStorage(globalStorageSettings);
                    break;

                case KindOfStorage.Host:
                    _globalStorage = new HostStorage(globalStorageSettings);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_kindGlobalOfStorage), _kindGlobalOfStorage, null);
            }
            
            //_storagesList.Add(_globalStorage);

            _globalStorage.DefaultSettingsOfCodeEntity = CreateDefaultSettingsOfCodeEntity();

#if IMAGINE_WORKING
            //Log("Do");
#else
                throw new NotImplementedException();
#endif
        }

        private DefaultSettingsOfCodeEntity CreateDefaultSettingsOfCodeEntity()
        {
            var result = new DefaultSettingsOfCodeEntity();

            return result;
        }

        /// <inheritdoc/>
        public string InsertPublicFact(string text)
        {
#if DEBUG
            //Log($"text = {text}");
#endif

            if(string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if(!text.StartsWith("{:"))
            {
                text = $"{{: {text} :}}";
            }

#if DEBUG
            //Log($"text = {text}");
#endif

            var fact = _logicQueryParseAndCache.GetLogicRuleOrFact(text);

#if DEBUG
            //Log($"fact = {fact}");
#endif

            _publicFactsStorage.LogicalStorage.Append(fact);
            _selfFactsStorage.LogicalStorage.Append(fact);

            return fact.Name.NameValue;
        }

        /// <inheritdoc/>
        public void RemovePublicFact(string id)
        {
#if DEBUG
            //Log($"id = {id}");
#endif

            _publicFactsStorage.LogicalStorage.RemoveById(id);
            _selfFactsStorage.LogicalStorage.RemoveById(id);
        }

        public void AddVisibleStorage(IStorage storage)
        {
            _globalStorage.AddParentStorage(storage);
        }

        public void RemoveVisibleStorage(IStorage storage)
        {
            _globalStorage.RemoveParentStorage(storage);
        }

        /// <inheritdoc/>
        public string InsertPerceptedFact(string text)
        {
#if DEBUG
            Log($"text = {text}");
#endif

            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (!text.StartsWith("{:"))
            {
                text = $"{{: {text} :}}";
            }

#if DEBUG
            Log($"text = {text}");
#endif

            var fact = _logicQueryParseAndCache.GetLogicRuleOrFact(text);

#if DEBUG
            Log($"fact = {fact}");
#endif

            _perceptedFactsStorage.LogicalStorage.Append(fact);

            return fact.Name.NameValue;
        }

        /// <inheritdoc/>
        public void RemovePerceptedFact(string id)
        {
            _perceptedFactsStorage.LogicalStorage.RemoveById(id);
        }
    }
}
