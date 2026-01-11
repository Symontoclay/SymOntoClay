/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchResultItem : IResultOfQueryToRelation
    {
        public string KeyForTrigger
        {
            get
            {
                if(_keyForTrigger == null)
                {
                    CalculateKeyForTrigger();
                }

                return _keyForTrigger;
            }
        }

        private string _keyForTrigger;

        private void CalculateKeyForTrigger()
        {
            if(ResultOfVarOfQueryToRelationList.IsNullOrEmpty())
            {
                _keyForTrigger = string.Empty;
                return;
            }

            var strList = new List<string>();

            foreach(var resultVar in ResultOfVarOfQueryToRelationList.OrderBy(p => p.NameOfVar.NormalizedNameValue))
            {
                strList.Add(resultVar.NameOfVar.NormalizedNameValue);
                strList.Add(DebugHelperForRuleInstance.ToString(resultVar.FoundExpression));
            }

            _keyForTrigger = string.Join(string.Empty, strList);
        }

        public IList<IResultOfVarOfQueryToRelation> ResultOfVarOfQueryToRelationList { get; set; }

        public ulong GetLongHashCode(CheckDirtyOptions options)
        {
            ulong result = 0;

            if (!ResultOfVarOfQueryToRelationList.IsNullOrEmpty())
            {
                foreach (var item in ResultOfVarOfQueryToRelationList)
                {
                    result ^= item.GetLongHashCode(options);
                }
            }

            return result;
        }

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

            sb.PrintObjListProp(n, nameof(ResultOfVarOfQueryToRelationList), ResultOfVarOfQueryToRelationList);

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

            sb.PrintShortObjListProp(n, nameof(ResultOfVarOfQueryToRelationList), ResultOfVarOfQueryToRelationList);

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

            sb.PrintBriefObjListProp(n, nameof(ResultOfVarOfQueryToRelationList), ResultOfVarOfQueryToRelationList);

            return sb.ToString();
        }
    }
}
