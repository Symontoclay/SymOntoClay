/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;

namespace TestSandbox.Parsing
{
    public class CompileInlineTriggerHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            var globalStorage = context.Storage.GlobalStorage;

            var filesList = FileHelper.GetParsedFilesInfo(context.AppFile, context.Id);

#if DEBUG
            _logger.Info($"filesList.Count = {filesList.Count}");

            _logger.Info($"filesList = {filesList.WriteListToString()}");
#endif

            var parsedFilesList = context.Parser.Parse(filesList, globalStorage.DefaultSettingsOfCodeEntity);

#if DEBUG
            _logger.Info($"parsedFilesList.Count = {parsedFilesList.Count}");

            _logger.Info($"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG
            _logger.Info($"parsedCodeEntitiesList.Count = {parsedCodeEntitiesList.Count}");

            _logger.Info($"parsedCodeEntitiesList = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            var metadataStorage = globalStorage.MetadataStorage;

            foreach (var codeEntity in parsedCodeEntitiesList)
            {
                metadataStorage.Append(codeEntity);
            }

            var inlineTrigger = parsedCodeEntitiesList.FirstOrDefault(p => p.Kind == KindOfCodeEntity.InlineTrigger);

            _logger.Info($"inlineTrigger = {inlineTrigger}");

            var triggersStorage = globalStorage.TriggersStorage;

            triggersStorage.Append(inlineTrigger.AsInlineTrigger);

            var mainEntity = metadataStorage.MainCodeEntity;

#if DEBUG
            _logger.Info($"mainEntity = {mainEntity}");
#endif

            var triggersResolver = context.DataResolversFactory.GetTriggersResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = globalStorage;

            var targetTriggersList = triggersResolver.ResolveSystemEventsTriggersList(KindOfSystemEventOfInlineTrigger.Enter, mainEntity.Name, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            _logger.Info($"targetTriggersList = {targetTriggersList.WriteListToString()}");
#endif

            _logger.Info("End");
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
