/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class AppInstance : BaseInstance
    {
        public AppInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage)
            : base(codeItem, context, parentStorage, new ObjectStorageFactory())
        {
        }

        /// <inheritdoc/>
        protected override void ApplyCodeDirectives()
        {
#if DEBUG
            Log("Begin");
#endif

            var codeItemDirectivesResolver = _context.DataResolversFactory.GetCodeItemDirectivesResolver();

            var directivesList = codeItemDirectivesResolver.Resolve(_localCodeExecutionContext);

#if DEBUG
            //Log($"_codeItem = {_codeItem}");
#endif

            foreach (var directive in directivesList)
            {
#if DEBUG
                Log($"directive = {directive}");
#endif

                var kindOfDirective = directive.KindOfCodeItemDirective;

                switch (kindOfDirective)
                {
                    case KindOfCodeItemDirective.SetDefaultState:
                        {
                            throw new NotImplementedException();
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfDirective), kindOfDirective, null);
                }
            }

#if DEBUG
            Log("End");
#endif
        }
    }
}