using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class AdvLogicalMeaningsSource : LogicalMeaningsSource
    {
        public AdvLogicalMeaningsSource()
            : base(@"Resources\LogicalMeanings\adv")
        {
        }
    }
}
