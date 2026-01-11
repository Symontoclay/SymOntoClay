/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal
{
    public static class CGGramamaticalNamesOfConcepts
    {
        public const string KindOfQuestion_General = "general";
        public const string KindOfQuestion_Subject = "subject";
        public const string KindOfQuestion_Special = "special";
        public const string KindOfQuestion_Tag = "tag";

        public const string GrammaticalAspect_Simple = "simple";
        public const string GrammaticalAspect_Continuous = "continuous";
        public const string GrammaticalAspect_Perfect = "perfect";
        public const string GrammaticalAspect_PerfectContinuous = "perfect continuous";

        public const string GrammaticalTenses_All = "all";
        public const string GrammaticalTenses_Present = "present";
        public const string GrammaticalTenses_Past = "past";
        public const string GrammaticalTenses_Future = "future";
        public const string GrammaticalTenses_FutureInThePast = "future in the past";

        public const string GrammaticalVoice_Active = "active";
        public const string GrammaticalVoice_Passive = "passive";
        public const string GrammaticalMood_Indicative = "indicative";
        public const string GrammaticalMood_Subjunctive = "subjunctive";
        public const string GrammaticalMood_Imperative = "imperative";
        public const string GrammaticalMood_Jussive = "jussive";
        public const string GrammaticalMood_Potential = "potential";
        public const string GrammaticalMood_Hypothetical = "hypothetical";
        public const string GrammaticalMood_Hortative = "hortative";
        public const string GrammaticalMood_Optative = "optative";

        public const string KindOfModal_None = "none";

        public const string KindOfModal_Must = "must";
        public const string KindOfModal_Can = "can";
        public const string KindOfModal_Could = "could";
        public const string KindOfModal_May = "may";
        public const string KindOfModal_Shall = "shall";
        public const string KindOfModal_Should = "should";
        public const string KindOfModal_Might = "might";
        public const string KindOfModal_Would = "would";
        public const string KindOfModal_Need = "need";
        public const string KindOfModal_BeAbleTo = "be_able_to";
        public const string KindOfModal_BeAllowedTo = "be_allowed_to";
        public const string KindOfModal_BePermittedTo = "be_permitted_to";
        public const string KindOfModal_HaveTo = "have_to";
        public const string KindOfModal_BeTo = "be_to";
        public const string KindOfModal_OughtTo = "ought_to";
        public const string KindOfModal_BeSupposedTo = "be_supposed_to";
    }
}
