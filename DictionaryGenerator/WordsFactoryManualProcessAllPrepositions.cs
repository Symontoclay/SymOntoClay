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

using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private void ProcessAllPrepositions()
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info("Begin ProcessAllPrepositions");
#endif

            var word = "aboard";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "about";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "above";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "absent";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "across";
            var rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "cross";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "after";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "against";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'gainst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "gainst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsPoetic = true
            });

            word = "again";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "gain";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "along";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'long";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "alongst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "alongside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "amid";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "amidst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "mid";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "midst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsPoetic = true
            });

            word = "among";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "amongst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "'mong";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "mong";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "'mongst";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "apropos";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                IsRare = true
            });

            word = "apud";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "around";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'round";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "round";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "as";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "astride";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "at";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "@";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "atop";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ontop";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "bar";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "before";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "afore";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "tofore";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "B4";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "behind";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ahind";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "below";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ablow";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "allow";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsDialectal = true
            });

            word = "beneath";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'neath";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "neath";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsPoetic = true
            });

            word = "beside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "besides";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "between";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "atween";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "beyond";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "ayond ";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "but";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "by";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "chez";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                IsRare = true
            });

            word = "circa";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "c.";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "ca.";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "come";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "dehors";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "despite";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "spite";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "down";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "during";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "except";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "for";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "4";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "from";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "in";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "inside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "into";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "less";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "like";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "minus";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "near";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "nearer";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Comparison = GrammaticalComparison.Comparative
            });

            word = "nearest ";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Comparison = GrammaticalComparison.Superlative
            });

            word = "anear";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "notwithstanding";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "of";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "o'";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsPoetic = true,
                IsDialectal = true
            });

            word = "off";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "on";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "onto";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "opposite";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "out";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "outen";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true,
                IsDialectal = true
            });

            word = "outside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "over";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "o'er";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsPoetic = true
            });

            word = "pace";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "past";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "per";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "plus";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "post";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "pre";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "pro";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "qua";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "re";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "sans";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "save";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "sauf";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "short";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "since";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "sithence";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "than";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "through";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "thru";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "throughout";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "thruout";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "till";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "to";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                ConditionalLogicalMeaning = new Dictionary<string, IList<string>>()
                {
                    {
                        "go", new List<string>() {
                            "direction"
                        }
                    }
                }
            });

            word = "2";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "toward";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "towards";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord
            });

            word = "under";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "underneath";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "unlike";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "until";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'til";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "til";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "unto";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "up";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "upon";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "'pon";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "pon";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "upside";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "versus";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "vs.";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "v.";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "via";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "vice";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "vis-à-vis";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "with";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "w/";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "wi'";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "c̄";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "within";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "w/i";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "without";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "w/o";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsAbbreviation = true
            });

            word = "worth";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            word = "next";
            AddGrammaticalWordFrame(word, new PrepositionGrammaticalWordFrame()
            {
            });

            //------
            word = "ago";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "apart";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "aside";
            rootWord = word;
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "aslant";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
                RootWord = rootWord,
                IsArchaic = true
            });

            word = "away";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "hence";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
            });

            word = "withal";
            AddGrammaticalWordFrame(word, new PostpositionGrammaticalWordFrame()
            {
                IsArchaic = true
            });
        }
    }
}
