using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Dicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClayBaseTestLib
{
    public static class DictionaryInstance
    {
        static DictionaryInstance()
        {
            var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

            Instance = new JsonDictionary(mainDictPath);
        }

        public static IWordsDict Instance { get; private set; }
    }
}
