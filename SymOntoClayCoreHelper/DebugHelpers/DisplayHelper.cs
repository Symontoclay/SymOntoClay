/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

        public static string GetDefaultToDbgStringInformation(this IObjectToDbgString targetObject, uint n)
        {
            return targetObject.PropertiesToDbgString(n);
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

        public static void PrintObjListProp<T>(this StringBuilder sb, uint n, string propName, IEnumerable<T> items) 
            where T : IObjectToString
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
                    if(item == null)
                    {
                        sb.AppendLine($"{nextNSpaces}NULL");
                    }
                    else
                    {
                        sb.Append(item.ToString(nextN));
                    }           
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintShortObjListProp<T>(this StringBuilder sb, uint n, string propName, IEnumerable<T> items) 
            where T: IObjectToShortString
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
                    if (item == null)
                    {
                        sb.AppendLine($"{nextNSpaces}NULL");
                    }
                    else
                    {
                        sb.Append(item.ToShortString(nextN));
                    }                  
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintBriefObjListProp<T>(this StringBuilder sb, uint n, string propName, IEnumerable<T> items) 
            where T : IObjectToBriefString
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
                    if (item == null)
                    {
                        sb.AppendLine($"{nextNSpaces}NULL");
                    }
                    else
                    {
                        sb.Append(item.ToBriefString(nextN));
                    }                   
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintValueTypesListProp<T>(this StringBuilder sb, uint n, string propName, IEnumerable<T> items) 
            where T : struct
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

        public static void PrintObjDict_1_Prop<K, V>(this StringBuilder sb, uint n, string propName, IDictionary<K, V> items)
            where K : IObjectToString
            where V : IObjectToString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintShortObjDict_1_Prop<K, V>(this StringBuilder sb, uint n, string propName, IDictionary<K, V> items)
            where K : IObjectToShortString
            where V : IObjectToShortString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintBriefObjDict_1_Prop<K, V>(this StringBuilder sb, uint n, string propName, IDictionary<K, V> items)
            where K : IObjectToBriefString
            where V : IObjectToBriefString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintObjDict_2_Prop<K, V>(this StringBuilder sb, uint n, string propName, IDictionary<K, V> items)
            where K : struct
            where V : IObjectToString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;
            
            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintShortObjDict_2_Prop<K, V>(this StringBuilder sb, uint n, string propName, IDictionary<K, V> items)
            where K : struct
            where V : IObjectToShortString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {propName}");
            }
        }

        public static void PrintBriefObjDict_2_Prop<K, V>(this StringBuilder sb, uint n, string propName, IDictionary<K, V> items)
            where K : struct
            where V : IObjectToBriefString
        {
            var spaces = Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            if (items == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {propName}");
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
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

        public static string WriteListToString<T>(this IEnumerable<T> items) 
            where T: IObjectToString
        {
            if(items == null)
            {
                return "NULL";
            }

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

        public static string WriteListToShortString<T>(this IEnumerable<T> items) 
            where T : IObjectToShortString
        {
            if (items == null)
            {
                return "NULL";
            }

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

        public static string WriteListToBriefString<T>(this IEnumerable<T> items) 
            where T : IObjectToBriefString
        {
            if (items == null)
            {
                return "NULL";
            }

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

        public static string WriteDict_1_ToString<K, V>(this IDictionary<K, V> items) 
            where K: IObjectToString
            where V: IObjectToString
        {
            if (items == null)
            {
                return "NULL";
            }

            var nextN = 4u;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            var sb = new StringBuilder();
            sb.AppendLine("Begin Dictionary");

            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
            }

            sb.AppendLine("End Dictionary");
            return sb.ToString();
        }

        public static string WriteDict_1_ToShortString<K, V>(this IDictionary<K, V> items)
            where K : IObjectToShortString
            where V : IObjectToShortString
        {
            if (items == null)
            {
                return "NULL";
            }

            var nextN = 4u;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            var sb = new StringBuilder();
            sb.AppendLine("Begin Dictionary");

            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
            }

            sb.AppendLine("End Dictionary");
            return sb.ToString();
        }

        public static string WriteDict_1_ToBriefString<K, V>(this IDictionary<K, V> items)
            where K : IObjectToBriefString
            where V : IObjectToBriefString
        {
            if (items == null)
            {
                return "NULL";
            }

            var nextN = 4u;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            var sb = new StringBuilder();
            sb.AppendLine("Begin Dictionary");

            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
            }

            sb.AppendLine("End Dictionary");
            return sb.ToString();
        }

        public static string WriteDict_2_ToString<K, V>(this IDictionary<K, V> items)
            where K : struct
            where V : IObjectToString
        {
            if (items == null)
            {
                return "NULL";
            }

            var nextN = 4u;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            var sb = new StringBuilder();
            sb.AppendLine("Begin Dictionary");

            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
            }

            sb.AppendLine("End Dictionary");
            return sb.ToString();
        }

        public static string WriteDict_2_ToShortString<K, V>(this IDictionary<K, V> items)
            where K : struct
            where V : IObjectToShortString
        {
            if (items == null)
            {
                return "NULL";
            }

            var nextN = 4u;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            var sb = new StringBuilder();
            sb.AppendLine("Begin Dictionary");

            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
            }

            sb.AppendLine("End Dictionary");
            return sb.ToString();
        }

        public static string WriteDict_2_ToBriefString<K, V>(this IDictionary<K, V> items)
            where K : struct
            where V : IObjectToBriefString
        {
            if (items == null)
            {
                return "NULL";
            }

            var nextN = 4u;
            var nextNSpaces = Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;

            var sb = new StringBuilder();
            sb.AppendLine("Begin Dictionary");

            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
            }

            sb.AppendLine("End Dictionary");
            return sb.ToString();
        }
    }
}
