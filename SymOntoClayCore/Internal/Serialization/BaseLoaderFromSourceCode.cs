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

            if(string.IsNullOrEmpty(_context.AppFile))
            {
                return;
            }

            var filesList = FileHelper.GetParsedFilesInfo(_context.AppFile, _context.Id);

#if DEBUG
            //Log($"filesList.Count = {filesList.Count}");

            //Log($"filesList = {filesList.WriteListToString()}");
#endif

            var globalStorage = _context.Storage.GlobalStorage;

            var parsedFilesList = _context.Parser.Parse(filesList, globalStorage.DefaultSettingsOfCodeEntity);

#if DEBUG
            //Log($"parsedFilesList.Count = {parsedFilesList.Count}");

            //Log($"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG
            //Log($"parsedCodeEntitiesList.Count = {parsedCodeEntitiesList.Count}");

            //Log($"parsedCodeEntitiesList = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            DetectMainCodeEntity(parsedCodeEntitiesList);

#if DEBUG
            //Log($"parsedCodeEntitiesList.Count (2) = {parsedCodeEntitiesList.Count}");

            //Log($"parsedCodeEntitiesList (2) = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            AddSystemDefinedSettings(parsedCodeEntitiesList);

#if DEBUG
            //Log($"parsedCodeEntitiesList.Count (3) = {parsedCodeEntitiesList.Count}");

            //Log($"parsedCodeEntitiesList (3) = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            SaveItems(parsedCodeEntitiesList);

            var instancesStorage = _context.InstancesStorage;

            instancesStorage.ActivateMainEntity();

#if IMAGINE_WORKING
            //Log("End");
#else
            throw new NotImplementedException();
#endif
        }

        private void DetectMainCodeEntity(List<CodeEntity> source)
        {
            var possibleMainCodeEntities = source.Where(p => p.Kind == KindOfCodeEntity.App || p.Kind == KindOfCodeEntity.World);

            var count = possibleMainCodeEntities.Count();

#if DEBUG
            //Log($"count = {count}");

            //Log($"possibleMainCodeEntities = {possibleMainCodeEntities.WriteListToString()}");
#endif

            if (count == 1)
            {
                var possibleMainCodeEntity = possibleMainCodeEntities.Single();
                possibleMainCodeEntity.CodeFile.IsMain = true;
            }
            else
            {
                if(count > 1)
                {
                    throw new NotImplementedException();
                }                
            }
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

        private void AddSystemDefinedSettingsToApp(CodeEntity codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _context.CommonNamesStorage.AppName;
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
