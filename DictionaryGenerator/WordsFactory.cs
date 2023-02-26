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

using NLog;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using SymOntoClay.CoreHelper.DebugHelpers;
using System.Text.RegularExpressions;
using SymOntoClay.NLP.CommonDict.Implementations;

namespace DictionaryGenerator
{
    public partial class WordsFactory
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public WordsFactory(string nameOfMainDict = "main.dict", string nameOfNamesDict = "names.dict")
        {
            mNameOfMainDict = nameOfMainDict;
            mNameOfNamesDict = nameOfNamesDict;

            mRootNounsSource = new RootNounsWordNetSource();
            mRootVerbsSource = new RootVerbsWordNetSource();
            mRootAdjSource = new RootAdjWordNetSource();
            mRootAdvSource = new RootAdvWordNetSource();

            mNounAntiStemmer = new NounAntiStemmer();
            mVerbAntiStemmer = new VerbAntiStemmer();
            mAdjAntiStemmer = new AdjAntiStemmer();
            mAdvAntiStemmer = new AdvAntiStemmer();

            mMayHaveGerundOrInfinitiveAfterSelfSource = new MayHaveGerundOrInfinitiveAfterSelfSource();

            mVerbLogicalMeaningsSource = new VerbLogicalMeaningsSource();

            mAdjLogicalMeaningsSource = new AdjLogicalMeaningsSource();
            mAdvLogicalMeaningsSource = new AdvLogicalMeaningsSource();
        }

        private string mNameOfMainDict;
        private string mNameOfNamesDict;

        private RootNounsWordNetSource mRootNounsSource;
        private RootVerbsWordNetSource mRootVerbsSource;
        private RootAdjWordNetSource mRootAdjSource;
        private RootAdvWordNetSource mRootAdvSource;

        private NounAntiStemmer mNounAntiStemmer;
        private VerbAntiStemmer mVerbAntiStemmer;
        private AdjAntiStemmer mAdjAntiStemmer;
        private AdvAntiStemmer mAdvAntiStemmer;

        private MayHaveGerundOrInfinitiveAfterSelfSource mMayHaveGerundOrInfinitiveAfterSelfSource;

        private VerbLogicalMeaningsSource mVerbLogicalMeaningsSource;
        private AdjLogicalMeaningsSource mAdjLogicalMeaningsSource;
        private AdvLogicalMeaningsSource mAdvLogicalMeaningsSource;

        private Dictionary<string, List<RootNounSourceWordItem>> mRootNounDict;
        private Dictionary<string, List<RootVerbSourceWordItem>> mRootVerbsDict;
        private Dictionary<string, List<RootAdjSourceWordItem>> mRootAdjsDict;
        private Dictionary<string, List<RootAdvSourceWordItem>> mRootAdvsDict;

        private WordSplitter mWordSplitter;

        //private Dictionary<string, List<string>> mNounClassesDict;

        private int mTotalCount;

        private InternalWordsDictData mWordsDictData;
        private InternalWordsDictData mWordsDictDataOfName;

        private List<string> mTargetWordsList;

        public void Run(List<string> targetWordsList = null)
        {
            mTargetWordsList = targetWordsList;

#if DEBUG
            _logger.Info($"mTargetWordsList.Count = {mTargetWordsList?.Count}");
#endif

            var usualWordsList = new List<string>();
            var namesWordsList = new List<string>();
            var digitsWordsList = new List<string>();
            var complexWordsList = new List<string>();

            var rootNounsList = mRootNounsSource.ReadAll();

            //var rootNounClassesFactory = new RootNounClassesFactory(rootNounsList);
            //mNounClassesDict = rootNounClassesFactory.Result;

#if DEBUG
            _logger.Info($"rootNounsList.Count = {rootNounsList.Count}");
#endif
            ReaderOfRootSourceWordItemHelper.SeparateWords(rootNounsList, ref usualWordsList, ref namesWordsList, ref digitsWordsList, ref complexWordsList);

#if DEBUG
            _logger.Info($"TSTSeparateWords usualWordsList.Count = {usualWordsList.Count}");
            _logger.Info($"TSTSeparateWords namesWordsList.Count = {namesWordsList.Count}");
            _logger.Info($"TSTSeparateWords digitsWordsList.Count = {digitsWordsList.Count}");
            _logger.Info($"TSTSeparateWords complexWordsList.Count = {complexWordsList.Count}");
#endif

            mRootNounDict = rootNounsList.GroupBy(p => p.Word).ToDictionary(p => p.Key, p => p.ToList());

            var rootVerbsList = mRootVerbsSource.ReadAll();

            ReaderOfRootSourceWordItemHelper.SeparateWords(rootVerbsList, ref usualWordsList, ref namesWordsList, ref digitsWordsList, ref complexWordsList);

#if DEBUG
            _logger.Info($"TSTSeparateWords usualWordsList.Count = {usualWordsList.Count}");
            _logger.Info($"TSTSeparateWords namesWordsList.Count = {namesWordsList.Count}");
            _logger.Info($"TSTSeparateWords digitsWordsList.Count = {digitsWordsList.Count}");
            _logger.Info($"TSTSeparateWords complexWordsList.Count = {complexWordsList.Count}");
#endif

            mRootVerbsDict = rootVerbsList.GroupBy(p => p.Word).ToDictionary(p => p.Key, p => p.ToList());

            var rootAdjsList = mRootAdjSource.ReadAll();

#if DEBUG
            _logger.Info($"Run rootAdjsList.Count = {rootAdjsList.Count}");
#endif

            ReaderOfRootSourceWordItemHelper.SeparateWords(rootAdjsList, ref usualWordsList, ref namesWordsList, ref digitsWordsList, ref complexWordsList);

#if DEBUG
            _logger.Info($"TSTSeparateWords usualWordsList.Count = {usualWordsList.Count}");
            _logger.Info($"TSTSeparateWords namesWordsList.Count = {namesWordsList.Count}");
            _logger.Info($"TSTSeparateWords digitsWordsList.Count = {digitsWordsList.Count}");
            _logger.Info($"TSTSeparateWords complexWordsList.Count = {complexWordsList.Count}");
#endif

            mRootAdjsDict = rootAdjsList.GroupBy(p => p.Word).ToDictionary(p => p.Key, p => p.ToList());

            var rootAdvsList = mRootAdvSource.ReadAll();

#if DEBUG
            _logger.Info($"Run rootAdvsList.Count = {rootAdvsList.Count}");
#endif

            ReaderOfRootSourceWordItemHelper.SeparateWords(rootAdvsList, ref usualWordsList, ref namesWordsList, ref digitsWordsList, ref complexWordsList);

#if DEBUG
            _logger.Info($"TSTSeparateWords usualWordsList.Count = {usualWordsList.Count}");
            _logger.Info($"TSTSeparateWords namesWordsList.Count = {namesWordsList.Count}");
            _logger.Info($"TSTSeparateWords digitsWordsList.Count = {digitsWordsList.Count}");
            _logger.Info($"TSTSeparateWords complexWordsList.Count = {complexWordsList.Count}");
#endif

            mRootAdvsDict = rootAdvsList.GroupBy(p => p.Word).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"Run usualWordsList.Count = {usualWordsList.Count}");
#endif

            mWordSplitter = new WordSplitter(mRootNounDict, mRootVerbsDict, mRootAdjsDict, mRootAdvsDict);

            mWordsDictData = new InternalWordsDictData();
            mWordsDictData.WordsDict = new Dictionary<string, WordFrame>();
            //mWordsDictData.NamesList = new List<string>();

            mWordsDictDataOfName = new InternalWordsDictData();
            mWordsDictDataOfName.WordsDict = new Dictionary<string, WordFrame>();
            //mWordsDictDataOfName.NamesList = new List<string>();

            var usualTargetWordsList = new List<string>();
            var namesTargetWordsList = new List<string>();

            if (!mTargetWordsList.IsNullOrEmpty())
            {
                SeparateTargetWordsList(ref usualTargetWordsList, ref namesTargetWordsList);

                if(namesTargetWordsList.Count > 0)
                {
                    var notFoundTargetNamesList = namesTargetWordsList.Where(p => !namesWordsList.Contains(p)).Distinct().ToList();

                    if(notFoundTargetNamesList.Count > 0)
                    {
                        namesWordsList.AddRange(notFoundTargetNamesList);
                    }

#if DEBUG
                    //NLog.LogManager.GetCurrentClassLogger().Info($"Run notFoundTargetNamesList.Count = {notFoundTargetNamesList.Count}");
                    //foreach(var notFoundTargetName in notFoundTargetNamesList)
                    //{
                    //    LogInstance.Log($"notFoundTargetName = {notFoundTargetName}");
                    //}
#endif
                }
            }

            AddUsualSpecialWords(ref usualWordsList);

            usualWordsList = usualWordsList.Distinct().ToList();
            namesWordsList = namesWordsList.Distinct().ToList();
            complexWordsList = complexWordsList.Distinct().ToList();

            ProcessUsualWords(usualWordsList);
            ProcessNames(namesWordsList);
            //ProcessDigits(digitsWordsList);
            ProcessComplexPhrases(complexWordsList);

            if (!mTargetWordsList.IsNullOrEmpty())
            {
                if(usualTargetWordsList.Count > 0)
                {
                    var notFoundUsualTargetWordsList = new List<string>();

                    foreach(var usualTargetWord in usualTargetWordsList)
                    {
                        if(mWordsDictData.WordsDict.ContainsKey(usualTargetWord))
                        {
                            continue;
                        }

                        notFoundUsualTargetWordsList.Add(usualTargetWord);
                    }

                    notFoundUsualTargetWordsList = notFoundUsualTargetWordsList.Distinct().ToList();

#if DEBUG
                    _logger.Info($"notFoundUsualTargetWordsList.Count = {notFoundUsualTargetWordsList.Count}");
                    foreach(var notFoundUsualTargetWord in notFoundUsualTargetWordsList)
                    {
                        _logger.Info($"notFoundUsualTargetWord = '{notFoundUsualTargetWord}'");
                    }
#endif
                }
            }

            SetUpWordInGrammaticalFrame(mWordsDictData);
            SetUpWordInGrammaticalFrame(mWordsDictDataOfName);

            var wordsDictDataToExport = ConvertToDictionary(mWordsDictData);
            var wordsDictDataOfNameToExport = ConvertToDictionary(mWordsDictDataOfName);

            SimpleSaveDict(mNameOfMainDict, wordsDictDataToExport);
            SimpleSaveDict(mNameOfNamesDict, wordsDictDataOfNameToExport);

#if DEBUG
            //_logger.Info($"Run mTotalCount = {mTotalCount}");
            _logger.Info($"Run mWordsDictData.WordsDict.Count = {mWordsDictData.WordsDict.Count}");
            //_logger.Info($"Run mWordsDictData = {mWordsDictData}");
            _logger.Info($"Run mWordsDictDataOfName.WordsDict.Count = {mWordsDictDataOfName.WordsDict.Count}");
            //_logger.Info($"Run mWordsDictDataOfName = {mWordsDictDataOfName}");

            var tmpWordFrame = wordsDictDataToExport["ought"];
            _logger.Info($"Run tmpWordFrame = {tmpWordFrame.WriteListToString()}");
            //WordsDictData
            tmpWordFrame = wordsDictDataToExport["go"];
            _logger.Info($"Run tmpWordFrame = {tmpWordFrame.WriteListToString()}");

            tmpWordFrame = wordsDictDataToExport["went"];
            _logger.Info($"Run tmpWordFrame = {tmpWordFrame.WriteListToString()}");

            //tmpWordFrame = mWordsDictDataOfName.WordsDict["Tim"];
            //_logger.Info($"Run tmpWordFrame = {tmpWordFrame}");
#endif
        }

        private void AddSpecialWordToUsualWords(string word, ref List<string> usualWordsList)
        {
            usualWordsList.Add(word);

            if (mTargetWordsList != null)
            {
                if(!mTargetWordsList.Contains(word))
                {
                    mTargetWordsList.Add(word);
                }
            }
        }

        private void AddSpecialWordToRootNounDict(string word)
        {
            if(mRootNounDict.ContainsKey(word))
            {
                return;
            }

            mRootNounDict[word] = new List<RootNounSourceWordItem>();
        }

        private void AddSpecialWordToRootVerbsDict(string word)
        {
            if(mRootVerbsDict.ContainsKey(word))
            {
                return;
            }

            mRootVerbsDict[word] = new List<RootVerbSourceWordItem>();
        }

        private void AddSpecialWordToRootAdjsDict(string word)
        {
            if(mRootAdjsDict.ContainsKey(word))
            {
                return;
            }

            mRootAdjsDict[word] = new List<RootAdjSourceWordItem>();
        }

        private void AddSpecialWordToRootAdvsDict(string word)
        {
            if(mRootAdvsDict.ContainsKey(word))
            {
                return;
            }

            mRootAdvsDict[word] = new List<RootAdvSourceWordItem>();
        }

        private void SeparateTargetWordsList(ref List<string> usualTargetWordsList, ref List<string> namesTargetWordsList)
        {
            if (mTargetWordsList == null)
            {
                return;
            }

            foreach(var targetWord in mTargetWordsList)
            {
                if(char.IsUpper(targetWord[0]))
                {
                    namesTargetWordsList.Add(targetWord);
                    continue;
                }

                usualTargetWordsList.Add(targetWord);
            }
        }

        private void SetUpWordInGrammaticalFrame(InternalWordsDictData dict)
        {
            var wordsDict = dict.WordsDict;

            foreach(var item in wordsDict)
            {
                var wordFrame = item.Value;

                var word = wordFrame.Word;

#if DEBUG
                //_logger.Info($"SimpleSaveDict word = {word}");
#endif

                var grammaticalWordFrames = wordFrame.GrammaticalWordFrames;

                foreach(var grammaticalWordFrame in grammaticalWordFrames)
                {
                    grammaticalWordFrame.Word = word;
                }
            }
        }

        private Dictionary<string, List<BaseGrammaticalWordFrame>> ConvertToDictionary(InternalWordsDictData dict)
        {
            return dict.WordsDict.ToDictionary(p => p.Key, p => p.Value.GrammaticalWordFrames);
        }

        private void SimpleSaveDict(string localPath, Dictionary<string, List<BaseGrammaticalWordFrame>> dict)
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;

#if DEBUG
            _logger.Info($"SimpleSaveDict rootPath = {rootPath}");
#endif

            var fullPath = Path.Combine(rootPath, localPath);

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"SimpleSaveDict fullPath = {fullPath}");
#endif
            var serializationEngine = new WordsDictJSONSerializationEngine();//new WordsDictSerializationEngine();
            serializationEngine.SaveToFile(dict, fullPath);

            //localPath = $"{localPath}.fs";

            //fullPath = Path.Combine(rootPath, localPath);

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"SimpleSaveDict fullPath = {fullPath}");
#endif

            //var jsonSerializer = new DataContractJsonSerializer(typeof(WordsDictData), new List<Type> {
            //    typeof(PronounGrammaticalWordFrame),
            //    typeof(AdjectiveGrammaticalWordFrame),
            //    typeof(AdverbGrammaticalWordFrame),
            //    typeof(ArticleGrammaticalWordFrame),
            //    typeof(ConjunctionGrammaticalWordFrame),
            //    typeof(NounGrammaticalWordFrame),
            //    typeof(NumeralGrammaticalWordFrame),
            //    typeof(PostpositionGrammaticalWordFrame),
            //    typeof(PrepositionGrammaticalWordFrame),
            //    typeof(PronounGrammaticalWordFrame),
            //    typeof(VerbGrammaticalWordFrame)
            //});

            //if (File.Exists(fullPath))
            //{
            //    File.Delete(fullPath);
            //}

            //using (var fs = File.OpenWrite(fullPath))
            //{
            //    jsonSerializer.WriteObject(fs, dict);
            //    fs.Flush();
            //}

            //if(File.Exists(fullPath))
            //{
            //    File.Delete(fullPath);
            //}

            //var binaryFormatter = new BinaryFormatter();

            //using (var fs = File.OpenWrite(fullPath))
            //{
            //    binaryFormatter.Serialize(fs, dict);
            //    fs.Flush();
            //}
        }

        /*
         VERBS THAT ARE NOT USUALLY USED IN THE CONTINUOUS FORM
The verbs in the list below are normally used in the simple form because they refer to states, rather than actions or processes.

SENSES / PERCEPTION
to feel*
to hear
to see*
to smell
to taste
OPINION
to assume
to believe
to consider
to doubt
to feel (= to think)
to find (= to consider)
to suppose
to think*
MENTAL STATES
to forget
to imagine
to know
to mean
to notice
to recognise
to remember
to understand
EMOTIONS / DESIRES
to envy
to fear
to dislike
to hate
to hope
to like
to love
to mind
to prefer
to regret
to want
to wish
MEASUREMENT
to contain
to cost
to hold
to measure
to weigh
OTHERS
to look (=resemble)
to seem
to be (in most cases)
to have (when it means "to possess")*
             */

        private void ProcessUsualWords(List<string> totalNamesList)
        {
            ProcessAllPronouns();
            ProcessAllPrepositions();
            ProcessAllConjunctions();
            ProcessAllInterjections();
            ProcessAllArticles();
            ProcessAllNumerals();
            ProcessAllVerbs();
            ProcessSpecialWords();

            foreach (var rootName in totalNamesList)
            {
                ProcessRootWordName(rootName);
            }
        }

        private WordFrame GetWordFrame(string word)
        {
            if (mWordsDictData.WordsDict.ContainsKey(word))
            {
                return mWordsDictData.WordsDict[word];
            }

            var wordFrame = new WordFrame();
            mWordsDictData.WordsDict[word] = wordFrame;
            wordFrame.Word = word;
            wordFrame.GrammaticalWordFrames = new List<BaseGrammaticalWordFrame>();

            return wordFrame;
        }

        private void AddGrammaticalWordFrame(string word, BaseGrammaticalWordFrame grammaticalWordFrame)
        {
            if(string.IsNullOrWhiteSpace(word))
            {
                throw new ArgumentNullException(nameof(word));
            }

            if(grammaticalWordFrame == null)
            {
                throw new ArgumentNullException(nameof(grammaticalWordFrame));
            }

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"AddGrammaticalWordFrame word = {word}");
#endif

            var wordFrame = GetWordFrame(word);

            if(grammaticalWordFrame.PartOfSpeech == GrammaticalPartOfSpeech.Noun || grammaticalWordFrame.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun)
            {
                grammaticalWordFrame.LogicalMeaning = new List<string>() { "entity" };
            }
            else
            {
                if(grammaticalWordFrame.PartOfSpeech == GrammaticalPartOfSpeech.Verb)
                {
                    if(grammaticalWordFrame.LogicalMeaning.IsNullOrEmpty())
                    {
                        grammaticalWordFrame.LogicalMeaning = new List<string>() { "act" };
                    }            
                }
            }

            if(grammaticalWordFrame.LogicalMeaning == null)
            {
                grammaticalWordFrame.LogicalMeaning = new List<string>();
            }

            grammaticalWordFrame.FullLogicalMeaning = grammaticalWordFrame.LogicalMeaning.ToList();

            if(string.IsNullOrWhiteSpace(grammaticalWordFrame.RootWord))
            {
                grammaticalWordFrame.RootWord = word;
            }

            wordFrame.GrammaticalWordFrames.Add(grammaticalWordFrame);
        }

        private void ProcessRootWordName(string rootWord)
        {
#if DEBUG
            //if (rootWord.ToLower() == "britain")
            //{
            //    NLog.LogManager.GetCurrentClassLogger().Info($"ProcessRootWordName rootWord = {rootWord}");
            //    throw new NotImplementedException();
            //}
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessRootWordName rootWord = '{rootWord}'");
#endif

            var rez = Regex.Match(rootWord, @"\(\w+\)");
            var rezStr = rez.ToString();

            if (!string.IsNullOrWhiteSpace(rezStr))
            {
                rootWord = rootWord.Replace(rezStr, string.Empty);
            }

            if(rootWord.Contains("("))
            {
                NLog.LogManager.GetCurrentClassLogger().Info($"ProcessRootWordName rootWord = '{rootWord}'");
                throw new NotImplementedException();
            }

            if (mRootNounDict.ContainsKey(rootWord))
            {
                ProcessNoun(rootWord);
            }

            var isEndsWithEr = false;

            if (mRootVerbsDict.ContainsKey(rootWord))
            {
                ProcessVerb(rootWord);

                if(rootWord.EndsWith("er"))
                {
                    isEndsWithEr = true;
                }
            }

            if (mRootAdjsDict.ContainsKey(rootWord))
            {
                ProcessAdj(rootWord);
            }
            else
            {
                if(isEndsWithEr)
                {
                    ProcessAdj(rootWord);
                }
            }

            if (mRootAdvsDict.ContainsKey(rootWord))
            {
                ProcessAdv(rootWord);
            }
        }

        private bool IsNumeral(string word)
        {
            if(mWordsDictData.WordsDict.ContainsKey(word))
            {
                return mWordsDictData.WordsDict[word].GrammaticalWordFrames.Any(p => p.IsNumeral);
            }

            return false;
        }

        private void ProcessNoun(string rootWord)
        {
            if (mTargetWordsList != null)
            {
                if (!mTargetWordsList.Contains(rootWord))
                {
                    return;
                }
            }

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun rootWord = {rootWord}");
#endif

            if(IsNumeral(rootWord))
            {
#if DEBUG
                NLog.LogManager.GetCurrentClassLogger().Info("ProcessNoun IsNumeral(rootWord) return; !!!!!");
#endif
                return;
            }

            var logicalMeaning = new List<string>() { "entity" };

            //List<string> logicalMeaning = null;

            //if(mNounClassesDict.ContainsKey(rootWord))
            //{
            //    logicalMeaning = mNounClassesDict[rootWord];
            //}
            //else
            //{
            //    logicalMeaning = new List<string>();
            //}

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun logicalMeaning.Count = {logicalMeaning.Count}");

            foreach (var className in logicalMeaning)
            {
                NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun className = {className}");
            }
#endif

            var isCountable = logicalMeaning.Contains("object") || logicalMeaning.Contains("causal_agent");

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun isCountable = {isCountable}");
#endif

            mTotalCount++;

            AddGrammaticalWordFrame(rootWord, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = isCountable,
                LogicalMeaning = logicalMeaning.ToList()
            });

            var possesiveSingular = mNounAntiStemmer.GetPossesiveForSingular(rootWord);

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun possesiveSingular = {possesiveSingular}");
#endif

            AddGrammaticalWordFrame(possesiveSingular, new NounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Number = GrammaticalNumberOfWord.Singular,
                IsCountable = isCountable,
                LogicalMeaning = logicalMeaning.ToList(),
                IsPossessive = true
            });

            var multipleForms = mNounAntiStemmer.GetPluralForm(rootWord);

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun multipleForms = {multipleForms}");
#endif
            AddGrammaticalWordFrame(multipleForms, new NounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Number = GrammaticalNumberOfWord.Plural,
                IsCountable = isCountable,
                LogicalMeaning = logicalMeaning.ToList()
            });

            mTotalCount++;

            var possesivePlural = mNounAntiStemmer.GetPossesiveForPlural(multipleForms);

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessNoun possesivePlural = {possesivePlural}");
#endif

            AddGrammaticalWordFrame(possesivePlural, new NounGrammaticalWordFrame()
            {
                RootWord = rootWord,
                Number = GrammaticalNumberOfWord.Plural,
                IsCountable = isCountable,
                LogicalMeaning = logicalMeaning.ToList(),
                IsPossessive = true
            });
        }

        private void ProcessAllVerbs()
        {
            ProcessBe("be");
            ProcessCan("can");
            ProcessCould("could");
            ProcessMay("may");
            ProcessMight("might");
            ProcessMust("must");
            ProcessWould("would");
            ProcessShould("should");
            ProcessShell("shell");
            ProcessWill("will");
            ProcessHave("have");
            ProcessDo("do");
            ProcessOught("ought");
        }

        private void ProcessVerb(string rootWord)
        {
            if (rootWord == "be")
            {      
                return;
            }

            if(rootWord == "can")
            {              
                return;
            }

            if (rootWord == "could")
            {
                return;
            }

            if (rootWord == "may")
            {
                return;
            }

            if(rootWord == "might")
            {
                return;
            }

            if (rootWord == "must")
            {
                return;
            }

            if (rootWord == "would")
            {
                return;
            }

            if (rootWord == "should")
            {
                return;
            }

            if(rootWord == "shell")
            {
                return;
            }
            
            if (rootWord == "will")
            {
                return;
            }

            if (rootWord == "have")
            {
                return;
            }

            if(rootWord == "do")
            {
                return;
            }

            if(rootWord == "ought")
            {
                return;
            }

            if (mTargetWordsList != null)
            {
                if (!mTargetWordsList.Contains(rootWord))
                {
                    return;
                }
            }

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessVerb rootWord = {rootWord}");
#endif
            mTotalCount++;

            var mayHaveGerundOrInfinitiveAfterSelf = mMayHaveGerundOrInfinitiveAfterSelfSource.ContainsWord(rootWord);
            var rootLogicalMeaningsList = mVerbLogicalMeaningsSource.GetLogicalMeanings(rootWord);

            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                LogicalMeaning = rootLogicalMeaningsList.ToList(),
                MayHaveGerundOrInfinitiveAfterSelf = mayHaveGerundOrInfinitiveAfterSelf
            });

            var pastFormsList = mVerbAntiStemmer.GetPastForms(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessVerb pastFormsList = {string.Join(',', pastFormsList)}");

            foreach(var pastForm in pastFormsList)
            {
                AddGrammaticalWordFrame(pastForm, new VerbGrammaticalWordFrame()
                {
                    Tense = GrammaticalTenses.Past,
                    VerbType = VerbType.Form_2,
                    RootWord = rootWord,
                    LogicalMeaning = rootLogicalMeaningsList.ToList(),
                    MayHaveGerundOrInfinitiveAfterSelf = mayHaveGerundOrInfinitiveAfterSelf
                });
            }

            var particleFormsList = mVerbAntiStemmer.GetParticleForms(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessVerb particleFormsList = {string.Join(',', particleFormsList)}");

            foreach(var particleForm in particleFormsList)
            {
                AddGrammaticalWordFrame(particleForm, new VerbGrammaticalWordFrame()
                {
                    VerbType = VerbType.Form_3,
                    RootWord = rootWord,
                    LogicalMeaning = rootLogicalMeaningsList.ToList(),
                    MayHaveGerundOrInfinitiveAfterSelf = mayHaveGerundOrInfinitiveAfterSelf
                });
            }

            mTotalCount += particleFormsList.Count;

            var ingForm = mVerbAntiStemmer.GetIngForm(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessVerb ingForm = {ingForm}");

            mTotalCount++;

            AddGrammaticalWordFrame(ingForm, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsGerund = true,
                RootWord = rootWord,
                LogicalMeaning = rootLogicalMeaningsList.ToList()
            });

            var presentThirdPersonForm = mVerbAntiStemmer.GetThirdPersonSingularPresent(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessVerb presentThirdPersonForm = {presentThirdPersonForm}");

            mTotalCount++;

            AddGrammaticalWordFrame(presentThirdPersonForm, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                Person = GrammaticalPerson.Third,
                RootWord = rootWord,
                LogicalMeaning = rootLogicalMeaningsList.ToList(),
                MayHaveGerundOrInfinitiveAfterSelf = mayHaveGerundOrInfinitiveAfterSelf
            });
        }

        private void ProcessBe(string rootWord)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info("ProcessBe");
#endif

            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsFormOfToBe = true
            });

            var word = "been";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                VerbType = VerbType.Form_3,
                RootWord = rootWord
            });

            word = "am";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Present,
                Number = GrammaticalNumberOfWord.Singular,
                Person = GrammaticalPerson.First,
                RootWord = rootWord
            });

            word = "is";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Present,
                Number = GrammaticalNumberOfWord.Singular,
                Person = GrammaticalPerson.Second,
                RootWord = rootWord
            });

            word = "are";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Present,
                Number = GrammaticalNumberOfWord.Plural,
                Person = GrammaticalPerson.Third,
                RootWord = rootWord
            });

            word = "was";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Past,
                Number = GrammaticalNumberOfWord.Singular,
                RootWord = rootWord
            });

            word = "were";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Past,
                Number = GrammaticalNumberOfWord.Plural,
                RootWord = rootWord
            });

            word = "being";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsGerund = true,
                RootWord = rootWord
            });
        }

        private void ProcessCan(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });
        }

        private void ProcessCould(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                IsModal = true,
                RootWord = "can"
            });
        }

        private void ProcessMay(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });
        }

        private void ProcessMight(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                IsModal = true,
                RootWord = "may"
            });
        }

        private void ProcessMust(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });
        }

        private void ProcessOught(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });
        }

        private void ProcessWould(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                IsModal = true,
                RootWord = "will"
            });
        }

        private void ProcessShell(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsModal = true
            });

            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Future,
                IsModal = false,
                IsRare = true
            });
        }

        private void ProcessShould(string rootWord)
        {
            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Past,
                IsModal = true,
                RootWord = "shell"
            });
        }

        private void ProcessWill(string rootWord)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info("ProcessWill");
#endif

            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                RootWord = "be",
                IsFormOfToBe = true,
                Tense = GrammaticalTenses.Future
            });
        }

        private void ProcessHave(string rootWord)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info("ProcessHave");
#endif

            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsFormOfToHave = true
            });

            var word = "has";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToHave = true,
                Tense = GrammaticalTenses.Present,
                Person = GrammaticalPerson.Third,
                RootWord = rootWord
            });

            word = "had";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToHave = true,
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord
            });

            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToHave = true,
                VerbType = VerbType.Form_3,
                RootWord = rootWord
            });

            word = "having";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsGerund = true,
                RootWord = rootWord
            });
        }

        private void ProcessDo(string rootWord)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info("ProcessDo");
#endif

            AddGrammaticalWordFrame(rootWord, new VerbGrammaticalWordFrame()
            {
                Tense = GrammaticalTenses.Present,
                IsFormOfToDo = true
            });

            var word = "does";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToDo = true,
                Tense = GrammaticalTenses.Present,
                Person = GrammaticalPerson.Third,
                RootWord = rootWord
            });

            word = "did";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToDo = true,
                Tense = GrammaticalTenses.Past,
                VerbType = VerbType.Form_2,
                RootWord = rootWord
            });

            word = "done";
            AddGrammaticalWordFrame(word, new VerbGrammaticalWordFrame()
            {
                IsFormOfToDo = true,
                VerbType = VerbType.Form_3,
                RootWord = rootWord
            });

            word = "doing";
            AddGrammaticalWordFrame(word, new NounGrammaticalWordFrame()
            {
                Number = GrammaticalNumberOfWord.Singular,
                IsGerund = true,
                RootWord = rootWord
            });
        }

        private void ProcessAdj(string rootWord)
        {
            if (mTargetWordsList != null)
            {
                if (!mTargetWordsList.Contains(rootWord))
                {
                    return;
                }
            }

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessAdj rootWord = {rootWord}");
#endif
            mTotalCount++;

            var rootLogicalMeaningsList = mAdjLogicalMeaningsSource.GetLogicalMeanings(rootWord);

            AddGrammaticalWordFrame(rootWord, new AdjectiveGrammaticalWordFrame()
            {
                LogicalMeaning = rootLogicalMeaningsList.ToList()
            });

            var comparativeForm = mAdjAntiStemmer.GetComparativeForm(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessAdj comparativeForm = {comparativeForm}");
            mTotalCount++;

            AddGrammaticalWordFrame(comparativeForm, new AdjectiveGrammaticalWordFrame()
            {
                Comparison = GrammaticalComparison.Comparative,
                RootWord = rootWord,
                LogicalMeaning = rootLogicalMeaningsList.ToList()
            });

            var superlativeForm = mAdjAntiStemmer.GetSuperlativeForm(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessAdj superlativeForm = {superlativeForm}");
            mTotalCount++;

            AddGrammaticalWordFrame(superlativeForm, new AdjectiveGrammaticalWordFrame()
            {
                Comparison = GrammaticalComparison.Superlative,
                RootWord = rootWord,
                LogicalMeaning = rootLogicalMeaningsList.ToList()
            });
        }

        private void ProcessAdv(string rootWord)
        {
            if (mTargetWordsList != null)
            {
                if (!mTargetWordsList.Contains(rootWord))
                {
                    return;
                }
            }

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessAdv rootWord = {rootWord}");
#endif

            if (IsNumeral(rootWord))
            {
#if DEBUG
                NLog.LogManager.GetCurrentClassLogger().Info("ProcessAdv IsNumeral(rootWord) return; !!!!!");
#endif
                return;
            }

            mTotalCount++;

            var rootLogicalMeaningsList = mAdvLogicalMeaningsSource.GetLogicalMeanings(rootWord);

            AddGrammaticalWordFrame(rootWord, new AdverbGrammaticalWordFrame()
            {
                LogicalMeaning = rootLogicalMeaningsList.ToList()
            });

            var comparativeForm = mAdvAntiStemmer.GetComparativeForm(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessAdv comparativeForm = {comparativeForm}");

            if(!string.IsNullOrWhiteSpace(comparativeForm))
            {
                mTotalCount++;

                AddGrammaticalWordFrame(comparativeForm, new AdverbGrammaticalWordFrame()
                {
                    Comparison = GrammaticalComparison.Comparative,
                    RootWord = rootWord,
                    LogicalMeaning = rootLogicalMeaningsList.ToList()
                });
            }

            var superlativeForm = mAdvAntiStemmer.GetSuperlativeForm(rootWord);

            NLog.LogManager.GetCurrentClassLogger().Info($"ProcessAdv superlativeForm = {superlativeForm}");

            if (!string.IsNullOrWhiteSpace(superlativeForm))
            {
                mTotalCount++;

                AddGrammaticalWordFrame(superlativeForm, new AdverbGrammaticalWordFrame()
                {
                    Comparison = GrammaticalComparison.Superlative,
                    RootWord = rootWord,
                    LogicalMeaning = rootLogicalMeaningsList.ToList()
                });
            }
        }
    }
}
