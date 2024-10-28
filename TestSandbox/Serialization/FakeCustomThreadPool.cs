using SymOntoClay.Serialization;
using System;
using System.Threading;

namespace TestSandbox.Serialization
{
    public class FakeCustomThreadPool: IFakeCustomThreadPool//, IConvertible
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public FakeCustomThreadPool() { }
        public FakeCustomThreadPool(int minThreadsCount, int maxThreadsCount, CancellationToken cancellationToken) { }

//        TypeCode IConvertible.GetTypeCode()
//        {
//            throw new NotImplementedException("B2432AB1-E60A-4491-B4DD-4C02134F2E4E");
//        }

//        bool IConvertible.ToBoolean(IFormatProvider provider)
//        {
//            throw new NotImplementedException("F005A989-4AB5-4340-B9A0-AF9820408D00");
//        }

//        byte IConvertible.ToByte(IFormatProvider provider)
//        {
//            throw new NotImplementedException("0D7D55DA-7909-448C-A74E-A0C566C10990");
//        }

//        char IConvertible.ToChar(IFormatProvider provider)
//        {
//            throw new NotImplementedException("446FEDF5-522D-4C01-B983-A2B05D4A98A2");
//        }

//        DateTime IConvertible.ToDateTime(IFormatProvider provider)
//        {
//            throw new NotImplementedException("DD61B935-72A6-4C55-B81D-28E3C2A5B217");
//        }

//        decimal IConvertible.ToDecimal(IFormatProvider provider)
//        {
//            throw new NotImplementedException("6A4BB1EE-E48F-452D-A792-F317063C4D67");
//        }

//        double IConvertible.ToDouble(IFormatProvider provider)
//        {
//            throw new NotImplementedException("3BA09C7C-2826-4610-9030-63426C982A81");
//        }

//        short IConvertible.ToInt16(IFormatProvider provider)
//        {
//            throw new NotImplementedException("201267AF-B50C-42C2-B625-F531F17B9DEC");
//        }

//        int IConvertible.ToInt32(IFormatProvider provider)
//        {
//            throw new NotImplementedException("58100138-4BF4-4B2E-BFE6-F50D98964E17");
//        }

//        long IConvertible.ToInt64(IFormatProvider provider)
//        {
//            throw new NotImplementedException("07D14C9E-ADB3-41ED-952B-122CA151FF6E");
//        }

//        sbyte IConvertible.ToSByte(IFormatProvider provider)
//        {
//            throw new NotImplementedException("A3686942-5C86-4083-B28D-6D035F32EC0D");
//        }

//        float IConvertible.ToSingle(IFormatProvider provider)
//        {
//            throw new NotImplementedException("E3BB73CC-7AAD-4EEC-B24B-C1161A7A0464");
//        }

//        string IConvertible.ToString(IFormatProvider provider)
//        {
//            throw new NotImplementedException("F1D0B833-8270-41AB-A9BA-07518B50A849");
//        }

//        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
//        {
//#if DEBUG
//            _logger.Info($"conversionType.FullName = {conversionType.FullName}");
//#endif

//            if(conversionType == typeof(IFakeCustomThreadPool))
//            {
//                return (IFakeCustomThreadPool)this;
//            }

//            throw new NotImplementedException("2E76924E-1BD7-4D52-B7BC-D58E7841C022");
//        }

//        ushort IConvertible.ToUInt16(IFormatProvider provider)
//        {
//            throw new NotImplementedException("B7049DEB-FABB-41A1-B41E-7432ECA14788");
//        }

//        uint IConvertible.ToUInt32(IFormatProvider provider)
//        {
//            throw new NotImplementedException("94D8A09A-DB76-4497-B511-8ED0E4001CCB");
//        }

//        ulong IConvertible.ToUInt64(IFormatProvider provider)
//        {
//            throw new NotImplementedException("FE70F09A-8BDA-4102-8BFA-A389D84BD4A9");
//        }
    }
}
