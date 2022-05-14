﻿using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal
{
    public static class GrammaticalElementsHelper
    {
        public static string GetAspectName(GrammaticalAspect aspect)
        {
            switch (aspect)
            {
                case GrammaticalAspect.Undefined:
                    return string.Empty;

                case GrammaticalAspect.Simple:
                    return CGGramamaticalNamesOfConcepts.GrammaticalAspect_Simple;

                case GrammaticalAspect.Continuous:
                    return CGGramamaticalNamesOfConcepts.GrammaticalAspect_Continuous;

                case GrammaticalAspect.Perfect:
                    return CGGramamaticalNamesOfConcepts.GrammaticalAspect_Perfect;

                case GrammaticalAspect.PerfectContinuous:
                    return CGGramamaticalNamesOfConcepts.GrammaticalAspect_PerfectContinuous;

                default: throw new ArgumentOutOfRangeException(nameof(aspect), aspect, null);
            }
        }

        public static GrammaticalAspect GetAspectFromName(string aspectName)
        {
            if (string.IsNullOrWhiteSpace(aspectName))
            {
                return GrammaticalAspect.Undefined;
            }

            if (aspectName == CGGramamaticalNamesOfConcepts.GrammaticalAspect_Simple)
            {
                return GrammaticalAspect.Simple;
            }

            if (aspectName == CGGramamaticalNamesOfConcepts.GrammaticalAspect_Continuous)
            {
                return GrammaticalAspect.Continuous;
            }

            if (aspectName == CGGramamaticalNamesOfConcepts.GrammaticalAspect_Perfect)
            {
                return GrammaticalAspect.Perfect;
            }

            if (aspectName == CGGramamaticalNamesOfConcepts.GrammaticalAspect_PerfectContinuous)
            {
                return GrammaticalAspect.PerfectContinuous;
            }

            return GrammaticalAspect.Undefined;
        }

        public static string GetTenseName(GrammaticalTenses tense)
        {
            switch (tense)
            {
                case GrammaticalTenses.Undefined:
                    return string.Empty;

                case GrammaticalTenses.All:
                    return CGGramamaticalNamesOfConcepts.GrammaticalTenses_All;

                case GrammaticalTenses.Present:
                    return CGGramamaticalNamesOfConcepts.GrammaticalTenses_Present;

                case GrammaticalTenses.Past:
                    return CGGramamaticalNamesOfConcepts.GrammaticalTenses_Past;

                case GrammaticalTenses.Future:
                    return CGGramamaticalNamesOfConcepts.GrammaticalTenses_Future;

                case GrammaticalTenses.FutureInThePast:
                    return CGGramamaticalNamesOfConcepts.GrammaticalTenses_FutureInThePast;

                default: throw new ArgumentOutOfRangeException(nameof(tense), tense, null);
            }
        }

        public static GrammaticalTenses GetTenseFromName(string tenseName)
        {
            if (string.IsNullOrWhiteSpace(tenseName))
            {
                return GrammaticalTenses.Undefined;
            }

            if (tenseName == CGGramamaticalNamesOfConcepts.GrammaticalTenses_All)
            {
                return GrammaticalTenses.All;
            }

            if (tenseName == CGGramamaticalNamesOfConcepts.GrammaticalTenses_Present)
            {
                return GrammaticalTenses.Present;
            }

            if (tenseName == CGGramamaticalNamesOfConcepts.GrammaticalTenses_Past)
            {
                return GrammaticalTenses.Past;
            }

            if (tenseName == CGGramamaticalNamesOfConcepts.GrammaticalTenses_Future)
            {
                return GrammaticalTenses.Future;
            }

            if (tenseName == CGGramamaticalNamesOfConcepts.GrammaticalTenses_FutureInThePast)
            {
                return GrammaticalTenses.FutureInThePast;
            }

            return GrammaticalTenses.Undefined;
        }

        public static string GetVoiceName(GrammaticalVoice voice)
        {
            switch (voice)
            {
                case GrammaticalVoice.Undefined:
                    return string.Empty;

                case GrammaticalVoice.Active:
                    return CGGramamaticalNamesOfConcepts.GrammaticalVoice_Active;

                case GrammaticalVoice.Passive:
                    return CGGramamaticalNamesOfConcepts.GrammaticalVoice_Passive;

                default: throw new ArgumentOutOfRangeException(nameof(voice), voice, null);
            }
        }

        public static GrammaticalVoice GetVoiceFromName(string voiceName)
        {
            if (string.IsNullOrWhiteSpace(voiceName))
            {
                return GrammaticalVoice.Undefined;
            }

            if (voiceName == CGGramamaticalNamesOfConcepts.GrammaticalVoice_Active)
            {
                return GrammaticalVoice.Active;
            }

            if (voiceName == CGGramamaticalNamesOfConcepts.GrammaticalVoice_Passive)
            {
                return GrammaticalVoice.Passive;
            }

            return GrammaticalVoice.Undefined;
        }

        public static string GetMoodName(GrammaticalMood mood)
        {
            switch (mood)
            {
                case GrammaticalMood.Undefined:
                    return string.Empty;

                case GrammaticalMood.Indicative:
                    return CGGramamaticalNamesOfConcepts.GrammaticalMood_Indicative;

                case GrammaticalMood.Subjunctive:
                    return CGGramamaticalNamesOfConcepts.GrammaticalMood_Subjunctive;

                case GrammaticalMood.Imperative:
                    return CGGramamaticalNamesOfConcepts.GrammaticalMood_Imperative;

                case GrammaticalMood.Jussive:
                    return CGGramamaticalNamesOfConcepts.GrammaticalMood_Jussive;

                case GrammaticalMood.Potential:
                    return CGGramamaticalNamesOfConcepts.GrammaticalMood_Potential;

                case GrammaticalMood.Hypothetical:
                    return CGGramamaticalNamesOfConcepts.GrammaticalMood_Hypothetical;

                case GrammaticalMood.Hortative:
                    return CGGramamaticalNamesOfConcepts.GrammaticalMood_Hortative;

                case GrammaticalMood.Optative:
                    return CGGramamaticalNamesOfConcepts.GrammaticalMood_Optative;

                default: throw new ArgumentOutOfRangeException(nameof(mood), mood, null);
            }
        }

        public static GrammaticalMood GetMoodFromName(string moodName)
        {
            if (string.IsNullOrWhiteSpace(moodName))
            {
                return GrammaticalMood.Undefined;
            }

            if (moodName == CGGramamaticalNamesOfConcepts.GrammaticalMood_Indicative)
            {
                return GrammaticalMood.Indicative;
            }

            if (moodName == CGGramamaticalNamesOfConcepts.GrammaticalMood_Subjunctive)
            {
                return GrammaticalMood.Subjunctive;
            }

            if (moodName == CGGramamaticalNamesOfConcepts.GrammaticalMood_Imperative)
            {
                return GrammaticalMood.Imperative;
            }

            if (moodName == CGGramamaticalNamesOfConcepts.GrammaticalMood_Jussive)
            {
                return GrammaticalMood.Jussive;
            }

            if (moodName == CGGramamaticalNamesOfConcepts.GrammaticalMood_Potential)
            {
                return GrammaticalMood.Potential;
            }

            if (moodName == CGGramamaticalNamesOfConcepts.GrammaticalMood_Hypothetical)
            {
                return GrammaticalMood.Hypothetical;
            }

            if (moodName == CGGramamaticalNamesOfConcepts.GrammaticalMood_Hortative)
            {
                return GrammaticalMood.Hortative;
            }

            if (moodName == CGGramamaticalNamesOfConcepts.GrammaticalMood_Optative)
            {
                return GrammaticalMood.Optative;
            }

            return GrammaticalMood.Undefined;
        }

        public static string GetAbilityModalityName(AbilityModality modal)
        {
            switch (modal)
            {
                case AbilityModality.Undefined:
                    return string.Empty;

                case AbilityModality.None:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_None;

                case AbilityModality.Can:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Can;

                case AbilityModality.Could:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Could;

                case AbilityModality.BeAbleTo:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_BeAbleTo;

                default: 
                    throw new ArgumentOutOfRangeException(nameof(modal), modal, null);
            }
        }

        public static AbilityModality GetAbilityModalityFromName(string modalName)
        {
            if (string.IsNullOrWhiteSpace(modalName))
            {
                return AbilityModality.Undefined;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_None)
            {
                return AbilityModality.None;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Can)
            {
                return AbilityModality.Can;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Could)
            {
                return AbilityModality.Could;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_BeAbleTo)
            {
                return AbilityModality.BeAbleTo;
            }

            return AbilityModality.Undefined;
        }

        public static string GetPermissionModalityName(PermissionModality modal)
        {
            switch (modal)
            {
                case PermissionModality.Undefined:
                    return string.Empty;

                case PermissionModality.None:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_None;

                case PermissionModality.May:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_May;

                case PermissionModality.BeAllowedTo:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_BeAllowedTo;

                case PermissionModality.BePermittedTo:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_BePermittedTo;

                default: throw new ArgumentOutOfRangeException(nameof(modal), modal, null);
            }
        }

        public static PermissionModality GetPermissionModalityFromName(string modalName)
        {
            if (string.IsNullOrWhiteSpace(modalName))
            {
                return PermissionModality.Undefined;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_None)
            {
                return PermissionModality.None;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_May)
            {
                return PermissionModality.May;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_BeAllowedTo)
            {
                return PermissionModality.BeAllowedTo;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_BePermittedTo)
            {
                return PermissionModality.BePermittedTo;
            }

            return PermissionModality.Undefined;
        }

        public static string GetObligationModalityName(ObligationModality modal)
        {
            switch (modal)
            {
                case ObligationModality.Undefined:
                    return string.Empty;

                case ObligationModality.None:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_None;

                case ObligationModality.Must:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Must;

                case ObligationModality.HaveTo:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_HaveTo;

                case ObligationModality.BeTo:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_BeTo;

                case ObligationModality.Shall:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Shall;

                case ObligationModality.Should:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Should;

                case ObligationModality.OughtTo:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_OughtTo;

                case ObligationModality.Need:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Need;

                case ObligationModality.BeSupposedTo:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_BeSupposedTo;

                default: throw new ArgumentOutOfRangeException(nameof(modal), modal, null);
            }
        }

        public static ObligationModality GetObligationModalityFromName(string modalName)
        {
            if (string.IsNullOrWhiteSpace(modalName))
            {
                return ObligationModality.Undefined;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_None)
            {
                return ObligationModality.None;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Must)
            {
                return ObligationModality.Must;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_HaveTo)
            {
                return ObligationModality.HaveTo;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_BeTo)
            {
                return ObligationModality.BeTo;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Shall)
            {
                return ObligationModality.Shall;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Should)
            {
                return ObligationModality.Should;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_OughtTo)
            {
                return ObligationModality.OughtTo;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Need)
            {
                return ObligationModality.Need;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_BeSupposedTo)
            {
                return ObligationModality.BeSupposedTo;
            }

            return ObligationModality.Undefined;
        }

        public static string GetProbabilityModalityName(ProbabilityModality modal)
        {
            switch (modal)
            {
                case ProbabilityModality.Undefined:
                    return string.Empty;

                case ProbabilityModality.None:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_None;

                case ProbabilityModality.Might:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Might;

                default: throw new ArgumentOutOfRangeException(nameof(modal), modal, null);
            }
        }

        public static ProbabilityModality GetProbabilityModalityFromName(string modalName)
        {
            if (string.IsNullOrWhiteSpace(modalName))
            {
                return ProbabilityModality.Undefined;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_None)
            {
                return ProbabilityModality.None;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Might)
            {
                return ProbabilityModality.Might;
            }

            return ProbabilityModality.Undefined;
        }

        public static string GetConditionalModalityName(ConditionalModality modal)
        {
            switch (modal)
            {
                case ConditionalModality.Undefined:
                    return string.Empty;

                case ConditionalModality.None:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_None;

                case ConditionalModality.Would:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Would;

                case ConditionalModality.Could:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Could;

                case ConditionalModality.Should:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_Should;

                case ConditionalModality.BeSupposedTo:
                    return CGGramamaticalNamesOfConcepts.KindOfModal_BeSupposedTo;

                default: throw new ArgumentOutOfRangeException(nameof(modal), modal, null);
            }
        }

        public static ConditionalModality GetConditionalModalityFromName(string modalName)
        {
            if (string.IsNullOrWhiteSpace(modalName))
            {
                return ConditionalModality.Undefined;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_None)
            {
                return ConditionalModality.None;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Would)
            {
                return ConditionalModality.Would;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Could)
            {
                return ConditionalModality.Could;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_Should)
            {
                return ConditionalModality.Should;
            }

            if (modalName == CGGramamaticalNamesOfConcepts.KindOfModal_BeSupposedTo)
            {
                return ConditionalModality.BeSupposedTo;
            }

            return ConditionalModality.Undefined;
        }

        public static KindOfGrammaticalRelation GetKindOfGrammaticalRelationFromName(string relationName)
        {
            if (string.IsNullOrWhiteSpace(relationName))
            {
                return KindOfGrammaticalRelation.Undefined;
            }

            if (relationName == CGGramamaticalNamesOfRelations.AspectName)
            {
                return KindOfGrammaticalRelation.Aspect;
            }

            if (relationName == CGGramamaticalNamesOfRelations.TenseName)
            {
                return KindOfGrammaticalRelation.Tense;
            }

            if (relationName == CGGramamaticalNamesOfRelations.VoiceName)
            {
                return KindOfGrammaticalRelation.Voice;
            }

            if (relationName == CGGramamaticalNamesOfRelations.MoodName)
            {
                return KindOfGrammaticalRelation.Mood;
            }

            if (relationName == CGGramamaticalNamesOfRelations.AbilityModalityName)
            {
                return KindOfGrammaticalRelation.AbilityModality;
            }

            if (relationName == CGGramamaticalNamesOfRelations.PermissionModalityName)
            {
                return KindOfGrammaticalRelation.PermissionModality;
            }

            if (relationName == CGGramamaticalNamesOfRelations.ObligationModalityName)
            {
                return KindOfGrammaticalRelation.ObligationModality;
            }

            if (relationName == CGGramamaticalNamesOfRelations.ProbabilityModalityName)
            {
                return KindOfGrammaticalRelation.ProbabilityModality;
            }

            if (relationName == CGGramamaticalNamesOfRelations.ConditionalModalityName)
            {
                return KindOfGrammaticalRelation.ConditionalModality;
            }

            return KindOfGrammaticalRelation.Undefined;
        }

        public static bool IsEntityCondition(string relationName)
        {
            if (relationName == CGGramamaticalNamesOfRelations.EntityCondition)
            {
                return true;
            }

            return false;
        }
    }
}