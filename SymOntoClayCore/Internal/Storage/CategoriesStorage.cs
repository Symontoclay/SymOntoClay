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

            _storage = new RealStorage(KindOfStorage.Categories, storageSettigs);
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

        public bool EnableCategories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
