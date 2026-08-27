using System;
using System.Globalization;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class TypesHelper: ITypesHelper
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

        /// <inheritdoc/>
        public KindOfSerializedValue GetKindOfSerializedValue(Type type)
        {
            if (type == null)
            {
                return KindOfSerializedValue.Null;
            }

            if (type == typeof(object))
            {
                return KindOfSerializedValue.ObjectPtr;
            }

            if (type.IsEnum)
            {
                return KindOfSerializedValue.Literal;
            }

            switch (type.FullName)
            {
                case "System.Byte":
                    return KindOfSerializedValue.Literal;

                case "System.SByte":
                    return KindOfSerializedValue.Literal;

                case "System.Int16":
                    return KindOfSerializedValue.Literal;

                case "System.Int32":
                    return KindOfSerializedValue.Literal;

                case "System.Int64":
                    return KindOfSerializedValue.Literal;

                case "System.UInt16":
                    return KindOfSerializedValue.Literal;

                case "System.UInt32":
                    return KindOfSerializedValue.Literal;

                case "System.UInt64":
                    return KindOfSerializedValue.Literal;

                case "System.Single":
                    return KindOfSerializedValue.Literal;

                case "System.Decimal":
                    return KindOfSerializedValue.Literal;

                case "System.Double":
                    return KindOfSerializedValue.Literal;

                case "System.Boolean":
                    return KindOfSerializedValue.Literal;

                case "System.String":
                    return KindOfSerializedValue.Literal;

                case "System.Char":
                    return KindOfSerializedValue.Literal;

                case "System.DateTime":
                    return KindOfSerializedValue.Literal;

                case "System.DateOnly":
                    return KindOfSerializedValue.Literal;

                case "System.TimeOnly":
                    return KindOfSerializedValue.Literal;

                case "System.Guid": 
                    return KindOfSerializedValue.Literal;

                case "System.TimeSpan":
                    return KindOfSerializedValue.Literal;
            }

            return KindOfSerializedValue.ObjectPtr;
        }

        /// <inheritdoc/>
        public string ToString(object obj)
        {
            switch(obj)
            {
                case null: return string.Empty;

                case byte b: return b.ToString(_cultureInfo);

                case sbyte b: return b.ToString(_cultureInfo);

                case short s: return s.ToString(_cultureInfo);

                case int i: return i.ToString(_cultureInfo);

                case long l: return l.ToString(_cultureInfo);

                case ushort u: return u.ToString(_cultureInfo);

                case uint v: return v.ToString(_cultureInfo);

                case ulong ul: return ul.ToString(_cultureInfo);

                case float f: return f.ToString(_cultureInfo);

                case double d: return d.ToString(_cultureInfo);

                case decimal d: return d.ToString(_cultureInfo);

                case DateTime date: return date.ToString(_cultureInfo);

                case TimeSpan timeSpan: return timeSpan.ToString("G", _cultureInfo);

                case System.Guid giud: return giud.ToString("D", _cultureInfo);

                default: return obj.ToString();
            }

            /*
            //case DateOnly date: return date.ToStringOnly(_cultureInfo);

            //TimeOnly
             */
        }

        /// <inheritdoc/>
        public object FromString(Type type, string literal)
        {
            throw new NotImplementedException("C9C99FB3-2FE7-4652-9922-497908C9C170");
        }
    }
}
