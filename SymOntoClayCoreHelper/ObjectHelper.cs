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
    }
}
