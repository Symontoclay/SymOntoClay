using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Storage.InheritanceStoraging;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

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

#if DEBUG
            //Log($"storageSettigs = {storageSettigs}");
#endif

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
                NAddCategories(_settings.Categories);
            }
        }

        public IStorage Storage => _storage;

        public void AddCategory(string category)
        {
            lock(_lockObj)
            {
                NAddCategory(category);
            }
        }

        public void AddCategories(List<string> categories)
        {
            lock (_lockObj)
            {
                NAddCategories(categories);
            }
        }

        public void RemoveCategory(string category)
        {
            lock (_lockObj)
            {
                NRemoveCategory(category);
            }
        }

        public void RemoveCategories(List<string> categories)
        {
            lock (_lockObj)
            {
                NRemoveCategories(categories);
            }
        }

        public bool EnableCategories { get => _storage.Enabled; set => _storage.Enabled = value; }

        private void NAddCategory(string category)
        {
#if DEBUG
            //Log($"category = {category}");
#endif

            if(string.IsNullOrWhiteSpace(category))
            {
                return;
            }

            if(_categoriesList.Contains(category))
            {
                return;
            }

            var categoryName = NameHelper.CreateName(category);

#if DEBUG
            //Log($"categoryName = {categoryName}");
            //Log($"_selfName = {_selfName}");
#endif

            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = false
            };

            inheritanceItem.SubName = _selfName;
            inheritanceItem.SuperName = categoryName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

            _categoriesDict.Add(categoryName, inheritanceItem);

            _inheritanceStorage.SetInheritance(inheritanceItem);
        }

        private void NAddCategories(List<string> categories)
        {
#if DEBUG
            //Log($"categories = {categories.WritePODListToString()}");
#endif

            foreach(var category in categories)
            {
                NAddCategory(category);
            }
        }

        private void NRemoveCategory(string category)
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

            _inheritanceStorage.RemoveInheritance(inheritanceItem);
        }

        private void NRemoveCategories(List<string> categories)
        {
            foreach (var category in categories)
            {
                NRemoveCategory(category);
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
