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

namespace SymOntoClay.Core.Internal.Converters
{
    public class ConvertersFactory: BaseContextComponent, IConvertersFactory
    {
        public ConvertersFactory(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
            _converterFactToImperativeCode = new ConverterFactToImperativeCode(_context);
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            ((IBaseContextComponent)_converterFactToImperativeCode).LinkWithOtherBaseContextComponents();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();

            ((IBaseContextComponent)_converterFactToImperativeCode).Init();
        }

        private readonly IMainStorageContext _context;

        private ConverterFactToImperativeCode _converterFactToImperativeCode;

        /// <inheritdoc/>
        public ConverterFactToImperativeCode GetConverterFactToImperativeCode()
        {
            return _converterFactToImperativeCode;
        }
    }
}
