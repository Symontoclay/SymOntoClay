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
            result.Logger = context.Logger;
            result.EntityDictionary = context.Dictionary;
            result.Compiler = context.Compiler;
            result.CommonNamesStorage = context.CommonNamesStorage;
            result.ParentsStorages = (parentStorages?.ToList()) ?? new List<IStorage>();

            return result;
        }
    }
}
