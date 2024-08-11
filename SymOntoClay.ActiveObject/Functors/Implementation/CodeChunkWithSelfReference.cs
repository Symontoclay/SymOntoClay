using System;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public partial class CodeChunkWithSelfReference: ICodeChunkWithSelfReference
    {
        /// <summary>
        /// Empty contructor for deserialization.
        /// </summary>
        public CodeChunkWithSelfReference()
        {
        }

        public CodeChunkWithSelfReference(string id, ICodeChunksContext codeChunksContext, Action action)
        {
        }


    }
}
