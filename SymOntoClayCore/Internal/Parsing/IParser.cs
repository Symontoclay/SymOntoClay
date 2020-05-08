using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public interface IParser
    {
        CodeFile Parse(ParsedFileInfo parsedFileInfo);
        List<CodeFile> Parse(List<ParsedFileInfo> parsedFileInfoList);
        List<CodeEntity> Parse(string text);
    }
}
