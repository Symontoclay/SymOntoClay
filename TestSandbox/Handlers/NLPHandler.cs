using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
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

            var wordsDict = new TstSimpleWordsDict();

            _converter = new NLPConverter(_engineContext, wordsDict);
        }

        private readonly EngineContext _engineContext;
        private readonly INLPConverter _converter;
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
            var wordsDict = new TstSimpleWordsDict();

            var text = "I like my cat.";

            var globalContext = new GlobalParserContext(_engineContext, wordsDict, text);

            globalContext.Run();

            //var parser = new ATNParser(_engineContext, wordsDict);

            //var result = parser.Run(text);

            //_logger.Log($"result.Count = {result.Count}");

            //foreach (var item in result)
            //{
            //    _logger.Log($"item = {item}");
            //}
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
