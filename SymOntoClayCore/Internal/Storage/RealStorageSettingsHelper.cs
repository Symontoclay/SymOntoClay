using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public static class RealStorageSettingsHelper
    {
        public static RealStorageSettings Create(IMainStorageContext context, IStorage parentStorage)
        {
            return Create(context, new List<IStorage>() { parentStorage });
        }

        public static RealStorageSettings Create(IMainStorageContext context, List<IStorage> parentStorages)
        {
            var result = new RealStorageSettings();
            result.MainStorageContext = context;

            result.ParentsStorages = (parentStorages?.ToList()) ?? new List<IStorage>();
            result.DefaultSettingsOfCodeEntity = parentStorages?.FirstOrDefault()?.DefaultSettingsOfCodeEntity;

            return result;
        }
    }
}
