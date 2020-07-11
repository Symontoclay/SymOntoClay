using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing.Internal;
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

            var result = new CodeFile();
            result.IsMain = parsedFileInfo.IsMain;
            result.FileName = parsedFileInfo.FileName;
            
            var internalParserContext = new InternalParserContext(text, result, _context);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var codeEntitiesList = parser.Result;

#if DEBUG
            Log($"codeEntitiesList = {codeEntitiesList.WriteListToString()}");
#endif

            result.CodeEntities = codeEntitiesList;

            return result;
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
