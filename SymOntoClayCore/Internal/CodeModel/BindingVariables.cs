/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class BindingVariables : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public BindingVariables()
        {
            _source = new List<BindingVariableItem>();
        }

        public BindingVariables(List<BindingVariableItem> source)
        {
            _source = source;

            foreach(var item in source)
            {
                var kind = item.Kind;

                switch(kind)
                {
                    case KindOfBindingVariable.LeftToRignt:
                        _varsDict[item.LeftVariable] = item.RightVariable;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }
        }

        private List<BindingVariableItem> _source;
        private Dictionary<StrongIdentifierValue, StrongIdentifierValue> _varsDict = new Dictionary<StrongIdentifierValue, StrongIdentifierValue>();

        public bool Any()
        {
            return _source.Any();
        }

        public int Count => _source.Count;

        public StrongIdentifierValue GetDest(StrongIdentifierValue sourceVarName)
        {
            if(_varsDict.ContainsKey(sourceVarName))
            {
                return _varsDict[sourceVarName];
            }

            return null;
        }

        public List<StrongIdentifierValue> GetTargetsList()
        {
            return _varsDict.Keys.ToList();
        }

        public List<StrongIdentifierValue> GetDestList()
        {
            return _varsDict.Values.ToList();
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BindingVariables Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public BindingVariables Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BindingVariables)context[this];
            }

            var result = new BindingVariables();
            context[this] = result;

            result._source = _source.Select(p => p.Clone(context)).ToList();
            result._varsDict = _varsDict.ToDictionary(p => p.Key.Clone(context), p => p.Value.Clone(context));

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
            sb.PrintObjListProp(n, "BindingVariables", _source);
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
            sb.PrintShortObjListProp(n, "BindingVariables", _source);
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
            sb.PrintBriefObjListProp(n, "BindingVariables", _source);
            return sb.ToString();
        }
    }
}
