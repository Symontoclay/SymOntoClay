using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class VerbAntiStemmer: BaseAntiStemmer
    {
        public VerbAntiStemmer()
            : base(new VerbsExceptionCasesWordNetSource())
        {
            var irregularVerbsSource = new IrregularVerbsSource();
            var irregularItemsList = irregularVerbsSource.ReadAll();

            foreach(var irregularItem in irregularItemsList)
            {
                var rootWord = irregularItem.RootWord;

                var pastFormsList = irregularItem.PastForm.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                mIrregularPastFormsDict[rootWord] = pastFormsList;

                var particleFormsList = irregularItem.ParticleForm.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                mIrregularParticlesDict[rootWord] = particleFormsList;
            }
        }

        private Dictionary<string, List<string>> mIrregularPastFormsDict = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> mIrregularParticlesDict = new Dictionary<string, List<string>>();

        private List<string> GetIrregularPastForms(string baseForm)
        {
            if(mIrregularPastFormsDict.ContainsKey(baseForm))
            {
                return mIrregularPastFormsDict[baseForm];
            }

            return new List<string>();
        }

        private List<string> GetIrregularParticleForms(string baseForm)
        {
            if (mIrregularParticlesDict.ContainsKey(baseForm))
            {
                return mIrregularParticlesDict[baseForm];
            }

            return new List<string>();
        }

        public List<string> GetPastForms(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPastForms baseForm = {baseForm}");
#endif

            if(baseForm == "be")
            {
                throw new NotSupportedException("The verb `be` is a very special word");
            }

            var irregularFormsList = GetIrregularPastForms(baseForm);

            if(irregularFormsList.Count > 0)
            {
                return irregularFormsList;
            }

            return new List<string>() { GetRegularPastOrParticleForm(baseForm) };
        }

        public List<string> GetParticleForms(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetParticleForms baseForm = {baseForm}");
#endif

            if (baseForm == "be")
            {
                throw new NotSupportedException("The verb `be` is a very special word");
            }

            var irregularFormsList = GetIrregularParticleForms(baseForm);

            if (irregularFormsList.Count > 0)
            {
                return irregularFormsList;
            }

            return new List<string>() { GetRegularPastOrParticleForm(baseForm) };
        }

        private string GetRegularPastOrParticleForm(string baseForm)
        {
            var lastChar = baseForm.Last();
            var pastForm = string.Empty;

            if (lastChar == 'e')
            {
                pastForm = $"{baseForm}d";
            }
            else
            {
                pastForm = $"{baseForm}ed";
            }

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPastForm pastForm = {pastForm}");
#endif

            var listOfExeptWords = GetExceptionsList(baseForm);

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPastForm listOfExeptWords.Count = {listOfExeptWords.Count}");
            //foreach (var exceptWord in listOfExeptWords)
            //{
            //    NLog.LogManager.GetCurrentClassLogger().Info($"GetPastForm exceptWord = {exceptWord}");
            //}
#endif

            if (listOfExeptWords.Count == 0)
            {
                return pastForm;
            }

            var edEndsWord = listOfExeptWords.Where(p => p.EndsWith("ed")).FirstOrDefault();

            if(!string.IsNullOrWhiteSpace(edEndsWord))
            {
                return edEndsWord;
            }

            return pastForm;
        }

        public string GetIngForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetIngForm baseForm = {baseForm}");
#endif

            var lastChar = baseForm.Last();
            var targetBaseForm = baseForm;

            if (lastChar == 'e')
            {
                targetBaseForm = baseForm.Remove(baseForm.Length - 1);
            }

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetIngForm after baseForm = {baseForm}");
#endif

            var ingForm = $"{targetBaseForm}ing";

            var listOfExeptWords = GetExceptionsList(baseForm);

            if (listOfExeptWords.Count == 0)
            {
                return ingForm;
            }

            var ingEndsWord = listOfExeptWords.Where(p => p.EndsWith("ing")).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(ingEndsWord))
            {
                return ingEndsWord;
            }

            return ingForm;
        }

        public string GetThirdPersonSingularPresent(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetThirdPersonSingularPresent baseForm = {baseForm}");
#endif
            var lastChar = baseForm.Last();

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetThirdPersonSingularPresent lastChar = {lastChar}");
#endif

            var targetForm = string.Empty;

            switch(lastChar)
            {
                case 'h':
                case 'o':
                case 'p':
                case 's':
                case 'z':
                    targetForm = $"{baseForm}es";
                    break;

                case 'y':
                    var modifiedBaseForm = baseForm.Remove(baseForm.Length - 1);
                    targetForm = $"{modifiedBaseForm}es";
                    break;

                default:
                    targetForm = $"{baseForm}s";
                    break;
            }

            var listOfExeptWords = GetExceptionsList(baseForm);

            if (listOfExeptWords.Count == 0)
            {
                return targetForm;
            }

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetThirdPersonSingularPresent listOfExeptWords.Count = {listOfExeptWords.Count}");
            //foreach (var exceptWord in listOfExeptWords)
            //{
            //    NLog.LogManager.GetCurrentClassLogger().Info($"GetThirdPersonSingularPresent exceptWord = {exceptWord}");
            //}
#endif

            var sEndsWord = listOfExeptWords.Where(p => p.EndsWith("s")).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(sEndsWord))
            {
                if(!GetIrregularPastForms(baseForm).Contains(sEndsWord) && !GetIrregularParticleForms(baseForm).Contains(sEndsWord))
                {
                    return sEndsWord;
                }         
            }

            return targetForm;
        }
    }
}
