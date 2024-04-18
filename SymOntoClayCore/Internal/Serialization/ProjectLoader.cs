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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization
{
    public class ProjectLoader : BaseComponent
    {
        public ProjectLoader(IMainStorageContext context)
            : this(context, false)
        {
        }

        public ProjectLoader(IMainStorageContext context, bool isDefferedImport)
            : base(context.Logger)
        {
            _context = context;
            _compiler = context.Compiler;
            _isDefferedImport = isDefferedImport;
        }

        private readonly IMainStorageContext _context;
        private readonly ICompiler _compiler;
        private readonly bool _isDefferedImport;
        private DefaultSettingsOfCodeEntity _defaultSettingsOfCodeEntity;
        private IStorage _globalStorage;
        private ICommonNamesStorage _commonNamesStorage;

        private void Init()
        {
            if(_globalStorage == null)
            {
                _globalStorage = _context.Storage.GlobalStorage;
                _defaultSettingsOfCodeEntity = _globalStorage.DefaultSettingsOfCodeEntity;
                _commonNamesStorage = _context.CommonNamesStorage;
            }
        }

        public void LoadCodeItem(IMonitorLogger logger, CodeItem codeItem, IStorage targetStorage)
        {
            var defferedLibsList = new List<string>();

            SaveItem(logger, codeItem, targetStorage, defferedLibsList);

            var subItems = LinearizeSubItems(logger, codeItem);

            if(subItems.Any())
            {
                SaveItems(logger, subItems, targetStorage, defferedLibsList);
            }
        }

        public List<string> LoadFromSourceFiles(IMonitorLogger logger, IStorage targetStorage, string projectFile)
        {
            return LoadFromSourceFiles(logger, targetStorage, projectFile, string.Empty);
        }

        public List<string> LoadFromSourceFiles(IMonitorLogger logger, IStorage targetStorage, string projectFile, string id)
        {
            Init();

            var filesList = FileHelper.GetParsedFilesInfo(logger, projectFile, id);

            var defferedLibsList = new List<string>();

            ProcessFilesList(logger, filesList, true, targetStorage, defferedLibsList);

            return defferedLibsList;
        }

        public List<string> LoadFromPaths(IMonitorLogger logger, IStorage targetStorage, IList<string> sourceCodePaths)
        {
            Init();

            var filesList = FileHelper.GetParsedFilesFromPaths(logger, sourceCodePaths);

#if DEBUG

#endif

            var defferedLibsList = new List<string>();

            ProcessFilesList(logger, filesList, false, targetStorage, defferedLibsList);

            return defferedLibsList;
        }

        private void ProcessFilesList(IMonitorLogger logger, List<ParsedFileInfo> filesList, bool detectMainCodeEntity, IStorage targetStorage, List<string> defferedLibsList)
        {

            var parsedFilesList = _context.Parser.Parse(filesList, _defaultSettingsOfCodeEntity);

#if DEBUG

#endif

            var parsedCodeEntitiesList = LinearizeSubItems(logger, parsedFilesList);

#if DEBUG

#endif

            if (detectMainCodeEntity)
            {
                DetectMainCodeEntity(logger, parsedCodeEntitiesList);
            }

#if DEBUG

#endif

            CheckHolderAndTypeOfAccess(logger, parsedCodeEntitiesList);

            AddSystemDefinedSettings(logger, parsedCodeEntitiesList);

#if DEBUG

#endif

            SaveItems(logger, parsedCodeEntitiesList, targetStorage, defferedLibsList);
        }

        private void DetectMainCodeEntity(IMonitorLogger logger, List<CodeItem> source)
        {
            var possibleMainCodeEntities = source.Where(p => p.Kind == KindOfCodeEntity.App || p.Kind == KindOfCodeEntity.World);

            var count = possibleMainCodeEntities.Count();

#if DEBUG

#endif

            if (count == 1)
            {
                var possibleMainCodeEntity = possibleMainCodeEntities.Single();
                possibleMainCodeEntity.CodeFile.IsMain = true;
            }
            else
            {
                if (count > 1)
                {
                    throw new NotImplementedException();
                }
            }
        }

        private List<CodeItem> LinearizeSubItems(IMonitorLogger logger, List<CodeFile> source)
        {
            var result = new List<CodeItem>();

            foreach (var item in source)
            {
                EnumerateSubItems(logger, item.CodeEntities, result);
            }

            return result.Distinct().ToList();
        }

        private List<CodeItem> LinearizeSubItems(IMonitorLogger logger, CodeItem codeItem)
        {
            var result = new List<CodeItem>();

            EnumerateSubItems(logger, codeItem.SubItems, result);

            return result.Distinct().ToList();
        }

        private void EnumerateSubItems(IMonitorLogger logger, List<CodeItem> source, List<CodeItem> result)
        {
            if (source.IsNullOrEmpty())
            {
                return;
            }

            result.AddRange(source);

            foreach (var item in source)
            {
                EnumerateSubItems(logger, item.SubItems, result);
            }
        }

        private void CheckHolderAndTypeOfAccess(IMonitorLogger logger, List<CodeItem> source)
        {
            foreach (var item in source)
            {
                CheckHolderAndTypeOfAccess(logger, item);
            }
        }

        private void CheckHolderAndTypeOfAccess(IMonitorLogger logger, CodeItem codeEntity)
        {
            var kind = codeEntity.Kind;

            if (kind == KindOfCodeEntity.App || kind == KindOfCodeEntity.World || kind == KindOfCodeEntity.Lib || kind == KindOfCodeEntity.Class)
            {
                return;
            }

            if (codeEntity.Holder == null && codeEntity.TypeOfAccess == TypeOfAccess.Protected)
            {
                codeEntity.TypeOfAccess = TypeOfAccess.Public;
            }
        }

        private void AddSystemDefinedSettings(IMonitorLogger logger, List<CodeItem> source)
        {
            foreach (var item in source)
            {
                AddSystemDefinedSettings(logger, item);
            }
        }

        private void AddSystemDefinedSettings(IMonitorLogger logger, CodeItem codeEntity)
        {
            switch (codeEntity.Kind)
            {
                case KindOfCodeEntity.World:
                    AddSystemDefinedSettingsToWorld(logger, codeEntity);
                    break;

                case KindOfCodeEntity.App:
                    AddSystemDefinedSettingsToApp(logger, codeEntity);
                    break;

                case KindOfCodeEntity.Lib:
                    break;

                case KindOfCodeEntity.Class:
                    AddSystemDefinedSettingsToClass(logger, codeEntity);
                    break;

                case KindOfCodeEntity.Action:
                    AddSystemDefinedSettingsToAction(logger, codeEntity);
                    break;

                case KindOfCodeEntity.State:
                    AddSystemDefinedSettingsToState(logger, codeEntity);
                    break;

                case KindOfCodeEntity.InlineTrigger:
                    break;

                case KindOfCodeEntity.RelationDescription:
                    break;

                case KindOfCodeEntity.RuleOrFact:
                    break;

                case KindOfCodeEntity.LinguisticVariable:
                    break;

                case KindOfCodeEntity.Function:
                    break;

                case KindOfCodeEntity.Constructor:
                    break;

                case KindOfCodeEntity.Operator:
                    break;

                case KindOfCodeEntity.Field:
                    break;

                case KindOfCodeEntity.MutuallyExclusiveStatesSet:
                    break;

                case KindOfCodeEntity.Synonym:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(codeEntity.Kind), codeEntity.Kind, null);
            }
        }

        private void AddSystemDefinedSettingsToWorld(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _commonNamesStorage.WorldName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToApp(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _commonNamesStorage.AppName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToClass(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _commonNamesStorage.ClassName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToAction(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _commonNamesStorage.ActionName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToState(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = codeEntity.Name;
            inheritanceItem.SuperName = _commonNamesStorage.StateName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void SaveItems(IMonitorLogger logger, List<CodeItem> source, IStorage targetStorage, List<string> defferedLibsList)
        {
            foreach (var item in source)
            {
                SaveItem(logger, item, targetStorage, defferedLibsList);
            }
        }

        private void SaveItem(IMonitorLogger logger, CodeItem codeItem, IStorage targetStorage, List<string> defferedLibsList)
        {
            var codeEntityName = codeItem.Name;

            var metadataStorage = targetStorage.MetadataStorage;

            metadataStorage.Append(logger, codeItem);

            var inheritanceStorage = targetStorage.InheritanceStorage;

            foreach (var inheritanceItem in codeItem.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(logger, inheritanceItem);
            }

            var kindOfEntity = codeItem.Kind;

            switch (kindOfEntity)
            {
                case KindOfCodeEntity.World:
                    ProcessImport(logger, codeItem, targetStorage, defferedLibsList);
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.App:
                    ProcessImport(logger, codeItem, targetStorage, defferedLibsList);
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.Lib:
                    ProcessImport(logger, codeItem, targetStorage, defferedLibsList);
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.Class:
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.AnonymousObject:
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.InlineTrigger:
                    targetStorage.TriggersStorage.Append(logger, codeItem.AsInlineTrigger);
                    break;

                case KindOfCodeEntity.RelationDescription:
                    targetStorage.RelationsStorage.Append(logger, codeItem.AsRelationDescription);
                    break;

                case KindOfCodeEntity.RuleOrFact:
                    {
                        var ruleInstance = codeItem.AsRuleInstance;

                        if (ruleInstance.IsParameterized)
                        {
                            throw new Exception($"SymOntoClay does not support parameterized rule or facts on object declaration.");
                        }

                        targetStorage.LogicalStorage.Append(logger, ruleInstance);

                    }
                    break;

                case KindOfCodeEntity.LinguisticVariable:
                    targetStorage.FuzzyLogicStorage.Append(logger, codeItem.AsLinguisticVariable);
                    break;

                case KindOfCodeEntity.Function:
                    targetStorage.MethodsStorage.Append(logger, codeItem.AsNamedFunction);
                    break;

                case KindOfCodeEntity.Constructor:
                    targetStorage.ConstructorsStorage.Append(logger, codeItem.AsConstructor);
                    break;

                case KindOfCodeEntity.Action:
                    targetStorage.ActionsStorage.Append(logger, codeItem.AsAction);
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.State:
                    targetStorage.StatesStorage.Append(logger, codeItem.AsState);
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.Operator:
                    if (codeItem.AsOperator.KindOfOperator == KindOfOperator.CallFunction)
                    {
                        break;
                    }
                    throw new NotImplementedException();

                case KindOfCodeEntity.Field:
                    break;

                case KindOfCodeEntity.MutuallyExclusiveStatesSet:
                    targetStorage.StatesStorage.Append(logger, codeItem.AsMutuallyExclusiveStatesSet);
                    break;

                case KindOfCodeEntity.Synonym:
                    targetStorage.SynonymsStorage.Append(logger, codeItem.AsSynonym);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfEntity), kindOfEntity, null);
            }

            CheckCodeDirectives(logger, codeItem);

            var idleActionItems = codeItem.IdleActionItems;

            if (!idleActionItems.IsNullOrEmpty())
            {
                var idleActionItemsStorage = targetStorage.IdleActionItemsStorage;

                foreach (var idleActionItem in idleActionItems)
                {
                    idleActionItemsStorage.Append(logger, idleActionItem);
                }
            }

        }

        private void GeneratePreConstructor(IMonitorLogger logger, CodeItem codeItem, IStorage targetStorage)
        {
            var subItems = codeItem.SubItems;

            if(!subItems.IsNullOrEmpty())
            {
                var fieldsList = subItems.Where(p => p.IsField).Select(p => p.AsField).ToList();

                if(fieldsList.Any())
                {
                    var compiledBody = _compiler.Compile(fieldsList);

                    var preConstructor = new Constructor();
                    preConstructor.CompiledFunctionBody = compiledBody;
                    preConstructor.Holder = codeItem.Name;

                    targetStorage.ConstructorsStorage.AppendPreConstructor(logger, preConstructor);
                }
            }
        }

        private void CheckCodeDirectives(IMonitorLogger logger, CodeItem codeItem)
        {
            var directives = codeItem.Directives;

            var kindOfEntity = codeItem.Kind;

            switch (kindOfEntity)
            {
                case KindOfCodeEntity.App:
                    foreach (var directive in directives)
                    {
                        var kindOfDirective = directive.KindOfCodeItemDirective;

                        switch (kindOfDirective)
                        {
                            case KindOfCodeItemDirective.SetDefaultState:
                                break;

                            case KindOfCodeItemDirective.SetState:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfDirective), kindOfDirective, null);
                        }
                    }
                    break;

                case KindOfCodeEntity.World:
                case KindOfCodeEntity.Lib:
                case KindOfCodeEntity.Class:
                case KindOfCodeEntity.AnonymousObject:
                case KindOfCodeEntity.InlineTrigger:
                case KindOfCodeEntity.RelationDescription:
                case KindOfCodeEntity.RuleOrFact:
                case KindOfCodeEntity.LinguisticVariable:
                case KindOfCodeEntity.Function:
                case KindOfCodeEntity.Constructor:
                case KindOfCodeEntity.Action:
                case KindOfCodeEntity.State:
                case KindOfCodeEntity.Operator:
                case KindOfCodeEntity.Field:
                case KindOfCodeEntity.MutuallyExclusiveStatesSet:
                case KindOfCodeEntity.Synonym:
                    if (directives.Any())
                    {
                        throw new Exception($"Directives does not allowed for {kindOfEntity}.");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfEntity), kindOfEntity, null);
            }
        }

        private void ProcessImport(IMonitorLogger logger, CodeItem codeItem, IStorage targetStorage, List<string> defferedLibsList)
        {
            var importsList = codeItem.ImportsList;

            if (importsList.IsNullOrEmpty())
            {
                return;
            }

            if(_isDefferedImport)
            {
                defferedLibsList.AddRange(importsList.Select(p => p.NameValue));
                return;
            }

            var libsList = _context.ModulesStorage.Import(logger, importsList);

            if(!libsList.Any())
            {
                return;
            }

            var existingStorages = targetStorage.GetStorages(logger);

            var storagesForAdding = libsList.Except(existingStorages);

            foreach(var storageForAdding in storagesForAdding)
            {
                targetStorage.AddParentStorage(logger, storageForAdding);
            }
        }
    }
}
