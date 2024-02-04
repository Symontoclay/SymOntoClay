using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public interface ILogFileCreatorContext
    {
        string ConvertDotStrToImg(string dotStr);
    }
}
