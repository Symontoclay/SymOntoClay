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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToFact
{
    public class ContextOfConvertingInternalCGToFact
    {
        public Dictionary<InternalConceptualGraph, RuleInstance> RuleInstancesDict { get; set; } = new Dictionary<InternalConceptualGraph, RuleInstance>();
        public List<RuleInstance> AnnotationsList { get; set; } = new List<RuleInstance>();

        private Dictionary<BaseInternalCGNode, List<InternalRelationCGNode>> mAnnotationRelations = new Dictionary<BaseInternalCGNode, List<InternalRelationCGNode>>();

        public void AddRelationAsAnnotation(BaseInternalCGNode target, InternalRelationCGNode annotatingRelation)
        {
            if (mAnnotationRelations.ContainsKey(target))
            {
                var list = mAnnotationRelations[target];

                if (!list.Contains(annotatingRelation))
                {
                    list.Add(annotatingRelation);
                }
                return;
            }

            mAnnotationRelations[target] = new List<InternalRelationCGNode>() { annotatingRelation };
        }

        public Dictionary<InternalRelationCGNode, BaseInternalConceptCGNode> ModifiedRelationsDict = new Dictionary<InternalRelationCGNode, BaseInternalConceptCGNode>();

        public List<InternalRelationCGNode> GetAnnotationRelations(InternalRelationCGNode annotatedRelation)
        {
            if (!ModifiedRelationsDict.ContainsKey(annotatedRelation))
            {
                return new List<InternalRelationCGNode>();
            }

            var sourceModifiedNode = ModifiedRelationsDict[annotatedRelation];

            if (mAnnotationRelations.ContainsKey(sourceModifiedNode))
            {
                return mAnnotationRelations[sourceModifiedNode];
            }

            return new List<InternalRelationCGNode>();
        }
    }
}
