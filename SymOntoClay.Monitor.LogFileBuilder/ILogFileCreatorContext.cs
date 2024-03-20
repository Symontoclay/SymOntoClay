using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public interface ILogFileCreatorContext
    {
        string GetFileExtension();
        string DecorateFileHeader();
        string DecorateFileFooter();
        string DecorateItem(ulong globalMessageNumber, string content);
        (string AbsoluteName, string RelativeName) ConvertDotStrToImg(string dotStr, string targetFileName);
        string CreateImgLink(string imgFileName, string relativeFileName);
        string ResolveMessagesRefs(string content);
        string NormalizeText(string content);
    }
}
