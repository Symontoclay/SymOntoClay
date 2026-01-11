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

using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Services
{
    public class ServicesFactory : BaseContextComponent, IServicesFactory
    {
        public ServicesFactory(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;

            _entityConstraintsService = new EntityConstraintsService(_context);
            _baseContextComponents.Add(_entityConstraintsService);

            _codeFrameService = new CodeFrameService(_context);
            _baseContextComponents.Add(_codeFrameService);
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            foreach (var item in _baseContextComponents)
            {
                item.LinkWithOtherBaseContextComponents();
            }
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();

            foreach (var item in _baseContextComponents)
            {
                item.Init();
            }
        }

        private readonly IMainStorageContext _context;

        private EntityConstraintsService _entityConstraintsService;

        private CodeFrameService _codeFrameService;

        private readonly List<IBaseContextComponent> _baseContextComponents = new List<IBaseContextComponent>();

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _entityConstraintsService?.Dispose();
            _codeFrameService?.Dispose();

            base.OnDisposed();
        }

        /// <inheritdoc/>
        public IEntityConstraintsService GetEntityConstraintsService()
        {
            return _entityConstraintsService;
        }

        /// <inheritdoc/>
        public ICodeFrameService GetCodeFrameService()
        {
            return _codeFrameService;
        }
    }
}
