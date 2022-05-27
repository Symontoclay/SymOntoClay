using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class EmptyDummyParser : BaseParser
    {
        /// <inheritdoc/>
        public override void SetStateAsInt32(int state)
        {
        }

        /// <inheritdoc/>
        public override string GetStateAsString()
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public override BaseParser Fork(ParserContext newParserContext)
        {
            var result = new EmptyDummyParser();
            result.FillUpBaseParser(this, newParserContext);
            return result;
        }
    }
}
