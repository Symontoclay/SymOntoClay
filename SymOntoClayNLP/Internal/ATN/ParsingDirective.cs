using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ParsingDirective<T, S>: IParsingDirective
        where T: BaseParser, new()
    {
        public ParsingDirective(S state)
            : this(KindOfParsingDirective.RunCurrToken, state, false)
        {
        }

        public ParsingDirective(S state, bool recoveryCurrToken)
            : this(KindOfParsingDirective.RunCurrToken, state, recoveryCurrToken)
        {
        }

        public ParsingDirective(KindOfParsingDirective kindOfParsingDirective, S state, bool recoveryCurrToken)
        {
            KindOfParsingDirective = kindOfParsingDirective;
            _state = state;
            RecoveryCurrToken = recoveryCurrToken;
        }

        public ParsingDirective(S state, ConcreteATNToken concreteATNToken)
        {
            KindOfParsingDirective = KindOfParsingDirective.RunVariant;
            _state = state;
            ConcreteATNToken = concreteATNToken;
        }

        public KindOfParsingDirective KindOfParsingDirective { get; private set; }
        private S _state;

        public int State => Convert.ToInt32(_state);

        public bool RecoveryCurrToken { get; private set; }

        public ConcreteATNToken ConcreteATNToken { get; private set; }

        /// <inheritdoc/>
        public BaseParser CreateParser(ParserContext parserContext)
        {
            var parser = new T();
            parser.SetContext(parserContext);
            parser.SetStateAsInt32(Convert.ToInt32(_state));

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
            sb.AppendLine($"{spaces}State = {_state}");
            sb.AppendLine($"{spaces}{nameof(RecoveryCurrToken)} = {RecoveryCurrToken}");
            sb.PrintObjProp(n, nameof(ConcreteATNToken), ConcreteATNToken);

            return sb.ToString();
        }
    }
}
