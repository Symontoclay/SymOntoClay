﻿using NUnit.Framework;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.NLP.ATN
{
    public class ConverterToPlainSentences_Tests
    {
        [SetUp]
        public void Setup()
        {
            _logger = new EmptyLogger();
            _wordsDict = DictionaryInstance.Instance;
        }

        private IEntityLogger _logger;
        private IWordsDict _wordsDict;

        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = "I like my cat.";

            var parser = new ATNParser(_logger, _wordsDict);

            var result = parser.Run(text);

            Assert.AreEqual(1, result.Count);

            var item = result[0];

            var compactizer = new PhraseCompactizer(_logger, _wordsDict);

            compactizer.Run(item);            

            var converterToPlainSentences = new ConverterToPlainSentences(_logger);

            var plainSentencesList = converterToPlainSentences.Run(item);

            Assert.AreEqual(1, plainSentencesList.Count);

            var plainSentence = plainSentencesList[0];

            Assert.AreEqual(@"S
    Subject:
        NP
            N:
                I
    Predicate:
        VP
            V:
                like
            Object:
                NP
                    N:
                        cat
                    D:
                        my", plainSentence.ToDbgString().Trim());
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = "Go to green place!";

            var parser = new ATNParser(_logger, _wordsDict);

            var result = parser.Run(text);

            Assert.AreEqual(1, result.Count);

            var item = result[0];

            var compactizer = new PhraseCompactizer(_logger, _wordsDict);

            compactizer.Run(item);

            var converterToPlainSentences = new ConverterToPlainSentences(_logger);

            var plainSentencesList = converterToPlainSentences.Run(item);

            Assert.AreEqual(1, plainSentencesList.Count);

            var plainSentence = plainSentencesList[0];

            Assert.AreEqual(@"S:(Imperative)
    Predicate:
        VP
            V:
                Go
            PP:
                PP
                    :P
                        to
                    :NP
                        NP
                            N:
                                place
                            AP:
                                green", plainSentence.ToDbgString().Trim());
        }
    }
}
