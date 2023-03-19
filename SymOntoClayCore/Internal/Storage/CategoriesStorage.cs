using SymOntoClay.CoreHelper.CollectionsHelpers;
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

            if(!settings.Categories.IsNullOrEmpty())
            {
                throw new NotImplementedException();
            }
        }

        private readonly RealStorage _storage;

        public IStorage Storage => _storage;

        public void AddCategory(string category)
        {
            throw new NotImplementedException();
        }

        public void AddCategories(List<string> categories)
        {
            throw new NotImplementedException();
        }

        public void RemoveCategory(string category)
        {
            throw new NotImplementedException();
        }

        public void RemoveCategories(List<string> categories)
        {
            throw new NotImplementedException();
        }

        public bool EnableCategories { get => _storage.Enabled; set => _storage.Enabled = value; }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.Dispose();

            base.OnDisposed();
        }
    }
}
