using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage.InheritanceStoraging;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class CategoriesStorage: BaseComponent
    {
        public CategoriesStorage(IMainStorageContext context, CategoriesStorageSettings settings)
            : base(context.Logger)
        {
            var storageSettigs = new RealStorageSettings()
            {
                MainStorageContext = context
            };

            storageSettigs.Enabled = settings.EnableCategories;

            _storage = new RealStorage(KindOfStorage.Categories, storageSettigs);
            _inheritanceStorage = _storage.InheritanceStorage;

            if (!settings.Categories.IsNullOrEmpty())
            {
                NAddCategories(settings.Categories);
            }
        }

        private readonly object _lockObj = new object();
        private readonly RealStorage _storage;
        private readonly IInheritanceStorage _inheritanceStorage;
        private readonly List<string> _categoriesList = new List<string>();
        private readonly Dictionary<StrongIdentifierValue, InheritanceItem> _categoriesDict = new Dictionary<StrongIdentifierValue, InheritanceItem>();

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
            Log($"category = {category}");
#endif

            if(string.IsNullOrWhiteSpace(category))
            {
                return;
            }

            if(_categoriesList.Contains(category))
            {
                return;
            }

            //_inheritanceStorage

            throw new NotImplementedException();
        }

        private void NAddCategories(List<string> categories)
        {
#if DEBUG
            Log($"categories = {categories.WritePODListToString()}");
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

            if (_categoriesList.Contains(category))
            {
                return;
            }

            //_inheritanceStorage

            throw new NotImplementedException();
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
