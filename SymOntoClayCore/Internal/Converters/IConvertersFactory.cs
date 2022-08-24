using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Converters
{
    public interface IConvertersFactory
    {
        ConverterFactToImperativeCode GetConverterFactToImperativeCode();
    }
}
