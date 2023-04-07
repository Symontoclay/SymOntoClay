/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

            var listOfExeptWords = GetExceptionsList(baseForm);

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
            var lastChar = baseForm.Last();
            var targetBaseForm = baseForm;

            if (lastChar == 'e')
            {
                targetBaseForm = baseForm.Remove(baseForm.Length - 1);
            }

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
            var lastChar = baseForm.Last();

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
