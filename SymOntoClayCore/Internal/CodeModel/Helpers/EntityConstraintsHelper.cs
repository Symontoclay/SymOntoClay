using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class EntityConstraintsHelper
    {
#if DEBUG
        private static ILogger _gbcLogger;
#endif

        private static StrongIdentifierValue _randomConstraintName;

        private static List<StrongIdentifierValue> _baseConstraintsList;

        static EntityConstraintsHelper()
        {
#if DEBUG
            _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

            _randomConstraintName = NameHelper.CreateName(StandardNamesConstants.RandomConstraintName);

            _baseConstraintsList = new List<StrongIdentifierValue>() { _randomConstraintName };
        }

        public static List<StrongIdentifierValue> GetConstraintsList(IMainStorageContext context, LocalCodeExecutionContext localCodeExecutionContext)
        {

        }
    }
}
