/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
            applicationInheritanceItem.SuperName = context.CommonNamesStorage.AppName;
            applicationInheritanceItem.Rank = new LogicalValue(1.0F);

            _logger.Info($"applicationInheritanceItem (1) = {applicationInheritanceItem}");

            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(applicationInheritanceItem, defaultSettings);

            _logger.Info($"applicationInheritanceItem (2) = {applicationInheritanceItem}");

            _logger.Info("End");
        }
    }
}
