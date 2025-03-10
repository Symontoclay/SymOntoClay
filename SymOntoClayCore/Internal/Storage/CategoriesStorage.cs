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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Storage
{
    public class CategoriesStorage: BaseComponent
    {
        public CategoriesStorage(IMainStorageContext context, CategoriesStorageSettings settings)
            : base(context.Logger)
        {
            _settings = settings;
            _selfName = context.SelfName;

            var storageSettigs = new RealStorageSettings()
            {
                MainStorageContext = context
            };

            storageSettigs.Enabled = settings.EnableCategories;
            storageSettigs.InheritancePublicFactsReplicator = settings.InheritancePublicFactsReplicator;

            _storage = new RealStorage(KindOfStorage.Categories, storageSettigs);
            _inheritanceStorage = _storage.InheritanceStorage;
        }

        private readonly CategoriesStorageSettings _settings;
        private readonly object _lockObj = new object();
        private readonly StrongIdentifierValue _selfName;
        private readonly RealStorage _storage;
        private readonly IInheritanceStorage _inheritanceStorage;
        private readonly List<string> _categoriesList = new List<string>();
        private readonly Dictionary<StrongIdentifierValue, InheritanceItem> _categoriesDict = new Dictionary<StrongIdentifierValue, InheritanceItem>();

        public void Init()
        {
            if (!_settings.Categories.IsNullOrEmpty())
            {
                NAddCategories(Logger, _settings.Categories);
            }
        }

        public IStorage Storage => _storage;

        public void AddCategory(IMonitorLogger logger, string category)
        {
            lock(_lockObj)
            {
                NAddCategory(logger, category);
            }
        }

        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_lockObj)
            {
                NAddCategories(logger, categories);
            }
        }

        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            lock (_lockObj)
            {
                NRemoveCategory(logger, category);
            }
        }

        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_lockObj)
            {
                NRemoveCategories(logger, categories);
            }
        }

        public bool EnableCategories { get => _storage.Enabled; set => _storage.Enabled = value; }

        private void NAddCategory(IMonitorLogger logger, string category)
        {
            if(string.IsNullOrWhiteSpace(category))
            {
                return;
            }

            if(_categoriesList.Contains(category))
            {
                return;
            }

            var categoryName = NameHelper.CreateName(category);

            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = false
            };

            inheritanceItem.SubType = _selfName;
            inheritanceItem.SuperType = categoryName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            _categoriesDict.Add(categoryName, inheritanceItem);

            _inheritanceStorage.SetInheritance(logger, inheritanceItem);
        }

        private void NAddCategories(IMonitorLogger logger, List<string> categories)
        {
            foreach(var category in categories)
            {
                NAddCategory(logger, category);
            }
        }

        private void NRemoveCategory(IMonitorLogger logger, string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return;
            }

            if (!_categoriesList.Contains(category))
            {
                return;
            }

            _categoriesList.Remove(category);

            var categoryName = NameHelper.CreateName(category);

            var inheritanceItem = _categoriesDict[categoryName];
            _categoriesDict.Remove(categoryName);

            _inheritanceStorage.RemoveInheritance(logger, inheritanceItem);
        }

        private void NRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            foreach (var category in categories)
            {
                NRemoveCategory(logger, category);
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.Dispose();

            _categoriesList.Clear();
            _categoriesDict.Clear();

            base.OnDisposed();
        }
    }
}
