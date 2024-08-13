using System;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
using SymOntoClay.Serialization;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class CodeChunk : BaseCodeChunk, ICodeChunk
    {
        public CodeChunk(string id, ICodeChunksContext codeChunksContext, Action action)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private ICodeChunksContext _codeChunksContext;
        private Action _action;

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action();
        }
    }
}
