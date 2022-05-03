using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class AdvIrregularComparisonSource: BaseIrregularComparisonSource
    {
        public AdvIrregularComparisonSource()
            : base(@"Resources\MyDicts\adv.txt")
        {
        }
    }
}
