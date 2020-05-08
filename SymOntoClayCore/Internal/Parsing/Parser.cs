using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public class Parser : BaseComponent, IParser
    {
        public Parser(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        public CodeFile Parse(ParsedFileInfo parsedFileInfo)
        {
            throw new NotImplementedException();
        }

        public List<CodeFile> Parse(List<ParsedFileInfo> parsedFileInfoList)
        {
            throw new NotImplementedException();
        }

        public List<CodeEntity> Parse(string text)
        {
            throw new NotImplementedException();
        }
    }
}
