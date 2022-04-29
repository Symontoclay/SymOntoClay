using NLog;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.GrammarHelpers
{
    public static class CorrespondsHelper
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static bool SubjectAndVerb(BaseGrammaticalWordFrame subject, VerbGrammaticalWordFrame verb)
        {
#if DEBUG
            _gbcLogger.Info($"subject = {subject}");
            _gbcLogger.Info($"verb = {verb}");
#endif

            switch(subject.PartOfSpeech)
            {
                case GrammaticalPartOfSpeech.Pronoun:
                    return PronounSubjectAndVerb(subject.AsPronoun, verb);

                default:
                    throw new ArgumentOutOfRangeException(nameof(subject.PartOfSpeech), subject.PartOfSpeech, null);
            }
        }

        private static bool PronounSubjectAndVerb(PronounGrammaticalWordFrame subject, VerbGrammaticalWordFrame verb)
        {
#if DEBUG
            _gbcLogger.Info($"subject = {subject}");
            _gbcLogger.Info($"verb = {verb}");
#endif

            if(subject.Person == GrammaticalPerson.First)
            {
                if(subject.Number == GrammaticalNumberOfWord.Singular && verb.Person == GrammaticalPerson.Neuter && (verb.Number == GrammaticalNumberOfWord.Singular || verb.Number == GrammaticalNumberOfWord.Neuter))
                {
                    return true;
                }
            }

            throw new NotImplementedException();
        }
    }
}
