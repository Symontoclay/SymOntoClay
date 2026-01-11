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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ConditionalEntitySourceValueNode
    {
        public ConditionalEntitySourceValueNode(ConditionalEntitySourceValue value, ContextOfConverterFactToInternalCG context)
        {
            _value = value;
            _context = context;
            _logger = context.Logger;
        }

        private readonly ConditionalEntitySourceValue _value;
        private readonly ContextOfConverterFactToInternalCG _context;
        private readonly IMonitorLogger _logger;

        public ResultOfNode Run(IMonitorLogger logger)
        {
            var outerConceptualGraph = new InternalConceptualGraph();

            if (_context.ConceptualGraph != null)
            {
                outerConceptualGraph.Parent = _context.ConceptualGraph;
            }

            var context = new ContextOfConverterFactToInternalCG();
            context.ConceptualGraph = outerConceptualGraph;
            context.Logger = _logger;
            context.NLPContext = _context.NLPContext;

            if(_value.LogicalQuery == null)
            {
                throw new NotImplementedException("14AF570A-DB94-43F8-AC0D-C3D191120EDB");
            }
            else
            {
                var factNode = new RuleInstanceNode(_value.LogicalQuery, context);
                var factNodeResult = factNode.Run(logger);

            }

            var result = new ResultOfNode() { KindOfResult = KindOfResultOfNode.ProcessConditionalEntity };
            result.CGNode = outerConceptualGraph;

            return result;
        }
    }
}
