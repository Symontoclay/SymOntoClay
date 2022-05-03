using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class AdvAntiStemmer : BaseCompariconBaseAntiStemmer
    {
        public AdvAntiStemmer()
            : base(new AdvExceptionCasesWordNetSource(), new AdvIrregularComparisonSource())
        {
        }

        public string GetComparativeForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetComparativeForm baseForm = {baseForm}");
#endif

            var comparativeForm = GetIrregularComparativeForm(baseForm);

            if (!string.IsNullOrWhiteSpace(comparativeForm))
            {
                return comparativeForm;
            }

            if(!IsOriginAdv(baseForm))
            {
                return string.Empty;
            }

            return GetStandardComparisonForm(baseForm);
        }

        public string GetSuperlativeForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetSuperlativeForm baseForm = {baseForm}");
#endif

            var superlativeForm = GetIrregularSuperlativeForm(baseForm);

            if (!string.IsNullOrWhiteSpace(superlativeForm))
            {
                return superlativeForm;
            }

            if (!IsOriginAdv(baseForm))
            {
                return string.Empty;
            }

            return GetStandardSuperlativeForm(baseForm);
        }

        private bool IsOriginAdv(string baseForm)
        {
            if(baseForm == "early")
            {
                return true;
            }

            return !baseForm.EndsWith("ly");
        }
    }
}
