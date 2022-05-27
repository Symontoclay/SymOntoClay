using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    public class Word : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.Word;

        /// <inheritdoc/>
        public override bool IsWord => true;

        /// <inheritdoc/>
        public override Word AsWord => this;

        public bool IsNumber { get; set; }
        public string Content { get; set; } = string.Empty;

        public BaseGrammaticalWordFrame WordFrame { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => WordFrame;

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public Word Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public Word Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Word)context[this];
            }

            var result = new Word();
            context[this] = result;

            result.IsNumber = IsNumber;
            result.Content = Content;
            result.WordFrame = WordFrame;

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNumber)} = {IsNumber}");
            sb.AppendLine($"{spaces}{nameof(Content)} = {Content}");
            sb.PrintObjProp(n, nameof(WordFrame), WordFrame);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{Content}");

            return sb.ToString();
        }
    }
}
