using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public abstract class BaseWithParentsSourceWordItem : IObjectToString, IRootWordBasedMetrixOfSourceWordNet
    {
        public int WordNum { get; set; }
        public string Word { get; set; }
        public IList<int> ParentWordNumsList { get; set; } = new List<int>();
        public bool IsDigit { get; set; }
        public bool IsName { get; set; }
        public bool IsComplex { get; set; }

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(WordNum)} = {WordNum}");
            sb.AppendLine($"{spaces}{nameof(Word)} = {Word}");
            if (ParentWordNumsList == null)
            {
                sb.AppendLine($"{spaces}{nameof(ParentWordNumsList)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(ParentWordNumsList)}");
                foreach (var parentWordNum in ParentWordNumsList)
                {
                    sb.AppendLine($"{nextNSpaces}{parentWordNum}");
                }
                sb.AppendLine($"{spaces}End {nameof(ParentWordNumsList)}");
            }
            sb.AppendLine($"{spaces}{nameof(IsDigit)} = {IsDigit}");
            sb.AppendLine($"{spaces}{nameof(IsName)} = {IsName}");
            sb.AppendLine($"{spaces}{nameof(IsComplex)} = {IsComplex}");
            return sb.ToString();
        }
    }
}
