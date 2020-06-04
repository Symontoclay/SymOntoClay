using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization
{
    public class LoaderFromSourceCode : BaseComponent, ILoaderFromSourceCode
    {
        public LoaderFromSourceCode(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        public void LoadFromSourceFiles()
        {
#if DEBUG
            Log("Begin");
#endif

            var filesList = FileHelper.GetParsedFilesInfo(_context.AppFile, _context.Id);

#if DEBUG
            Log($"filesList.Count = {filesList.Count}");

            Log($"filesList = {filesList.WriteListToString()}");
#endif

            var parsedFilesList = _context.Parser.Parse(filesList);

#if DEBUG
            Log($"parsedFilesList.Count = {parsedFilesList.Count}");

            Log($"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG
            Log($"parsedCodeEntitiesList.Count = {parsedCodeEntitiesList.Count}");

            Log($"parsedCodeEntitiesList = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            AddSystemDefinedSettings(parsedCodeEntitiesList);

#if DEBUG
            Log($"parsedCodeEntitiesList.Count (2) = {parsedCodeEntitiesList.Count}");

            Log($"parsedCodeEntitiesList (2) = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            SaveItems(parsedCodeEntitiesList);

#if IMAGINE_WORKING
            Log("End");
#else
            throw new NotImplementedException();
#endif
        }

        private List<CodeEntity> LinearizeSubItems(List<CodeFile> source)
        {
            var result = new List<CodeEntity>();

            foreach(var item in source)
            {
                EnumerateSubItems(item.CodeEntities, result);
            }

            return result.Distinct().ToList();
        }

        private void EnumerateSubItems(List<CodeEntity> source, List<CodeEntity> result)
        {
            if(source.IsNullOrEmpty())
            {
                return;
            }

            result.AddRange(source);

            foreach(var item in source)
            {
                EnumerateSubItems(item.SubItems, result);
            }
        }

        private void AddSystemDefinedSettings(List<CodeEntity> source)
        {
            foreach(var item in source)
            {
                AddSystemDefinedSettings(item);
            }
        }

        private void AddSystemDefinedSettings(CodeEntity codeEntity)
        {
#if DEBUG
            Log($"codeEntity = {codeEntity}");
#endif

            switch(codeEntity.Kind)
            {
                case KindOfCodeEntity.App:
                    AddSystemDefinedSettingsToApp(codeEntity);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(codeEntity.Kind), codeEntity.Kind, null);
            }
        }

        private void AddSystemDefinedSettingsToApp(CodeEntity codeEntity)
        {
            var applicationInheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            applicationInheritanceItem.SubName = codeEntity.Name;
            applicationInheritanceItem.SuperName = _context.CommonNamesStorage.ApplicationName;
            applicationInheritanceItem.Rank = new LogicalValue(1.0F);

#if DEBUG
            Log($"applicationInheritanceItem = {applicationInheritanceItem}");
#endif

            codeEntity.InheritanceItems.Add(applicationInheritanceItem);
        }

        private void SaveItems(List<CodeEntity> source)
        {
            foreach (var item in source)
            {
                SaveItem(item);
            }
        }

        public void SaveItem(CodeEntity codeEntity)
        {
#if DEBUG
            Log($"codeEntity = {codeEntity}");
#endif

            var codeEntityName = codeEntity.Name;

            var inheritanceStorage = _context.Storage.GlobalStorage.InheritanceStorage;

            foreach(var inheritanceItem in codeEntity.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(codeEntityName, inheritanceItem);
            }

#if IMAGINE_WORKING
            Log("End");
#else
            throw new NotImplementedException();
#endif
        }
    }
}
