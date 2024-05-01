/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;
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

        public static string WriteIProcessInfoToToHumanizedString(this IProcessInfo processInfo, IMonitorLogger logger)
        {
#if DEBUG
            //DebugLogger.Instance.Info("Begin");
#endif

            var processesInfoList = ProcessInfoHelper.ConvertToHierarchyList(logger, processInfo);

            //foreach(var item in processesInfoList)
            //{
#if DEBUG
            //DebugLogger.Instance.Info($"item.Id = {item.Id};item.EndPointName = {item.EndPointName}; item.Priority = {item.Priority}; item.Priority = {item.GlobalPriority}");
#endif
            //}

            return string.Join(" -> ", processesInfoList.Select(item => $"({item.Id}) {item.EndPointName} [{item.Priority}]"));
        }
    }
}
