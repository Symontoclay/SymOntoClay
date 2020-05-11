using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public class Parser : BaseComponent, IParser
    {
        public Parser(IParserContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IParserContext _context;

        public CodeFile Parse(ParsedFileInfo parsedFileInfo)
        {
#if DEBUG
            Log($"parsedFileInfo = {parsedFileInfo}");
#endif

            var text = File.ReadAllText(parsedFileInfo.FileName);

#if DEBUG
            Log($"text = {text}");
#endif




            throw new NotImplementedException();
        }

        public List<CodeFile> Parse(List<ParsedFileInfo> parsedFileInfoList)
        {
#if DEBUG
            Log($"parsedFileInfoList = {parsedFileInfoList.WriteListToString()}");
#endif

            var result = new List<CodeFile>();

            foreach(var parsedFileInfo in parsedFileInfoList)
            {
                result.Add(Parse(parsedFileInfo));
            }

            return result;
        }
    }
}
