using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    public static class DisplayHelper
    {
        public static string Spaces(uint n)
        {
            if (n == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < n; i++)
            {
                sb.Append(" ");
            }

            return sb.ToString();
        }

        public static string GetDefaultToStringInformation(this IObjectToString targetObject, uint n)
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            var nameOfType = targetObject.GetType().FullName;
            sb.AppendLine($"{spaces}Begin {nameOfType}");
            sb.Append(targetObject.PropertiesToString(nextN));
            sb.AppendLine($"{spaces}End {nameOfType}");
            return sb.ToString();
        }

        public static string GetDefaultToShortStringInformation(this IObjectToShortString targetObject, uint n)
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            var nameOfType = targetObject.GetType().FullName;
            sb.AppendLine($"{spaces}Begin {nameOfType}");
            sb.Append(targetObject.PropertiesToShortString(nextN));
            sb.AppendLine($"{spaces}End {nameOfType}");
            return sb.ToString();
        }

        public static string GetDefaultToBriefStringInformation(this IObjectToBriefString targetObject, uint n)
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            var nameOfType = targetObject.GetType().FullName;
            sb.AppendLine($"{spaces}Begin {nameOfType}");
            sb.Append(targetObject.PropertiesToBriefString(nextN));
            sb.AppendLine($"{spaces}End {nameOfType}");
            return sb.ToString();
        }

        public static void PrintObjProp(this StringBuilder sb, uint n, string propName, IObjectToString obj)
        {
            var spaces = Spaces(n);
            var nextN = n + 4;

            if (obj == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                sb.Append(obj.ToString(nextN));
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintShortObjProp(this StringBuilder sb, uint n, string propName, IObjectToShortString obj)
        {
            var spaces = Spaces(n);
            var nextN = n + 4;

            if (obj == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                sb.Append(obj.ToShortString(nextN));
                sb.AppendLine($"{spaces}End {propName}");
            }
        }
        
        public static void PrintBriefObjProp(this StringBuilder sb, uint n, string propName, IObjectToBriefString obj)
        {
            var spaces = Spaces(n);
            var nextN = n + 4;

            if (obj == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                sb.Append(obj.ToBriefString(nextN));
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintPODList<T>(this StringBuilder sb, uint n, string propName, IList<T> items)
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = Spaces(nextN);

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}{propName} = Begin List");
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}{item}");
                }
                sb.AppendLine($"{spaces}End List");
            }
        }

        public static void PrintObjListProp<T>(this StringBuilder sb, uint n, string propName, IEnumerable<T> items) where T : IObjectToString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.Append(item.ToString(nextN));
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintShortObjListProp<T>(this StringBuilder sb, uint n, string propName, IEnumerable<T> items) where T: IObjectToShortString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.Append(item.ToShortString(nextN));
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintBriefObjListProp<T>(this StringBuilder sb, uint n, string propName, IEnumerable<T> items) where T : IObjectToBriefString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.Append(item.ToBriefString(nextN));
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintExisting(this StringBuilder sb, uint n, string propName, object value)
        {
            var spaces = Spaces(n);
            var mark = value == null ? "No" : "Yes";
            sb.AppendLine($"{spaces}{propName} = {mark}");
        }

        public static void PrintExistingList<T>(this StringBuilder sb, uint n, string propName, IEnumerable<T> items)
        {
            var spaces = Spaces(n);
            var mark = items == null ? "No" : items.Any() ? "Yes" : "No";
            sb.AppendLine($"{spaces}{propName} = {mark}");
        }

        public static string WriteListToString<T>(this IEnumerable<T> items) where T: IObjectToString
        {
            var sb = new StringBuilder();
            sb.AppendLine("Begin List");
            if(!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.Append(item.ToString(4u));
                }
            }
            sb.AppendLine("End List");
            return sb.ToString();
        }

        public static string WriteListToShortString<T>(this IEnumerable<T> items) where T : IObjectToShortString
        {
            var sb = new StringBuilder();
            sb.AppendLine("Begin List");
            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.Append(item.ToShortString(4u));
                }
            }
            sb.AppendLine("End List");
            return sb.ToString();
        }

        public static string WriteListToBriefString<T>(this IEnumerable<T> items) where T : IObjectToBriefString
        {
            var sb = new StringBuilder();
            sb.AppendLine("Begin List");
            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.Append(item.ToBriefString(4u));
                }
            }
            sb.AppendLine("End List");
            return sb.ToString();
        }
    }
}
