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

using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Serialization
{
    public class BaseLoaderFromSourceCode : BaseComponent, ILoaderFromSourceCode
    {
        public BaseLoaderFromSourceCode(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
            _projectLoader = new ProjectLoader(context);
        }

        private readonly IMainStorageContext _context;
        private readonly ProjectLoader _projectLoader;

        public virtual void LoadFromSourceFiles()
        {
            if (string.IsNullOrEmpty(_context.AppFile))
            {
                return;
            }

            _projectLoader.LoadFromSourceFiles(Logger, _context.Storage.GlobalStorage, _context.AppFile, _context.Id);

            var instancesStorage = _context.InstancesStorage;

            instancesStorage.ActivateMainEntity(Logger);
        }

        public virtual void LoadFromPaths(IList<string> sourceCodePaths)
        {
            _projectLoader.LoadFromPaths(Logger, _context.Storage.GlobalStorage, sourceCodePaths);
        }
    }
}
