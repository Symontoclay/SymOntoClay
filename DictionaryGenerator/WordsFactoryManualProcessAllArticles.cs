using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void ProcessAllArticles()
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info("Begin ProcessAllArticles");
#endif

            var word = "the";
            AddGrammaticalWordFrame(word, new ArticleGrammaticalWordFrame()
            {
                Kind = KindOfArticle.Definite
            });

            word = "a";
            AddGrammaticalWordFrame(word, new ArticleGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                Kind = KindOfArticle.Indefinite
            });

            word = "an";
            AddGrammaticalWordFrame(word, new ArticleGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                Kind = KindOfArticle.Indefinite
            });

            word = "no";
            AddGrammaticalWordFrame(word, new ArticleGrammaticalWordFrame()
            {
                Kind = KindOfArticle.Negative
            });
        }
    }
}
