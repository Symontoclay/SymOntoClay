using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class ItemsListDbgHelper
    {
        public static string WriteListToToHumanizedString(this IEnumerable<IObjectToHumanizedString> items, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return WriteListToToHumanizedString(items, opt);
        }

        public static string WriteListToToHumanizedString(this IEnumerable<IObjectToHumanizedString> items, DebugHelperOptions options)
        {
            if (items == null)
            {
                return "NULL";
            }

            var spaces = DisplayHelper.Spaces(DisplayHelper.IndentationStep);

            var sb = new StringBuilder();
            sb.AppendLine("Begin List");
            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.AppendLine($"{spaces}{item.ToHumanizedString(options)}");
                }
            }
            sb.AppendLine("End List");
            return sb.ToString();
        }
    }
}
