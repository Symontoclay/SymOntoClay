/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
        public ParsingDirective(KindOfParsingDirective kindOfParsingDirective, object state, object stateAfterRunChild, ConcreteATNToken concreteATNToken, BaseSentenceItem phrase, object role)
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

            Role = role;
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

        public object Role { get; private set; }

        /// <inheritdoc/>
        public BaseParser CreateParser(ParserContext parserContext)
        {
            var parser = new T();
            parser.SetContext(parserContext);
            parser.SetStateAsInt32(State.Value);

            if (Role != null)
            {
                parser.SetRole(Role);
            }

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
            sb.AppendLine($"{spaces}{nameof(Role)} = {Role}");

            return sb.ToString();
        }
    }
}
