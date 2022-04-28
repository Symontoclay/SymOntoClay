﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class WholeTextParsingResult : IObjectToString
    {
        public bool IsSuccess { get; set; }
        public int CountSteps { get; set; }
        public List<ParsingResult> Results { get; set; } = new List<ParsingResult>();
        public Exception Error { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.AppendLine($"{spaces}{nameof(CountSteps)} = {CountSteps}");
            sb.PrintObjListProp(n, nameof(Results), Results);
            sb.AppendLine($"{spaces}{nameof(Error)} = {Error}");

            return sb.ToString();
        }
    }
}