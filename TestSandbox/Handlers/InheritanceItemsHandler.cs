/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using TestSandbox.Helpers;

namespace TestSandbox.Handlers
{
    public class InheritanceItemsHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public void Run()
        {
            _logger.Info("FFFEDD0B-BDBF-40BD-8939-4D5D53629B75", "Begin");

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;


            var inheritanceStorage = context.Storage.GlobalStorage.InheritanceStorage;

            var subName = NameHelper.CreateName("SubClass");
            var superName = NameHelper.CreateName("SuperClass");

            var inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(1);

            _logger.Info("EF5322F5-8263-47B6-A95B-A39249B41A29", $"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(_logger, inheritanceItem);

            inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(0.5F);

            _logger.Info("1FF978A6-CDAA-46DD-A5BE-CFB2A561C68F", $"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(_logger, inheritanceItem);

            inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(0);

            _logger.Info("83E54C8F-F3A5-44DB-8E20-C4880A4F6162", $"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(_logger, inheritanceItem);

            var list = inheritanceStorage.GetItemsDirectly(_logger, subName);

            _logger.Info("A6755979-C263-45DF-92FD-F5AEC3DEAF54", $"list.Count = {list.Count}");
            _logger.Info("0F084B37-A9AC-4597-A8C3-B9CA3963F17C", $"inheritanceItem = {list.WriteListToString()}");

            _logger.Info("990B0ABE-9272-4BC7-93B0-E62FEDBE4716", "End");
        }
    }
}
