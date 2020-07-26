using NLog;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.Helpers;

namespace TestSandbox.Handlers
{
    public class DefaultSettingsOfCodeEntityHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext();

            var defaultSettings = new DefaultSettingsOfCodeEntity();
            defaultSettings.Holder = NameHelper.CreateName("Tor", context.Dictionary);

            _logger.Info($"defaultSettings = {defaultSettings}");

            var applicationInheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            applicationInheritanceItem.SubName = NameHelper.CreateName("PeaseKeeper", context.Dictionary);
            applicationInheritanceItem.SuperName = context.CommonNamesStorage.ApplicationName;
            applicationInheritanceItem.Rank = new LogicalValue(1.0F);

            _logger.Info($"applicationInheritanceItem (1) = {applicationInheritanceItem}");

            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(applicationInheritanceItem, defaultSettings);

            _logger.Info($"applicationInheritanceItem (2) = {applicationInheritanceItem}");

            _logger.Info("End");
        }
    }
}
