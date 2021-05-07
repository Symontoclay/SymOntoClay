/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LinguisticVariableConstraint : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public void AddItem(LinguisticVariableConstraintItem item)
        {
            if(item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if(_items.Contains(item))
            {
                return;
            }

            _items.Add(item);
        }

        public IEnumerable<LinguisticVariableConstraintItem> Items => _items;

        public bool IsEmpty => !_items.Any();

        private List<LinguisticVariableConstraintItem> _items { get; set; } = new List<LinguisticVariableConstraintItem>();

        public bool isFit(ReasonOfFuzzyLogicResolving reason)
        {
            if((reason == null || reason.Kind == KindOfReasonOfFuzzyLogicResolving.Unknown) && _items.IsNullOrEmpty())
            {
                return true;
            }

            var kindOfReson = reason.Kind;

            switch(kindOfReson)
            {
                case KindOfReasonOfFuzzyLogicResolving.Inheritance:
                    return _items.Any(p => p.Kind == KindOfLinguisticVariableConstraintItem.Inheritance);

                case KindOfReasonOfFuzzyLogicResolving.Relation:
                    return _items.Any(p => p.Kind == KindOfLinguisticVariableConstraintItem.Relation && p.RelationName == reason.RelationName);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfReson), kindOfReson, null);
            }
        }

        private bool _isDirty = true;

        public void CheckDirty()
        {
            if (_isDirty)
            {
                Check();
                _isDirty = false;
            }
        }

        public void Check()
        {
            if (_items.Any(p => p.Kind == KindOfLinguisticVariableConstraintItem.Unknown))
            {
                throw new Exception("Linguistic variable constraints can not be Unknown!");
            }

            if(_items.Any(p => p.Kind == KindOfLinguisticVariableConstraintItem.Inheritance))
            {
                if (_items.Count(p => p.Kind == KindOfLinguisticVariableConstraintItem.Inheritance) > 1)
                {
                    throw new Exception("Linguistic variable's constraints for inheritance is duplicated!");
                }

                if(!_items.Any(p => p.Kind == KindOfLinguisticVariableConstraintItem.Relation && p.RelationName.NameValue == "is"))
                {
                    var relationItem = new LinguisticVariableConstraintItem() { Kind = KindOfLinguisticVariableConstraintItem.Relation };
                    relationItem.RelationName = NameHelper.CreateName("is");
                    _items.Add(relationItem);
                }
            }

            if(_items.Any(p => p.Kind == KindOfLinguisticVariableConstraintItem.Relation))
            {
                var groppedItems = _items.Where(p => p.Kind == KindOfLinguisticVariableConstraintItem.Relation).GroupBy(p => p.RelationName).Where(p => p.Count() > 1).Select(p => $"`{p.Key.NameValue}`");
            
                if(groppedItems.Any())
                {
                    if(groppedItems.Count() == 1)
                    {
                        throw new Exception($"Linguistic variable's constraint for relation {groppedItems.First()} is duplicated!");
                    }
                    else
                    {
                        throw new Exception($"Linguistic variable's constraints for relation {string.Join(", ", groppedItems)} are duplicated!");
                    }
                }

                if(_items.Any(p => p.Kind == KindOfLinguisticVariableConstraintItem.Relation && p.RelationName.NameValue == "is") && !_items.Any(p => p.Kind == KindOfLinguisticVariableConstraintItem.Inheritance))
                {
                    var inheritanceItem = new LinguisticVariableConstraintItem() { Kind = KindOfLinguisticVariableConstraintItem.Inheritance };
                    _items.Add(inheritanceItem);
                }
            }
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public LinguisticVariableConstraint Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public LinguisticVariableConstraint Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (LinguisticVariableConstraint)context[this];
            }

            var result = new LinguisticVariableConstraint();
            context[this] = result;

            result._items = _items?.Select(p => p.Clone(context)).ToList();

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

            sb.PrintObjListProp(n, nameof(Items), Items);

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

            sb.PrintShortObjListProp(n, nameof(Items), Items);

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

            sb.PrintBriefObjListProp(n, nameof(Items), Items);

            return sb.ToString();
        }
    }
}
