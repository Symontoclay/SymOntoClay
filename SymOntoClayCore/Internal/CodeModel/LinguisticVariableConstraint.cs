﻿using SymOntoClay.Core.Internal.CodeModel.Helpers;
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

            //var kindOfItem = item.Kind;

            //switch(kindOfItem)
            //{
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(kindOfItem), kindOfItem, null);
            //}
        }

        public IEnumerable<LinguisticVariableConstraintItem> Items => _items;

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
                    return _items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Inheritance);

                case KindOfReasonOfFuzzyLogicResolving.Relation:
                    return _items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Relation && p.RelationName == reason.RelationName);

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
            if (_items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Unknown))
            {
                throw new Exception("Linguistic variable constraints can not be Unknown!");
            }

            if(_items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Inheritance))
            {
                if (_items.Count(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Inheritance) > 1)
                {
                    throw new Exception("Linguistic variable's constraints for inheritance is duplicated!");
                }

                if(!_items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Relation && p.RelationName.NameValue == "is"))
                {
                    var relationItem = new LinguisticVariableConstraintItem() { Kind = KindOfLinguisticVariableСonstraintItem.Relation };
                    relationItem.RelationName = NameHelper.CreateName("is");
                    _items.Add(relationItem);
                }
            }

            if(_items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Relation))
            {
                var groppedItems = _items.Where(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Relation).GroupBy(p => p.RelationName).Where(p => p.Count() > 1).Select(p => $"`{p.Key.NameValue}`");
            
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

                if(_items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Relation && p.RelationName.NameValue == "is") && !_items.Any(p => p.Kind == KindOfLinguisticVariableСonstraintItem.Inheritance))
                {
                    var inheritanceItem = new LinguisticVariableConstraintItem() { Kind = KindOfLinguisticVariableСonstraintItem.Inheritance };
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
