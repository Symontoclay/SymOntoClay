using SymOntoClay.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class QuickStorage: BaseComponent, IQuickStorage, ISerializableWithImageEngine
    {
        public QuickStorage(QuickStorageSettings settings)
            : base(settings.Logger)
        {
            _logicQueryParseAndCache = settings.LogicQueryParseAndCache;
        }

        private readonly ILogicQueryParseAndCache _logicQueryParseAndCache;

        /// <inheritdoc/>
        public IStorage Storage => throw new NotImplementedException();

        /// <inheritdoc/>
        public void LoadFromImage(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SaveToImage(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string InsertFact(string text)
        {
#if DEBUG
            Log($"text = {text}");
#endif

            if(!text.StartsWith("{:"))
            {
                text = $"{{: {text} :}}";
            }

#if DEBUG
            Log($"text (after) = {text}");
#endif

            var fact = _logicQueryParseAndCache.GetLogicRuleOrFact(text);

#if DEBUG
            Log($"fact = {fact}");
#endif

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveFact(string id)
        {
#if DEBUG
            Log($"id = {id}");
#endif

            throw new NotImplementedException();
        }
    }
}
