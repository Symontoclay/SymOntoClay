using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class NounAntiStemmer : BaseAntiStemmer
    {
        public NounAntiStemmer()
            : base(new NounExceptionCasesWordNetSource())
        {
        }

        public string GetPluralForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm baseForm = {baseForm}");
#endif

            var lastChar = baseForm.Last();

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm lastChar = {lastChar}");
#endif

            var multipleForm = string.Empty;

            switch(lastChar)
            {
                case 'h':
                case 'j':
                case 's':
                case 'x':
                case 'z':
                    multipleForm = $"{baseForm}es";
                    break;

                default:
                    multipleForm = $"{baseForm}s";
                    break;
            }

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm multipleForm = {multipleForm}");
#endif

            var listOfExeptWords = GetExceptionsList(baseForm);

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm listOfExeptWords.Count = {listOfExeptWords.Count}");
            //foreach (var exceptWord in listOfExeptWords)
            //{
            //    NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm exceptWord = {exceptWord}");
            //}
#endif
            if (listOfExeptWords.Count == 0)
            {
                return multipleForm;
            }

            var edEndsWord = listOfExeptWords.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(edEndsWord))
            {
                return edEndsWord;
            }

            return multipleForm;
        }

        public string GetPossesiveForSingular(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPossesiveForSingular baseForm = {baseForm}");
#endif

            var lastChar = baseForm.Last();

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPossesiveForSingular lastChar = {lastChar}");
#endif

            if(lastChar == '\'')
            {
                return $"{baseForm}s";
            }

            return $"{baseForm}'s";
        }

        public string GetPossesiveForPlural(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPossesiveForPlural baseForm = {baseForm}");
#endif

            var lastChar = baseForm.Last();

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPossesiveForPlural lastChar = {lastChar}");
#endif

            if (lastChar == '\'')
            {
                return baseForm;
            }

            return $"{baseForm}'";
        }
    }
}
