using System;
using System.Collections.Generic;
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

        public static string GetDefaultToShortStringInformation(this IShortObjectToString targetObject, uint n)
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

        public static void PrintShortObjProp(this StringBuilder sb, uint n, string propName, IShortObjectToString obj)
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
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}{item}");
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }
    }
}
