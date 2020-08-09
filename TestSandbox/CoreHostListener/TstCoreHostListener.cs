using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CoreHostListener
{
    public class TstCoreHostListener: IHostListener
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public IProcessCreatingResult CreateProcess(ICommand command)
        {
            _logger.Log($"command = {command}");

            throw new NotImplementedException();
        }
    }
}
