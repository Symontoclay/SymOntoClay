/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class DefaultSettingsOfCodeEntityHelper
    {
        public static void Mix(DefaultSettingsOfCodeEntity source, DefaultSettingsOfCodeEntity dest)
        {
            var context = new Dictionary<object, object>();

            if (dest.QuantityQualityModalities.IsNullOrEmpty() && !source.QuantityQualityModalities.IsNullOrEmpty()) 
            {
                dest.QuantityQualityModalities = source.QuantityQualityModalities.Select(p => p.CloneValue(context)).ToList();
            }

            if (dest.WhereSection.IsNullOrEmpty() && !source.WhereSection.IsNullOrEmpty())
            {
                dest.WhereSection = source.WhereSection.Select(p => p.CloneValue(context)).ToList();
            }

            if (dest.Holder == null && source.Holder != null)
            {
                dest.Holder = source.Holder.Clone(context);
            }
        }

        public static void SetUpCodeEntity(CodeEntity codeEntity, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpAnnotatedItem(codeEntity, defaultSettings, context);
        }

        public static void SetUpInlineTrigger(InlineTrigger inlineTrigger, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpAnnotatedItem(inlineTrigger, defaultSettings, context);
        }

        public static void SetUpInheritanceItem(InheritanceItem inheritanceItem, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpAnnotatedItem(inheritanceItem, defaultSettings, context);
        }

        public static void SetUpAnnotatedItem(AnnotatedItem item, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpAnnotatedItem(item, defaultSettings, context);
        }

        public static void SetUpAnnotatedItem(AnnotatedItem item, DefaultSettingsOfCodeEntity defaultSettings, Dictionary<object, object> context)
        {
            if(item.QuantityQualityModalities.IsNullOrEmpty() && !defaultSettings.QuantityQualityModalities.IsNullOrEmpty())
            {
                item.QuantityQualityModalities = defaultSettings.QuantityQualityModalities.Select(p => p.CloneValue(context)).ToList();
            }

            if(item.WhereSection.IsNullOrEmpty() && !defaultSettings.WhereSection.IsNullOrEmpty())
            {
                item.WhereSection = defaultSettings.WhereSection.Select(p => p.CloneValue(context)).ToList();
            }

            if (item.Holder == null && defaultSettings.Holder != null)
            {
                item.Holder = defaultSettings.Holder.Clone(context);
            }
        }
    }
}
