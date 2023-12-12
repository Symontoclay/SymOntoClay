using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public interface IMessageContentToTextConverterOptions
    {
        bool EnableCallMethodIdOfMethodLabel { get; }
        bool EnableMethodSignatureArguments { get; }
        bool EnableTypesListOfMethodSignatureArguments { get; }
        bool EnableDefaultValueOfMethodSignatureArguments { get; }
        bool EnablePassedVauesOfMethodLabel { get; }
    }
}
