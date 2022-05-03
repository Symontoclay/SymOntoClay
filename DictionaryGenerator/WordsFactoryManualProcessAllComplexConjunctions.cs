using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void ProcessAllComplexConjunctions()
        {
            var word = "even_though";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Concession
            });

            word = "only_if";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "provided_that";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "assuming_that";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "even_if";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "in_case";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "in_case_that";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Condition
            });

            word = "rather_than";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Comparison
            });

            word = "as_much_as";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Comparison
            });

            word = "as_long_as";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "as_soon_as";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "by_the_time";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "now_that";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Time
            });

            word = "so_that";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "in_order";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "in_order_that";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Reason
            });

            word = "as_though";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Manner
            });

            word = "as_if";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Subordinating,
                SecondKind = SecondKindOfConjunction.Manner
            });

            word = "as...as";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "as";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "just_as...so";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "just_as";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "so";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "both...and";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "both";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "and";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "hardly...when";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "hardly";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "when";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "scarcely...when";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "scarcely";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "when";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "either...or";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "either";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "or";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "neither...nor";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative,
                IsNegation = true
            });

            word = "neither";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart,
                IsNegation = true
            });

            word = "nor";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart,
                IsNegation = true
            });

            word = "if...then";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "if";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "then";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "not...but";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative,
                IsNegation = true
            });

            word = "not";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart,
                IsNegation = true
            });

            word = "but";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart,
                IsNegation = true
            });

            word = "what_with...and";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "what_with";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "and";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "whether...or";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "whether";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "or";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "not_only...but_also";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "not_only";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart,
                IsNegation = true
            });

            word = "but_also";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "no_sooner...than";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "no_sooner";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart,
                IsNegation = true
            });

            word = "than";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });

            word = "rather...than";
            rootWord = word;
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FullCorrelative
            });

            word = "rather";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.FirstCorrelativePart
            });

            word = "than";
            AddGrammaticalWordFrame(word, new ConjunctionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Kind = KindOfConjunction.Correlative,
                SecondKind = SecondKindOfConjunction.SecondCorrelativePart
            });
        }
    }
}
