using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSandbox.PlatformImplementations;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using TestSandbox.Helpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using System.Threading;

namespace TestSandbox.CodeExecution
{
    public class ActivateMainEntityHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext();

            var globalStorage = context.Storage.GlobalStorage;

            var filesList = FileHelper.GetParsedFilesInfo(context.AppFile, context.Id);

#if DEBUG
            _logger.Log($"filesList.Count = {filesList.Count}");

            _logger.Log($"filesList = {filesList.WriteListToString()}");
#endif

            var parsedFilesList = context.Parser.Parse(filesList);

#if DEBUG
            _logger.Log($"parsedFilesList.Count = {parsedFilesList.Count}");

            _logger.Log($"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG
            _logger.Log($"parsedCodeEntitiesList.Count = {parsedCodeEntitiesList.Count}");

            _logger.Log($"parsedCodeEntitiesList = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            var metadataStorage = globalStorage.MetadataStorage;

            foreach (var codeEntity in parsedCodeEntitiesList)
            {
                metadataStorage.Append(codeEntity);
            }

            var inlineTrigger = parsedCodeEntitiesList.FirstOrDefault(p => p.Kind == KindOfCodeEntity.InlineTrigger);

            _logger.Log($"inlineTrigger = {inlineTrigger}");

            var triggersStorage = globalStorage.TriggersStorage;

            triggersStorage.Append(inlineTrigger.InlineTrigger);

            var mainEntity = metadataStorage.MainCodeEntity;

#if DEBUG
            _logger.Log($"mainEntity = {mainEntity}");
#endif

            var triggersResolver = context.DataResolversFactory.GetTriggersResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = globalStorage;

            var targetTriggersList = triggersResolver.ResolveSystemEventsTriggersList(KindOfSystemEventOfInlineTrigger.Init, mainEntity.Name, localCodeExecutionContext, ResolverOptions.GetDefaultFluentOptions());

#if DEBUG
            _logger.Log($"targetTriggersList = {targetTriggersList.WriteListToString()}");
#endif

            var instancesStorage = context.InstancesStorage;

            instancesStorage.ActivateMainEntity();

            Thread.Sleep(10000);

            _logger.Log("End");
        }

        private List<CodeEntity> LinearizeSubItems(List<CodeFile> source)
        {
            var result = new List<CodeEntity>();

            foreach (var item in source)
            {
                EnumerateSubItems(item.CodeEntities, result);
            }

            return result.Distinct().ToList();
        }

        private void EnumerateSubItems(List<CodeEntity> source, List<CodeEntity> result)
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
