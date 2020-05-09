using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization
{
    public class LoaderFromSourceCode : BaseComponent, ILoaderFromSourceCode
    {
        public LoaderFromSourceCode(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        public void LoadFromSourceFiles()
        {
#if DEBUG
            Log("Begin");
#endif

            var filesList = FileHelper.GetParsedFilesInfo(_context.AppFile, _context.Id);

#if DEBUG
            Log($"filesList.Count = {filesList.Count}");

            Log($"filesList = {filesList.WriteListToString()}");
#endif

            var parsedFilesList = _context.Parser.Parse(filesList);

#if DEBUG
            Log($"parsedFilesList.Count = {parsedFilesList.Count}");

            Log($"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

#if DEBUG
            Log("End");
#endif
        }
    }
}
