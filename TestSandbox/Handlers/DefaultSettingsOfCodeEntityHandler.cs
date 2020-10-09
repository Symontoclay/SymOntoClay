/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;

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
