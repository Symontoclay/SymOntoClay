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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{    
    public class QueryExecutingCardForIndexedPersistLogicalData : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public StrongIdentifierValue TargetRelation { get; set; }
        public int CountParams { get; set; }
        public IList<QueryExecutingCardAboutVar> VarsInfoList { get; set; }
        public IList<QueryExecutingCardAboutKnownInfo> KnownInfoList { get; set; }
        public List<ResultOfQueryToRelation> ResultsOfQueryToRelationList { get; set; } = new List<ResultOfQueryToRelation>();
        public List<PostFilterOfQueryExecutingCardForPersistLogicalData> PostFiltersList { get; set; } = new List<PostFilterOfQueryExecutingCardForPersistLogicalData>();
        public bool IsSuccess { get; set; }
        public bool IsPostFiltersListOnly { get; set; }
        public List<StrongIdentifierValue> UsedKeysList { get; set; } = new List<StrongIdentifierValue>();

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

            sb.PrintObjProp(n, nameof(TargetRelation), TargetRelation);
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintObjListProp(n, nameof(PostFiltersList), PostFiltersList);

            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.AppendLine($"{spaces}{nameof(IsPostFiltersListOnly)} = {IsPostFiltersListOnly}");
            sb.PrintObjListProp(n, nameof(UsedKeysList), UsedKeysList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(TargetRelation), TargetRelation);
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintShortObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintShortObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintShortObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintShortObjListProp(n, nameof(PostFiltersList), PostFiltersList);

            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.AppendLine($"{spaces}{nameof(IsPostFiltersListOnly)} = {IsPostFiltersListOnly}");
            sb.PrintShortObjListProp(n, nameof(UsedKeysList), UsedKeysList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(TargetRelation), TargetRelation);
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintBriefObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintBriefObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintBriefObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintBriefObjListProp(n, nameof(PostFiltersList), PostFiltersList);

            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.AppendLine($"{spaces}{nameof(IsPostFiltersListOnly)} = {IsPostFiltersListOnly}");
            sb.PrintBriefObjListProp(n, nameof(UsedKeysList), UsedKeysList);

            return sb.ToString();
        }
    }
}
