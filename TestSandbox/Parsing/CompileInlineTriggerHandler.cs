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
            _logger.Info("054AA112-A5AB-4271-8656-68F8A83AAD41", "Begin");

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            var globalStorage = context.Storage.GlobalStorage;

            var filesList = FileHelper.GetParsedFilesInfo(context.AppFile, context.Id);

#if DEBUG
            _logger.Info("B3C4FE99-B0FA-426F-87CB-86963A26AE5F", $"filesList.Count = {filesList.Count}");

            _logger.Info("EB878C4E-1B31-4E29-ADAC-B1E7E80813DD", $"filesList = {filesList.WriteListToString()}");
#endif

            var parsedFilesList = context.Parser.Parse(filesList, globalStorage.DefaultSettingsOfCodeEntity);

#if DEBUG
            _logger.Info("684C171C-BE80-4481-A984-5DFC0073998D", $"parsedFilesList.Count = {parsedFilesList.Count}");

            _logger.Info("2A0D7352-4EBC-4A83-BE4C-A39BC1AF965B", $"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG
            _logger.Info("3CEDBEDA-6DF1-4535-8EF6-A96BA68E6E54", $"parsedCodeEntitiesList.Count = {parsedCodeEntitiesList.Count}");

            _logger.Info("F06A2EBE-10AE-41A0-9C9D-19C3DD8D19D0", $"parsedCodeEntitiesList = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            var metadataStorage = globalStorage.MetadataStorage;

            foreach (var codeEntity in parsedCodeEntitiesList)
            {
                metadataStorage.Append(codeEntity);
            }

            var inlineTrigger = parsedCodeEntitiesList.FirstOrDefault(p => p.Kind == KindOfCodeEntity.InlineTrigger);

            _logger.Info("8A38B320-8367-4BCB-9F1F-9D3D4DA1D2D4", $"inlineTrigger = {inlineTrigger}");

            var triggersStorage = globalStorage.TriggersStorage;

            triggersStorage.Append(inlineTrigger.AsInlineTrigger);

            var mainEntity = metadataStorage.MainCodeEntity;

#if DEBUG
            _logger.Info("D9807FBA-704B-414F-A025-5CE1A32DE5C4", $"mainEntity = {mainEntity}");
#endif

            var triggersResolver = context.DataResolversFactory.GetTriggersResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = globalStorage;

            var targetTriggersList = triggersResolver.ResolveSystemEventsTriggersList(KindOfSystemEventOfInlineTrigger.Enter, mainEntity.Name, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            _logger.Info("D30E0ACA-3D8B-4352-B278-A5443311D169", $"targetTriggersList = {targetTriggersList.WriteListToString()}");
#endif

            _logger.Info("D7B22065-75FF-48B4-97B1-6CAE41BA68D2", "End");
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
