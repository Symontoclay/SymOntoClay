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
    public class BaseLoaderFromSourceCode : BaseComponent, ILoaderFromSourceCode
    {
        public BaseLoaderFromSourceCode(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        public virtual void LoadFromSourceFiles()
        {
#if DEBUG
            //Log("Begin");
#endif

            var filesList = FileHelper.GetParsedFilesInfo(_context.AppFile, _context.Id);

#if DEBUG
            //Log($"filesList.Count = {filesList.Count}");

            //Log($"filesList = {filesList.WriteListToString()}");
#endif

            var parsedFilesList = _context.Parser.Parse(filesList);

#if DEBUG
            //Log($"parsedFilesList.Count = {parsedFilesList.Count}");

            //Log($"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG
            //Log($"parsedCodeEntitiesList.Count = {parsedCodeEntitiesList.Count}");

            //Log($"parsedCodeEntitiesList = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            AddSystemDefinedSettings(parsedCodeEntitiesList);

#if DEBUG
            //Log($"parsedCodeEntitiesList.Count (2) = {parsedCodeEntitiesList.Count}");

            //Log($"parsedCodeEntitiesList (2) = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            SaveItems(parsedCodeEntitiesList);

#if IMAGINE_WORKING
            //Log("End");
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
            //Log($"codeEntity = {codeEntity}");
#endif

            switch(codeEntity.Kind)
            {
                case KindOfCodeEntity.World:
                    AddSystemDefinedSettingsToWorld(codeEntity);
                    break;

                case KindOfCodeEntity.Host:
                    AddSystemDefinedSettingsToHost(codeEntity);
                    break;

                case KindOfCodeEntity.App:
                    AddSystemDefinedSettingsToApp(codeEntity);
                    break;

                case KindOfCodeEntity.Class:
                    AddSystemDefinedSettingsToClass(codeEntity);
                    break;

                case KindOfCodeEntity.InlineTrigger:
                    break;

                case KindOfCodeEntity.RuleOrFact:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(codeEntity.Kind), codeEntity.Kind, null);
            }
        }

        private void AddSystemDefinedSettingsToWorld(CodeEntity codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _context.CommonNamesStorage.WorldName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToHost(CodeEntity codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _context.CommonNamesStorage.HostName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToApp(CodeEntity codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _context.CommonNamesStorage.ApplicationName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToClass(CodeEntity codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _context.CommonNamesStorage.ClassName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void SaveItems(List<CodeEntity> source)
        {
            foreach (var item in source)
            {
                SaveItem(item);
            }
        }

        private void SaveItem(CodeEntity codeEntity)
        {
#if DEBUG
            //Log($"codeEntity = {codeEntity}");
#endif

            var codeEntityName = codeEntity.Name;

            var globalStorage = _context.Storage.GlobalStorage;

            var metadataStorage = globalStorage.MetadataStorage;

            metadataStorage.Append(codeEntity);

            var inheritanceStorage = globalStorage.InheritanceStorage;

            foreach(var inheritanceItem in codeEntity.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(inheritanceItem);
            }

#if DEBUG
            //Log($"codeEntity (2) = {codeEntity}");
#endif

            var kindOfEntity = codeEntity.Kind;

            switch(kindOfEntity)
            {
                case KindOfCodeEntity.World:
                    break;

                case KindOfCodeEntity.Host:
                    break;

                case KindOfCodeEntity.App:
                    break;

                case KindOfCodeEntity.Class:
                    break;

                case KindOfCodeEntity.InlineTrigger:
                    globalStorage.TriggersStorage.Append(codeEntity.InlineTrigger);
                    break;

                case KindOfCodeEntity.RuleOrFact:
                    globalStorage.LogicalStorage.Append(codeEntity.RuleInstance);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfEntity), kindOfEntity, null);
            }

#if DEBUG
            //Log("End");
#endif
        }
    }
}
