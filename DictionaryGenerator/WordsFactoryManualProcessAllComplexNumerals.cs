/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
        private void ProcessAllComplexNumerals()
        {
            var word = "one-off";
            var value = 1f;
            var rootWord = "one";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord,
                LogicalMeaning = new List<string>()
                {
                    "multiplicating"
                }
            });

            word = "twenty-one";
            value = 21f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "21st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-firsts";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
			
			word = "twenty-two";
            value = 22f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "22nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "twenty-seconds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
			
            word = "twenty-three";
            value = 23f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "23rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "twenty-thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
			
			word = "twenty-four";
            value = 24f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "24th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "twenty-fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
			
			word = "twenty-five";
            value = 25f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "25th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "twenty-fifths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
			
            word = "twenty-six";
            value = 26f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "26th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "twenty-sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

			word = "twenty-seven";
            value = 27f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "27th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "twenty-sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
			
			word = "twenty-eight";
            value = 28f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "28th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "twenty-eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
			
            word = "twenty-nine";
            value = 29f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "29th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "twenty-ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "twenty-ninths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-one";
            value = 31f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "31st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "thirty-firsts";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-two";
            value = 32f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "32nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "thirty-seconds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-three";
            value = 33f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "33rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "thirty-thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });	

            word = "thirty-four";
            value = 34f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "34th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "thirty-fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });	

            word = "thirty-five";
            value = 35f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "35th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "thirty-fifths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-six";
            value = 36f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "36th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "thirty-sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-seven";
            value = 37f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "37th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            }); 
			
			word = "thirty-sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-eight";
            value = 38f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "38th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-nine";
            value = 39f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "39th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "thirty-ninths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-one";
            value = 41f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "41st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-firsts";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-two";
            value = 42f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "42nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-seconds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-three";
            value = 43f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "43rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-four";
            value = 44f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "44th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-five";
            value = 45f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "45th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-fifths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-six";
            value = 46f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "46th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-seven";
            value = 47f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "47th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-eight";
            value = 48f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "48th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-nine";
            value = 49f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "49th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "forty-ninths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-one";
            value = 51f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "51st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-firsts";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-two";
            value = 52f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "52nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-seconds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-three";
            value = 53f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "53rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-four";
            value = 54f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "54th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-five";
            value = 55f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "55th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-fifths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-six";
            value = 56f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "56th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-seven";
            value = 57f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "57th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-eight";
            value = 58f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "58th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-nine";
            value = 59f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "59th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "fifty-ninths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-one";
            value = 61f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "61st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-firsts";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-two";
            value = 62f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "62nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-seconds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-three";
            value = 63f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "63rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-four";
            value = 64f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "64th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-five";
            value = 65f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "65th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-fifths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-six";
            value = 66f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "66th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-seven";
            value = 67f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "67th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-eight";
            value = 68f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "68th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-nine";
            value = 69f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "69th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "sixty-ninths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-one";
            value = 71f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "71st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-firsts";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-two";
            value = 72f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "72nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-seconds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-three";
            value = 73f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "73rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-four";
            value = 74f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "74th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-five";
            value = 75f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "75th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-fifths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
 
            word = "seventy-six";
            value = 76f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "76th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-seven";
            value = 77f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "77th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-eight";
            value = 78f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "78th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-nine";
            value = 79f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "79th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "seventy-ninths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-one";
            value = 81f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "81st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-firsts";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-two";
            value = 82f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "82nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-seconds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-three";
            value = 83f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "83rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-four";
            value = 84f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "84th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-five";
            value = 85f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "85th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-fifths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-six";
            value = 86f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "86th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-seven";
            value = 87f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "87th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-eight";
            value = 88f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "88th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-nine";
            value = 89f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "89th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "eighty-ninths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-one";
            value = 91f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "91st";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-first";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-firsts";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-two";
            value = 92f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "92nd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-second";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-seconds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-three";
            value = 93f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "93rd";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-third";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-thirds";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-four";
            value = 94f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "94th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-fourth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-fourths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-five";
            value = 95f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "95th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-fifth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-fifths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-six";
            value = 96f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "96th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-sixth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-sixths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-seven";
            value = 97f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "97th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-seventh";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-sevenths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-eight";
            value = 98f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "98th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-eighth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-eighths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-nine";
            value = 99f;
            rootWord = word;
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Cardinal,
                RepresentedNumber = value
            });

            word = "99th";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-ninth";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });

            word = "ninety-ninths";
            AddGrammaticalWordFrame(word, new NumeralGrammaticalWordFrame()
            {
                NumeralType = NumeralType.Ordinal,
                RepresentedNumber = value,
                RootWord = rootWord
            });
        }
    }
}
