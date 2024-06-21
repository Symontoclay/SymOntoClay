using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.Handlers
{
    public class LogFileBuilderAppCommandLineParserHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");


            //FullValidNamedCommandLine_Success();
            //FullValidPositionedCommandLine_Success();
            //ValidNamedCommandLine_Help_Success();
            //ValidPositionedCommandLine_Help_Success();
            //ValidNamedCommandLine_Html_AbsUrl_Success();
            //ValidPositionedCommandLine_Html_AbsUrl_Success();
            //ValidNamedCommandLine_Html_Success();
            //ValidPositionedCommandLine_Html_Success();
            //ValidNamedCommandLine_Configuration_Success();
            //ValidPositionedCommandLine_Configuration_Success();
            //ValidNamedCommandLine_Nologo_Success();
            //ValidPositionedCommandLine_Nologo_Success();
            //ValidNamedCommandLine_SplitByNodes_SplitByThreads_Success();
            //ValidPositionedCommandLine_SplitByNodes_SplitByThreads_Success();
            //ValidNamedCommandLine_TargetNodeId_TargetTreadId_Success();
            //ValidPositionedCommandLine_TargetNodeId_TargetTreadId_Success();
            //MinimalNamedCommandline_Success();
            MinimalPositionedCommandline_Success();

            _logger.Info("End");
        }

        private void FullValidNamedCommandLine_Success()
        {
            throw new NotImplementedException();
        }

        private void FullValidPositionedCommandLine_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidNamedCommandLine_Help_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidPositionedCommandLine_Help_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidNamedCommandLine_Html_AbsUrl_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidPositionedCommandLine_Html_AbsUrl_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidNamedCommandLine_Html_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidPositionedCommandLine_Html_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidNamedCommandLine_Configuration_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidPositionedCommandLine_Configuration_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidNamedCommandLine_Nologo_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidPositionedCommandLine_Nologo_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidNamedCommandLine_SplitByNodes_SplitByThreads_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidPositionedCommandLine_SplitByNodes_SplitByThreads_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidNamedCommandLine_TargetNodeId_TargetTreadId_Success()
        {
            throw new NotImplementedException();
        }

        private void ValidPositionedCommandLine_TargetNodeId_TargetTreadId_Success()
        {
            throw new NotImplementedException();
        }

        private void MinimalNamedCommandline_Success()
        {
            throw new NotImplementedException();
        }

        private void MinimalPositionedCommandline_Success()
        {
            throw new NotImplementedException();
        }
    }
}
