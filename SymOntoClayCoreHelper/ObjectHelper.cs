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

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper
{
    public static class ObjectHelper
    {
        public static bool IsNumber(object value)
        {
            if(value == null)
            {
                return false;
            }

            var type = value.GetType();

            if(type == typeof(float))
            {
                return true;
            }

            if (type == typeof(double))
            {
                return true;
            }

            if (type == typeof(decimal))
            {
                return true;
            }

            if (type == typeof(ulong))
            {
                return true;
            }

            if (type == typeof(long))
            {
                return true;
            }

            if (type == typeof(uint))
            {
                return true;
            }

            if (type == typeof(int))
            {
                return true;
            }

            if (type == typeof(ushort))
            {
                return true;
            }

            if (type == typeof(short))
            {
                return true;
            }

            if (type == typeof(byte))
            {
                return true;
            }

            return false;
        }

        public static bool IsEquals(object value1, object value2)
        {
            if (value1.Equals(value2))
            {
                return true;
            }

            if (IsNumber(value1) && IsNumber(value2))
            {
                if (Convert.ToDouble(value1) == Convert.ToDouble(value2))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Type> GetAllBaseTypes(Type type)
        {
            var result = new List<Type>();

            result.AddRange(type.GetInterfaces());

            FillUpBaseTypes(type.BaseType, result);

            return result;
        }

        private static void FillUpBaseTypes(Type type, List<Type> result)
        {
            if(type == null)
            {
                return;
            }

            result.Add(type);

            FillUpBaseTypes(type.BaseType, result);
        }
    }
}
