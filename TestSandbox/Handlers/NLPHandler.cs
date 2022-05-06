using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Dicts;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.NLP;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class NLPHandler
    {
        public NLPHandler()
        {
            _engineContext = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

#if DEBUG
            _logger.Log($"mainDictPath = {mainDictPath}");
#endif

            _wordsDict = new JsonDictionary(mainDictPath);

            _converter = new NLPConverter(_engineContext, _wordsDict);
        }

        private readonly EngineContext _engineContext;
        private readonly INLPConverter _converter;
        private readonly IWordsDict _wordsDict;
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Case2();
            //Case1();

            _logger.Log("End");
        }

        private void Case2()
        {
            //var wordsDict = new TstSimpleWordsDict();

            var text = "I like my cat.";

#if DEBUG
            _logger.Log($"text = {text}");
#endif

            //var globalContext = new GlobalParserContext(_engineContext, wordsDict, text);

            //globalContext.Run();

            var parser = new ATNParser(_engineContext, _wordsDict);

            var result = parser.Run(text);

            _logger.Log($"result.Count = {result.Count}");

            foreach (var item in result)
            {
                _logger.Log($"item = {item.ToDbgString()}");
            }
        }

        private void Case1()
        {
            var result = _converter.Convert("I like my cat.()!.?,...:;-1234567890 M1$nrg, #erty3, @maror, 3% can't");

            _logger.Log($"result.Count = {result.Count}");

            foreach(var item in result)
            {
                _logger.Log($"item = {item.ToHumanizedString()}");
            }
        }
    }
}
