using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public interface IRootWordBasedMetrixOfSourceWordNet
    {
        string Word { get; set; }
        bool IsDigit { get; set; }
        bool IsName { get; set; }
        bool IsComplex { get; set; }
    }
}
