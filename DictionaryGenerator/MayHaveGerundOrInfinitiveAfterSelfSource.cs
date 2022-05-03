using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class MayHaveGerundOrInfinitiveAfterSelfSource: LogicalMeaningsSourceForOneMeaningByRelativePath
    {
        public MayHaveGerundOrInfinitiveAfterSelfSource()
            : base(@"Resources\MyDicts\MayHaveGerundOrInfinitiveAfterSelf.txt")
        {
        }
    }
}
