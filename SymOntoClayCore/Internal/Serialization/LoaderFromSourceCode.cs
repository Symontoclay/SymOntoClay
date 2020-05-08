using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization
{
    public class LoaderFromSourceCode : BaseComponent
    {
        public LoaderFromSourceCode(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        public void Run()
        {
#if DEBUG
            Log("Begin");
#endif

            var filesList = FileHelper.GetParsedFilesInfo(_context.AppFile, _context.Id);

#if DEBUG
            Log($"filesList.Count = {filesList.Count}");

            foreach (var file in filesList)
            {
                Log($"file = {file}");
            }
#endif

#if DEBUG
            Log("End");
#endif
        }
    }
}
