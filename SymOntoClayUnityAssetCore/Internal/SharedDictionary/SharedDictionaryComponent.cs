using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.SharedDictionary
{
    public class SharedDictionaryComponent: BaseWorldCoreComponent
    {
        public SharedDictionaryComponent(IWorldCoreContext coreContext)
            : base(coreContext)
        {
            var sharedDictionarySettings = new SharedDictionarySettings();
            sharedDictionarySettings.Logger = Logger;

            Log($"sharedDictionarySettings = {sharedDictionarySettings}");

            _sharedDictionary = new SymOntoClay.Core.SharedDictionary(sharedDictionarySettings);
        }

        private readonly SymOntoClay.Core.SharedDictionary _sharedDictionary;

        public SymOntoClay.Core.IEntityDictionary Dictionary => _sharedDictionary;

        public void LoadFromSourceCode()
        {
            _sharedDictionary.LoadFromSourceCode();
        }
    }
}
