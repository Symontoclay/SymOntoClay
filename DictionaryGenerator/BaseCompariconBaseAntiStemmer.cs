using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public abstract class BaseCompariconBaseAntiStemmer : BaseAntiStemmer
    {
        protected BaseCompariconBaseAntiStemmer(BaseExceptionCasesWordNetSource exceptionCasesSource, BaseIrregularComparisonSource irregularComparisonSource)
            : base(exceptionCasesSource)
        {
            var irregularComparisonList = irregularComparisonSource.ReadAll();

            foreach(var irregularComparison in irregularComparisonList)
            {
                var rootWord = irregularComparison.BaseWord;

                mIrregularComparativeDict[rootWord] = irregularComparison.ComparativeForm;
                mIrregularSuperlativeDict[rootWord] = irregularComparison.SuperlativeForm;
            }
        }

        private Dictionary<string, string> mIrregularComparativeDict = new Dictionary<string, string>();
        private Dictionary<string, string> mIrregularSuperlativeDict = new Dictionary<string, string>();

        protected string GetIrregularComparativeForm(string baseForm)
        {
            if(mIrregularComparativeDict.ContainsKey(baseForm))
            {
                return mIrregularComparativeDict[baseForm];
            }

            return string.Empty;
        }

        protected string GetIrregularSuperlativeForm(string baseForm)
        {
            if(mIrregularSuperlativeDict.ContainsKey(baseForm))
            {
                return mIrregularSuperlativeDict[baseForm];
            }

            return string.Empty;
        }

        protected string GetStandardComparisonForm(string baseForm)
        {
            var modifiedBaseForm = baseForm;

            var lastChar = baseForm.Last();

            if (lastChar == 'y')
            {
                modifiedBaseForm = $"{baseForm.Remove(baseForm.Length - 1)}i";
            }

            var comparativeForm = $"{modifiedBaseForm}er";

            var listOfExeptWords = GetExceptionsList(baseForm);
            var exceptWord = listOfExeptWords.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(exceptWord))
            {
                return exceptWord;
            }

            return comparativeForm;
        }

        protected string GetStandardSuperlativeForm(string baseForm)
        {
            var modifiedBaseForm = baseForm;

            var lastChar = baseForm.Last();

            if (lastChar == 'y')
            {
                modifiedBaseForm = $"{baseForm.Remove(baseForm.Length - 1)}i";
            }

            var superlativeForm = $"{modifiedBaseForm}est";

            var listOfExeptWords = GetExceptionsList(baseForm);
            var exceptWord = listOfExeptWords.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(exceptWord))
            {
                return exceptWord;
            }

            return superlativeForm;
        }
    }
}
