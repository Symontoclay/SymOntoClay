using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.ParsingDirectives
{
    public class ParsingDirective<T>: IParsingDirective
        where T: BaseParser, new()
    {
        public ParsingDirective(KindOfParsingDirective kindOfParsingDirective, object state, object stateAfterRunChild, ConcreteATNToken concreteATNToken, BaseSentenceItem phrase)
        {
            KindOfParsingDirective = kindOfParsingDirective;

            if(state != null)
            {
                State = Convert.ToInt32(state);
            }            

            if(stateAfterRunChild != null)
            {
                StateAfterRunChild = Convert.ToInt32(stateAfterRunChild);
            }

            ConcreteATNToken = concreteATNToken;
            Phrase = phrase;
        }

        /// <inheritdoc/>
        public KindOfParsingDirective KindOfParsingDirective { get; private set; }

        /// <inheritdoc/>
        public int? State { get; private set; }

        /// <inheritdoc/>
        public int? StateAfterRunChild { get; private set; }

        /// <inheritdoc/>
        public ConcreteATNToken ConcreteATNToken { get; private set; }

        /// <inheritdoc/>
        public BaseSentenceItem Phrase { get; private set; }

        /// <inheritdoc/>
        public BaseParser CreateParser(ParserContext parserContext)
        {
            var parser = new T();
            parser.SetContext(parserContext);
            parser.SetStateAsInt32(State.Value);

            return parser;
        }

        /// <inheritdoc/>
        public Type ParserType => typeof(T);

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfParsingDirective)} = {KindOfParsingDirective}");
            sb.AppendLine($"{spaces}Parser = {typeof(T).FullName}");
            sb.AppendLine($"{spaces}{nameof(State)} = {State}");
            sb.AppendLine($"{spaces}{nameof(StateAfterRunChild)} = {StateAfterRunChild}");
            sb.PrintObjProp(n, nameof(ConcreteATNToken), ConcreteATNToken);
            sb.PrintObjProp(n, nameof(Phrase), Phrase);

            return sb.ToString();
        }
    }
}
