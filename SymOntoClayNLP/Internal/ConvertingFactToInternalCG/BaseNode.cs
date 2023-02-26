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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public abstract class BaseNode
    {
        protected BaseNode(ContextOfConverterFactToInternalCG context)
        {
            _context = context;
            _logger = context.Logger;
        }

        protected readonly ContextOfConverterFactToInternalCG _context;
        protected readonly IEntityLogger _logger;

        protected InternalConceptCGNode CreateOrGetExistingInternalConceptCGNode(string name)
        {
#if DEBUG
            //_logger.Log($"name = {name}");
#endif

            var result = _context.ConceptualGraph.Children.SingleOrDefault(p => p.Kind == KindOfCGNode.Concept && p.Name == name);

#if DEBUG
            //_logger.Log($"result = {result}");
#endif

            if (result != null)
            {
                return result.AsConceptNode;
            }

            return new InternalConceptCGNode() { Name = name, Parent = _context.ConceptualGraph };
        }
    }
}
