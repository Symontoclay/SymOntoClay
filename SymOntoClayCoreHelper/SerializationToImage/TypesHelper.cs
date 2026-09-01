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
                    throw new NotImplementedException("C9719420-E5A5-41B5-849D-4E4FA0F98BBD");

                case "System.SByte":
                    throw new NotImplementedException("C8950F25-F286-4A5A-A62E-A8B556485E28");

                case "System.Int16":
                    throw new NotImplementedException("C5202D05-2D1D-4F66-A1F9-B9F9044C4B2A");

                case "System.Int32":
                    return int.Parse(literal);

                case "System.Int64":
                    throw new NotImplementedException("C3DE4402-94D8-4AFD-B67C-69E59304373B");

                case "System.UInt16":
                    throw new NotImplementedException("CCE6CE31-2758-46F1-A168-65F64A4EE64C");

                case "System.UInt32":
                    throw new NotImplementedException("C5F62D15-CE59-443E-8FFA-51575996DE3F");

                case "System.UInt64":
                    throw new NotImplementedException("C7A2F01C-CB41-4860-A118-5F14CB63F091");

                case "System.Single":
                    throw new NotImplementedException("C9DF0F3B-F993-43E6-9C29-BE7657E8C3F7");

                case "System.Decimal":
                    throw new NotImplementedException("C246AEBD-65C9-498C-8B51-9E765E901493");

                case "System.Double":
                    throw new NotImplementedException("C240B14D-831C-42E2-B3EB-AB59C8AE69C3");

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
