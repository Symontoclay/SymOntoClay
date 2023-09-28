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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class StorageComponent: BaseComponent, IStorageComponent
    {
        public StorageComponent(IMainStorageContext context, IStandaloneStorage parentStorage, KindOfStorage kindGlobalOfStorage, StorageComponentSettings settings)
            : base(context.Logger)
        {
            _settings = settings;

            _context = context;

            _parentStorage = parentStorage;
            _kindGlobalOfStorage = kindGlobalOfStorage;
        }

        private readonly StorageComponentSettings _settings;
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
        private ConsolidatedPublicFactsStorage _visibleFactsStorage;
        private ConsolidatedPublicFactsStorage _worldPublicFactsStorage;
        private ConsolidatedPublicFactsStorage _additionalPublicFactsStorage;
        private InheritancePublicFactsReplicator _inheritancePublicFactsReplicator;
        private CategoriesStorage _categoriesStorage;

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
        public ConsolidatedPublicFactsStorage WorldPublicFactsStorage => _worldPublicFactsStorage;

        public void LoadFromSourceCode(IEngineContext engineContext = null)
        {
            _logicQueryParseAndCache = _context.LogicQueryParseAndCache;
            _parser = _context.Parser;

            var globalStorageSettings = new RealStorageSettings();

            var parentStoragesList = new List<IStorage>();

            switch (_kindGlobalOfStorage)
            {
                case KindOfStorage.World:
                    {
                        _worldPublicFactsStorage = new ConsolidatedPublicFactsStorage(_context.Logger, KindOfStorage.WorldPublicFacts);

                        parentStoragesList.Add(_worldPublicFactsStorage);
                    }
                    break;

                case KindOfStorage.Global:
                    {
                        var publicFactsStorageSettings = new RealStorageSettings();
                        publicFactsStorageSettings.MainStorageContext = _context;

                        _publicFactsStorage = new RealStorage(KindOfStorage.PublicFacts, publicFactsStorageSettings);

                        _inheritancePublicFactsReplicator = new InheritancePublicFactsReplicator(_context, _publicFactsStorage);
                        globalStorageSettings.InheritancePublicFactsReplicator = _inheritancePublicFactsReplicator;


                        _selfFactsStorage = new RealStorage(KindOfStorage.PublicFacts, publicFactsStorageSettings);

                        parentStoragesList.Add(_selfFactsStorage);

                        var perceptedFactsStorageSettings = new RealStorageSettings();
                        perceptedFactsStorageSettings.MainStorageContext = _context;

                        _perceptedFactsStorage = new RealStorage(KindOfStorage.PerceptedFacts, perceptedFactsStorageSettings);


                        parentStoragesList.Add(_perceptedFactsStorage);

                        var listenedFactsStorageSettings = new RealStorageSettings();
                        listenedFactsStorageSettings.MainStorageContext = _context;
                        listenedFactsStorageSettings.KindOfGC = KindOfGC.ByTimeOut;
                        listenedFactsStorageSettings.EnableOnAddingFactEvent = true;

                        _listenedFactsStorage = new RealStorage(KindOfStorage.PerceptedFacts, listenedFactsStorageSettings);

                        parentStoragesList.Add(_listenedFactsStorage);

                        var visibleFactsStorageSettings = new ConsolidatedPublicFactsStorageSettings();
                        visibleFactsStorageSettings.MainStorageContext = _context;
                        visibleFactsStorageSettings.EnableOnAddingFactEvent = KindOfOnAddingFactEvent.Isolated;

                        _visibleFactsStorage = new ConsolidatedPublicFactsStorage(_context.Logger, KindOfStorage.VisiblePublicFacts, visibleFactsStorageSettings);
                        parentStoragesList.Add(_visibleFactsStorage);

                        var categoriesStorageSettings = new CategoriesStorageSettings()
                        {
                            Categories = _settings.Categories,
                            EnableCategories = _settings.EnableCategories,
                            InheritancePublicFactsReplicator = _inheritancePublicFactsReplicator
                        };

                        _categoriesStorage = new CategoriesStorage(_context, categoriesStorageSettings);

                        parentStoragesList.Add(_categoriesStorage.Storage);
                    }
                    break;

                case KindOfStorage.Host:
                    {
                        var publicFactsStorageSettings = new RealStorageSettings();
                        publicFactsStorageSettings.MainStorageContext = _context;

                        _publicFactsStorage = new RealStorage(KindOfStorage.PublicFacts, publicFactsStorageSettings);

                        _inheritancePublicFactsReplicator = new InheritancePublicFactsReplicator(_context, _publicFactsStorage);
                        globalStorageSettings.InheritancePublicFactsReplicator = _inheritancePublicFactsReplicator;


                        _selfFactsStorage = new RealStorage(KindOfStorage.PublicFacts, publicFactsStorageSettings);

                        parentStoragesList.Add(_selfFactsStorage);

                        var categoriesStorageSettings = new CategoriesStorageSettings()
                        {
                            Categories = _settings.Categories,
                            EnableCategories = _settings.EnableCategories,
                            InheritancePublicFactsReplicator = _inheritancePublicFactsReplicator
                        };

                        _categoriesStorage = new CategoriesStorage(_context, categoriesStorageSettings);

                        parentStoragesList.Add(_categoriesStorage.Storage);
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
            

            _globalStorage.DefaultSettingsOfCodeEntity = CreateDefaultSettingsOfCodeEntity();

            

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = _globalStorage;

            _checkDirtyOptions = new CheckDirtyOptions();
            _checkDirtyOptions.LocalContext = localCodeExecutionContext;
            _checkDirtyOptions.EngineContext = engineContext;
            _checkDirtyOptions.ConvertWaypointValueFromSource = true;

            _categoriesStorage?.Init();

        }

        private DefaultSettingsOfCodeEntity CreateDefaultSettingsOfCodeEntity()
        {
            var result = new DefaultSettingsOfCodeEntity();

            return result;
        }

        private RuleInstance ParseFact(IMonitorLogger logger, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (!text.StartsWith("{:"))
            {
                text = $"{{: {text} :}}";
            }

            return _logicQueryParseAndCache.GetLogicRuleOrFact(logger, text);
        }

        /// <inheritdoc/>
        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            if(logger == null)
            {
                logger = Logger;
            }

            var fact = ParseFact(logger, text);

            return InsertPublicFact(logger, fact);
        }

        /// <inheritdoc/>
        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            if (fact == null)
            {
                return string.Empty;
            }

            if (logger == null)
            {
                logger = Logger;
            }

            var checkDirtyOptions = new CheckDirtyOptions()
            { 
                ReplaceConcepts = new Dictionary<StrongIdentifierValue, StrongIdentifierValue>() 
            };

            checkDirtyOptions.ReplaceConcepts[NameHelper.CreateName("I")] = NameHelper.CreateName(_context.Id);

            fact.CalculateLongHashCodes(checkDirtyOptions);

            _publicFactsStorage.LogicalStorage.Append(logger, fact);

            _selfFactsStorage.LogicalStorage.Append(logger, fact);

            return fact.Name.NameValue;
        }

        /// <inheritdoc/>
        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            if (logger == null)
            {
                logger = Logger;
            }

            _publicFactsStorage.LogicalStorage.RemoveById(logger, id);

            _selfFactsStorage.LogicalStorage.RemoveById(logger, id);
        }

        /// <inheritdoc/>
        public string InsertFact(IMonitorLogger logger, string text)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            var fact = ParseFact(logger, text);

            if (fact == null)
            {
                return string.Empty;
            }

            _globalStorage.LogicalStorage.Append(logger, fact);

            return fact.Name.NameValue;
        }

        /// <inheritdoc/>
        public void RemoveFact(IMonitorLogger logger, string id)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            _globalStorage.LogicalStorage.RemoveById(logger, id);
        }

        public void AddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            _visibleFactsStorage.AddConsolidatedStorage(logger, storage);
        }

        public void RemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            _visibleFactsStorage.RemoveConsolidatedStorage(logger, storage);
        }

        /// <inheritdoc/>
        public string InsertPerceptedFact(IMonitorLogger logger, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (logger == null)
            {
                logger = Logger;
            }

            if (!text.StartsWith("{:"))
            {
                text = $"{{: {text} :}}";
            }

            var fact = _logicQueryParseAndCache.GetLogicRuleOrFact(logger, text);

            _perceptedFactsStorage.LogicalStorage.Append(logger, fact);

            return fact.Name.NameValue;
        }

        /// <inheritdoc/>
        public void RemovePerceptedFact(IMonitorLogger logger, string id)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            _perceptedFactsStorage.LogicalStorage.RemoveById(logger, id);
        }

        /// <inheritdoc/>
        public void InsertListenedFact(IMonitorLogger logger, string text)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            var fact = _parser.ParseRuleInstance(text, false);

            InsertListenedFact(logger, fact);
        }

        /// <inheritdoc/>
        public void InsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            fact.CheckDirty(_checkDirtyOptions);

            _listenedFactsStorage.LogicalStorage.Append(logger, fact);
        }

        /// <inheritdoc/>
        public void AddCategory(IMonitorLogger logger, string category)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            _categoriesStorage.AddCategory(logger, category);
        }

        /// <inheritdoc/>
        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            _categoriesStorage.AddCategories(logger, categories);
        }

        /// <inheritdoc/>
        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            _categoriesStorage.RemoveCategory(logger, category);
        }

        /// <inheritdoc/>
        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            _categoriesStorage.RemoveCategories(logger, categories);
        }

        /// <inheritdoc/>
        public bool EnableCategories { get => _categoriesStorage.EnableCategories; set => _categoriesStorage.EnableCategories = value; }

        public void Die()
        {
            _globalStorage.Dispose();
            _selfFactsStorage.Dispose();
            _perceptedFactsStorage.Dispose();
            _listenedFactsStorage.Dispose();
            _visibleFactsStorage.Dispose();
            _worldPublicFactsStorage?.Dispose();
            _categoriesStorage.Dispose();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _globalStorage.Dispose();
            _publicFactsStorage.Dispose();
            _selfFactsStorage.Dispose();
            _perceptedFactsStorage.Dispose();
            _listenedFactsStorage.Dispose();
            _visibleFactsStorage.Dispose();
            _worldPublicFactsStorage?.Dispose();
            _categoriesStorage.Dispose();

            base.OnDisposed();
        }
    }
}
