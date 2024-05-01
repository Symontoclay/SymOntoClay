/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Noun_phrase
    /// NP
    /// </summary>
    public class NounPhrase: BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.NounPhrase;

        /// <inheritdoc/>
        public override bool IsNounPhrase => true;

        /// <inheritdoc/>
        public override NounPhrase AsNounPhrase => this;

        /// <summary>
        /// Noun, NounPhrase, ConjunctionPhrase, QuantifierPhrase which is root of the phrase.
        /// </summary>
        public BaseSentenceItem N { get; set; }

        /// <summary>
        /// Determiner
        /// Such as "the", "this", "my", "some", "Jane's"
        /// [the guy with a hat]'s dog
        /// [the girl who was laughing]'s scarf
        /// </summary>
        public BaseSentenceItem D { get; set; }

        /// <summary>
        /// "many" in "many very happy girls"
        /// </summary>
        public BaseSentenceItem QP { get; set; }

        /// <summary>
        /// such as "large", "beautiful", "sweeter"
        ///  such as "extremely large", "hard as nails", "made of wood", "sitting on the step"
        /// </summary>
        public BaseSentenceItem AP { get; set; }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Noun_adjunct
        /// "college" in "a college student"
        /// </summary>
        public BaseSentenceItem NounAdjunct { get; set; }

        /// <summary>
        /// Represents "not"
        /// </summary>
        public Word Negation { get; set; }

        public bool HasPossesiveMark { get; set; }

        /// <summary>
        /// such as "in the drawing room", "of his aunt"
        /// </summary>
        public BaseSentenceItem PP { get; set; }

        /// <summary>
        /// such as "(over) there" in the noun phrase "the man (over) there"
        /// </summary>
        public BaseSentenceItem AdvP { get; set; }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Relative_clause
        /// such as "which we noticed"
        /// </summary>
        public BaseSentenceItem RelativeClauses { get; set; }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Clause
        /// other clauses serving as complements to the noun, such as "that God exists" in the noun phrase "the belief that God exists"
        /// </summary>
        public BaseSentenceItem OtherClauses { get; set; }

        /// <summary>
        /// infinitive phrases, such as "to sing well" and "to beat" in the noun phrases "a desire to sing well" and "the man to beat"
        /// </summary>
        public BaseSentenceItem InfinitivePhrases { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => N.RootWordFrame;

        /// <inheritdoc/>
        public override string RootWordAsString => N.RootWordAsString;

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            return N.GetConditionalLogicalMeaning(conditionalWord);
        }

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public NounPhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public NounPhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (NounPhrase)context[this];
            }

            var result = new NounPhrase();
            context[this] = result;

            result.N = N?.CloneBaseSentenceItem(context);
            result.D = D?.CloneBaseSentenceItem(context);
            result.QP = QP?.CloneBaseSentenceItem(context);
            result.AP = AP?.CloneBaseSentenceItem(context);
            result.NounAdjunct = NounAdjunct?.CloneBaseSentenceItem(context);

            result.Negation = Negation?.Clone(context);

            result.HasPossesiveMark = HasPossesiveMark;

            result.PP = PP?.CloneBaseSentenceItem(context);
            result.AdvP = AdvP?.CloneBaseSentenceItem(context);
            result.RelativeClauses = RelativeClauses?.CloneBaseSentenceItem(context);
            result.OtherClauses = OtherClauses?.CloneBaseSentenceItem(context);
            result.InfinitivePhrases = InfinitivePhrases?.CloneBaseSentenceItem(context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(N), N);
            sb.PrintObjProp(n, nameof(D), D);
            sb.PrintObjProp(n, nameof(QP), QP);
            sb.PrintObjProp(n, nameof(AP), AP);
            sb.PrintObjProp(n, nameof(NounAdjunct), NounAdjunct);
            sb.PrintObjProp(n, nameof(Negation), Negation);
            sb.AppendLine($"{spaces}{nameof(HasPossesiveMark)} = {HasPossesiveMark}");
            sb.PrintObjProp(n, nameof(PP), PP);
            sb.PrintObjProp(n, nameof(AdvP), AdvP);            
            sb.PrintObjProp(n, nameof(RelativeClauses), RelativeClauses);
            sb.PrintObjProp(n, nameof(OtherClauses), OtherClauses);
            sb.PrintObjProp(n, nameof(InfinitivePhrases), InfinitivePhrases);


            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNspaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}NP");

            if(N != null)
            {
                sb.AppendLine($"{nextNspaces}N:");
                sb.Append(N.ToDbgString(nextNextN));
            }

            if (D != null)
            {
                sb.AppendLine($"{nextNspaces}D:");
                sb.Append(D.ToDbgString(nextNextN));
            }

            if (QP != null)
            {
                throw new NotImplementedException();
            }

            if (AP != null)
            {
                sb.AppendLine($"{nextNspaces}AP:");
                sb.Append(AP.ToDbgString(nextNextN));
            }

            if (NounAdjunct != null)
            {
                throw new NotImplementedException();
            }

            if (Negation != null)
            {
                throw new NotImplementedException();
            }

            if (HasPossesiveMark)
            {
                throw new NotImplementedException();
            }

            if (PP != null)
            {
                throw new NotImplementedException();
            }

            if (AdvP != null)
            {
                throw new NotImplementedException();
            }

            if (RelativeClauses != null)
            {
                throw new NotImplementedException();
            }

            if (OtherClauses != null)
            {
                throw new NotImplementedException();
            }

            if (InfinitivePhrases != null)
            {
                throw new NotImplementedException();
            }

            return sb.ToString();
        }
    }
}
