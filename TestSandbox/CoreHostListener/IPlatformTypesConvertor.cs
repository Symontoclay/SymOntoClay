using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public interface IPlatformTypesConvertor
    {
        Type PlatformType { get; }
        Type CoreType { get; }
    }
}
