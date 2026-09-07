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
            if (type.IsEnum)
            {
                return Enum.Parse(type, literal, true);
            }

            switch (type.FullName)
            {
                case "System.Byte":
                    return Byte.Parse(literal);

                case "System.SByte":
                    return SByte.Parse(literal);

                case "System.Int16":
                    return Int16.Parse(literal);

                case "System.Int32":
                    return int.Parse(literal);

                case "System.Int64":
                    return Int64.Parse(literal);

                case "System.UInt16":
                    return UInt16.Parse(literal);

                case "System.UInt32":
                    return uint.Parse(literal);

                case "System.UInt64":
                    return ulong.Parse(literal);

                case "System.Single":
                    return Single.Parse(literal);

                case "System.Decimal":
                    return Decimal.Parse(literal);

                case "System.Double":
                    return Double.Parse(literal);

                case "System.Boolean":
                    return bool.Parse(literal);

                case "System.String":
                    return literal;

                case "System.Char":
                    throw new NotImplementedException("C7C7001E-32DB-4982-97A0-6D4FB470CEA3");

                case "System.DateTime":
                    throw new NotImplementedException("C66C62EB-6EA9-41A2-B5A6-B93B14C8DBD0");

                case "System.DateOnly":
                    throw new NotImplementedException("C12854E3-1938-4869-B57D-46E246CF2FBA");

                case "System.TimeOnly":
                    throw new NotImplementedException("C757DBDB-C779-43AA-B854-7D481F860CAD");

                case "System.TimeSpan":
                    throw new NotImplementedException("C2C051F2-0E9C-4845-9E26-863DB6507FF6");

                case "System.Guid":
                    throw new NotImplementedException("C2B19843-1E68-4A2B-BDE3-CFC60EB809F9");

                default:
                    throw new ArgumentOutOfRangeException(nameof(type.FullName), type.FullName, "4A06B1F6-BBF3-4B58-A122-8AD1AD23CECB");
            }
        }
    }
}
