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

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal
{
    public static class CGGramamaticalNamesOfRelations
    {
        public const string KindOfQuestionName = "__kind_of_question";
        public const string AspectName = "__aspect";
        public const string TenseName = "__tense";
        public const string VoiceName = "__voice";
        public const string MoodName = "__mood";
        public const string AbilityModalityName = "__ability_modality";
        public const string PermissionModalityName = "__permission_modality";
        public const string ObligationModalityName = "__obligation_modality";
        public const string ProbabilityModalityName = "__probability_modality";
        public const string ConditionalModalityName = "__conditional_modalityName";

        public const string DeterminerName = "determiner";
        public const string EntityCondition = "__entity_condition";
    }
}
