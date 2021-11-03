/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing;
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

            _parentStorage = parentStorage;
            _kindGlobalOfStorage = kindGlobalOfStorage;
        }

        private readonly IMainStorageContext _context;
        private readonly IStandaloneStorage _parentStorage;
        private readonly KindOfStorage _kindGlobalOfStorage;
        private ILogicQueryParseAndCache _logicQueryParseAndCache;
        private IParser _parser;

        private RealStorage _globalStorage;
        private RealStorage _publicFactsStorage;
        private RealStorage _selfFactsStorage;
        private RealStorage _perceptedFactsStorage;
        private RealStorage _listenedFactsStorage;
        private ConsolidatedPublicFactsStorage _worldPublicFactsStorage;
        private InheritancePublicFactsReplicator _inheritancePublicFactsReplicator;

        private CheckDirtyOptions _checkDirtyOptions;

        /// <inheritdoc/>
        public IStorage GlobalStorage => _globalStorage;

        /// <inheritdoc/>
        public IStorage PublicFactsStorage => _publicFactsStorage;

        /// <inheritdoc/>
        public IStorage PerceptedFactsStorage => _perceptedFactsStorage;

        /// <inheritdoc/>
        public IStorage ListenedFactsStorage => _listenedFactsStorage;

        /// <inheritdoc/>
        public IStorage WorldPublicFactsStorage => _worldPublicFactsStorage;

        //private List<RealStorage> _storagesList;

        public void LoadFromSourceCode(IEngineContext engineContext = null)
        {
            _logicQueryParseAndCache = _context.LogicQueryParseAndCache;
            _parser = _context.Parser;

#if DEBUG
            //Log($"_logicQueryParseAndCache == null = {_logicQueryParseAndCache == null}");
#endif

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

                        var listenedFactsStorageSettings = new RealStorageSettings();
                        listenedFactsStorageSettings.MainStorageContext = _context;
                        listenedFactsStorageSettings.KindOfGC = KindOfGC.ByTimeOut;

                        _listenedFactsStorage = new RealStorage(KindOfStorage.PerceptedFacts, listenedFactsStorageSettings);

                        parentStoragesList.Add(_listenedFactsStorage);

                        _worldPublicFactsStorage = new ConsolidatedPublicFactsStorage(_context.Logger);
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

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = _globalStorage;

            _checkDirtyOptions = new CheckDirtyOptions();
            _checkDirtyOptions.LocalContext = localCodeExecutionContext;
            _checkDirtyOptions.EngineContext = engineContext;
            _checkDirtyOptions.ConvertWaypointValueFromSource = true;

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

        private RuleInstance ParseFact(string text)
        {
#if DEBUG
            //Log($"text = {text}");
#endif

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (!text.StartsWith("{:"))
            {
                text = $"{{: {text} :}}";
            }

#if DEBUG
            //Log($"text = {text}");
#endif

            return _logicQueryParseAndCache.GetLogicRuleOrFact(text);
        }

        /// <inheritdoc/>
        public string InsertPublicFact(string text)
        {
#if DEBUG
            //Log($"text = {text}");
#endif

            var fact = ParseFact(text);

#if DEBUG
            //Log($"fact = {fact}");
#endif

            if(fact == null)
            {
                return string.Empty;
            }

            _publicFactsStorage.LogicalStorage.Append(fact);

#if DEBUG
            //Log($"NEXT text = {text}");
#endif

            _selfFactsStorage.LogicalStorage.Append(fact);

#if DEBUG
            //Log($"NEXT (2) text = {text}");
#endif

            return fact.Name.NameValue;
        }

        /// <inheritdoc/>
        public void RemovePublicFact(string id)
        {
#if DEBUG
            //Log($"id = {id}");
#endif

            if(string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            _publicFactsStorage.LogicalStorage.RemoveById(id);

#if DEBUG
            //Log($"NEXT id = {id}");
#endif

            _selfFactsStorage.LogicalStorage.RemoveById(id);


#if DEBUG
            //Log($"NEXT 2 id = {id}");
#endif
        }

        /// <inheritdoc/>
        public string InsertFact(string text)
        {
#if DEBUG
            //Log($"text = {text}");
#endif

            var fact = ParseFact(text);

#if DEBUG
            //Log($"fact = {fact}");
#endif

            if (fact == null)
            {
                return string.Empty;
            }

            _globalStorage.LogicalStorage.Append(fact);

            return fact.Name.NameValue;
        }

        /// <inheritdoc/>
        public void RemoveFact(string id)
        {
            _globalStorage.LogicalStorage.RemoveById(id);
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
            //Log($"text = {text}");
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
            //Log($"text (after) = {text}");
#endif

            var fact = _logicQueryParseAndCache.GetLogicRuleOrFact(text);

#if DEBUG
            //Log($"fact = {fact}");
#endif

            _perceptedFactsStorage.LogicalStorage.Append(fact);

            return fact.Name.NameValue;
        }

        /// <inheritdoc/>
        public void RemovePerceptedFact(string id)
        {
#if DEBUG
            //Log($"id = {id}");
#endif

            _perceptedFactsStorage.LogicalStorage.RemoveById(id);
        }

        /// <inheritdoc/>
        public void InsertListenedFact(string text)
        {
#if DEBUG
            //Log($"text = {text}");
#endif

            var fact = _parser.ParseRuleInstance(text, false);

#if DEBUG
            //Log($"fact = {fact}");
            //Log($"fact = {DebugHelperForRuleInstance.ToString(fact)}");
#endif

            fact.CheckDirty(_checkDirtyOptions);

#if DEBUG
            //Log($"fact.Normalized = {fact.Normalized}");
            //Log($"fact.Normalized = {DebugHelperForRuleInstance.ToString(fact.Normalized)}");
#endif

            _listenedFactsStorage.LogicalStorage.Append(fact);
        }

        /// <inheritdoc/>
        public void AddPublicFactsStorageOfOtherGameComponent(IStorage storage)
        {
            _worldPublicFactsStorage.AddPublicFactsStorageOfOtherGameComponent(storage);
        }

        /// <inheritdoc/>
        public void RemovePublicFactsStorageOfOtherGameComponent(IStorage storage)
        {
            _worldPublicFactsStorage.RemovePublicFactsStorageOfOtherGameComponent(storage);
        }

        public void Die()
        {
            _globalStorage.Dispose();
            _selfFactsStorage.Dispose();
            _perceptedFactsStorage.Dispose();
            _listenedFactsStorage.Dispose();
            _worldPublicFactsStorage.Dispose();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _globalStorage.Dispose();
            _publicFactsStorage.Dispose();
            _selfFactsStorage.Dispose();
            _perceptedFactsStorage.Dispose();
            _listenedFactsStorage.Dispose();
            _worldPublicFactsStorage.Dispose();

            base.OnDisposed();
        }
    }
}
