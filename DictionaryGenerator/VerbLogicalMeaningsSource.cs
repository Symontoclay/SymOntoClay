using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class VerbLogicalMeaningsSource: LogicalMeaningsSource
    {
        public VerbLogicalMeaningsSource()
            : base(@"Resources\LogicalMeanings\verb")
        {
        }
    }
}
