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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestSandbox.Helpers;

namespace TestSandbox.CodeExecution
{
    public class ActivateMainEntityHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public void Run()
        {
            _logger.Info("063417C9-B09E-414A-B62A-A06E5A8DAC4E", "Begin");

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            var globalStorage = context.Storage.GlobalStorage;

            var filesList = FileHelper.GetParsedFilesInfo(_logger, context.AppFile, context.Id);

#if DEBUG
            _logger.Info("54D2CA26-DF1B-457E-92DE-521225BC9DF4", $"filesList.Count = {filesList.Count}");

            _logger.Info("E32EDBFA-7ACF-4380-BCA6-26129BFE6C54", $"filesList = {filesList.WriteListToString()}");
#endif

            var parsedFilesList = context.Parser.Parse(filesList, globalStorage.DefaultSettingsOfCodeEntity);

#if DEBUG
            _logger.Info("5C47577F-86E5-49DB-BA60-C71C060E6DE6", $"parsedFilesList.Count = {parsedFilesList.Count}");

            _logger.Info("E8F6F152-E5FA-4AD3-9685-591D37D00FBA", $"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG
            _logger.Info("3AA315F2-F51B-44C9-A306-1CF0D67598E4", $"parsedCodeEntitiesList.Count = {parsedCodeEntitiesList.Count}");

            _logger.Info("F7C9516D-845A-4DEC-8D23-2B53870B773F", $"parsedCodeEntitiesList = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            var metadataStorage = globalStorage.MetadataStorage;

            foreach (var codeEntity in parsedCodeEntitiesList)
            {
                metadataStorage.Append(_logger, codeEntity);
            }

            var inlineTrigger = parsedCodeEntitiesList.FirstOrDefault(p => p.Kind == KindOfCodeEntity.InlineTrigger);

            _logger.Info("551208E0-3E80-4059-8C76-8B331DC5E036", $"inlineTrigger = {inlineTrigger}");

            var triggersStorage = globalStorage.TriggersStorage;

            triggersStorage.Append(_logger, inlineTrigger.AsInlineTrigger);

            var mainEntity = metadataStorage.MainCodeEntity;

#if DEBUG
            _logger.Info("8400A737-96A9-40AF-987D-329A723827AB", $"mainEntity = {mainEntity}");
#endif

            var triggersResolver = context.DataResolversFactory.GetTriggersResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = globalStorage;

            var targetTriggersList = triggersResolver.ResolveSystemEventsTriggersList(_logger, KindOfSystemEventOfInlineTrigger.Enter, mainEntity.Name, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            _logger.Info("19A255FA-44E8-4B4C-995B-7A5D946E69C2", $"targetTriggersList = {targetTriggersList.WriteListToString()}");
#endif

            var instancesStorage = context.InstancesStorage;

            instancesStorage.ActivateMainEntity(_logger);

            Thread.Sleep(10000);

            _logger.Info("6C2F10BC-3376-462E-AA32-E75422AF78D2", "End");
        }

        private List<CodeItem> LinearizeSubItems(List<CodeFile> source)
        {
            var result = new List<CodeItem>();

            foreach (var item in source)
            {
                EnumerateSubItems(item.CodeEntities, result);
            }

            return result.Distinct().ToList();
        }

        private void EnumerateSubItems(List<CodeItem> source, List<CodeItem> result)
        {
            if (source.IsNullOrEmpty())
            {
                return;
            }

            result.AddRange(source);

            foreach (var item in source)
            {
                EnumerateSubItems(item.SubItems, result);

            }
        }
    }
}
