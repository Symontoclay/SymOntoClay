using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseCompoundTaskParser : BaseInternalParser
    {
        protected BaseCompoundTaskParser(InternalParserContext context)
            : base(context)
        {
        }

        public BaseCompoundTask Result { get; protected set; }
    }
}
