/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static void Mix(DefaultSettingsOfCodeEntity source, DefaultSettingsOfCodeEntity dest)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
            //_gbcLogger.Info($"dest = {dest}");
#endif

            var context = new Dictionary<object, object>();

            if (dest.WhereSection.IsNullOrEmpty() && !source.WhereSection.IsNullOrEmpty())
            {
                dest.WhereSection = source.WhereSection.Select(p => p.CloneValue(context)).ToList();
            }

            if (dest.Holder == null && source.Holder != null)
            {
                dest.Holder = source.Holder.Clone(context);
            }

            if(dest.TypeOfAccess == TypeOfAccess.Unknown && source.TypeOfAccess != TypeOfAccess.Unknown)
            {
                dest.TypeOfAccess = source.TypeOfAccess;
            }
        }

        public static void SetUpInlineTrigger(InlineTrigger inlineTrigger, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpCodeItem(inlineTrigger, defaultSettings, context);
        }

        public static void SetUpLinguisticVariable(LinguisticVariable linguisticVariable, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpCodeItem(linguisticVariable, defaultSettings, context);
        }

        public static void SetUpAction(ActionDef action, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpCodeItem(action, defaultSettings, context);
        }

        public static void SetUpState(StateDef state, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpCodeItem(state, defaultSettings, context);
        }

        public static void SetUpNamedFunction(NamedFunction namedFunction, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpCodeItem(namedFunction, defaultSettings, context);
        }

        public static void SetUpOperator(Operator op, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpCodeItem(op, defaultSettings, context);
        }

        public static void SetUpInheritanceItem(InheritanceItem inheritanceItem, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpAnnotatedItem(inheritanceItem, defaultSettings, context);
        }

        public static void SetUpCodeItem(CodeItem item, DefaultSettingsOfCodeEntity defaultSettings, Dictionary<object, object> context)
        {
            if (defaultSettings == null)
            {
                return;
            }

            if (item.Holder == null && defaultSettings.Holder != null)
            {
                item.Holder = defaultSettings.Holder.Clone(context);
            }

            SetUpAnnotatedItem(item, defaultSettings, context);
        }

        public static void SetUpAnnotatedItem(AnnotatedItem item, DefaultSettingsOfCodeEntity defaultSettings)
        {
            var context = new Dictionary<object, object>();

            SetUpAnnotatedItem(item, defaultSettings, context);
        }

        public static void SetUpAnnotatedItem(AnnotatedItem item, DefaultSettingsOfCodeEntity defaultSettings, Dictionary<object, object> context)
        {
            if(defaultSettings == null)
            {
                return;
            }

            if(item.WhereSection.IsNullOrEmpty() && !defaultSettings.WhereSection.IsNullOrEmpty())
            {
                item.WhereSection = defaultSettings.WhereSection.Select(p => p.CloneValue(context)).ToList();
            }
        }
    }
}
