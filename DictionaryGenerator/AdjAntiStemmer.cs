using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class AdjAntiStemmer : BaseCompariconBaseAntiStemmer
    {
        public AdjAntiStemmer()
            : base(new AdjExceptionCasesWordNetSource(), new AdjIrregularComparisonSource())
        {
        }

        public string GetComparativeForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetComparativeForm baseForm = {baseForm}");
#endif

            var comparativeForm = GetIrregularComparativeForm(baseForm);

            if(!string.IsNullOrWhiteSpace(comparativeForm))
            {
                return comparativeForm;
            }

            return GetStandardComparisonForm(baseForm);
        }

        public string GetSuperlativeForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetSuperlativeForm baseForm = {baseForm}");
#endif
            var superlativeForm = GetIrregularSuperlativeForm(baseForm);

            if(!string.IsNullOrWhiteSpace(superlativeForm))
            {
                return superlativeForm;
            }

            return GetStandardSuperlativeForm(baseForm);
        }
    }
}
