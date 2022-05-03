using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void ProcessAllConjunctions()
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info("Begin ProcessAllConjunctions");
#endif

            var word = "and";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "but";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "for";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "nor";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "or";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "so";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "yet";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Coordinating
            });

            word = "though";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Concession
            });

            word = "although";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Concession
            });

            word = "while";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Concession
            });

            word = "if";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "unless";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "until";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "lest";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "than";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Comparison
            });

            word = "whether";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Comparison
            });

            word = "whereas";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Comparison
            });

            word = "after";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "before";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "once";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "since";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "till";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "until";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "when";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "whenever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "while";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "because";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "since";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "why";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "that";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective
            });

            word = "what";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective,
                IsQuestionWord = true
            });

            word = "whatever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective,
                IsQuestionWord = true
            });

            word = "which";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective,
                IsQuestionWord = true
            });

            word = "whichever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Adjective,
                IsQuestionWord = true
            });

            word = "who";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "whoever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "whom";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "whomever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "whose";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.RelativePronoun,
                IsQuestionWord = true
            });

            word = "how";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Manner,
                IsQuestionWord = true
            });

            word = "where";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Place,
                IsQuestionWord = true
            });

            word = "wherever";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Place,
                IsQuestionWord = true
            });
        }
    }
}
