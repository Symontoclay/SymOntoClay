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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
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

        public void LoadCodeItem(CodeItem codeItem, IStorage targetStorage)
        {
            var defferedLibsList = new List<string>();

            SaveItem(codeItem, targetStorage, defferedLibsList);

            var subItems = LinearizeSubItems(codeItem);

            if(subItems.Any())
            {
                SaveItems(subItems, targetStorage, defferedLibsList);
            }
        }

        public List<string> LoadFromSourceFiles(IStorage targetStorage, string projectFile)
        {
            return LoadFromSourceFiles(targetStorage, projectFile, string.Empty);
        }

        public List<string> LoadFromSourceFiles(IStorage targetStorage, string projectFile, string id)
        {
            Init();

            var filesList = FileHelper.GetParsedFilesInfo(projectFile, id);

            var defferedLibsList = new List<string>();

            ProcessFilesList(filesList, true, targetStorage, defferedLibsList);

            return defferedLibsList;
        }

        public List<string> LoadFromPaths(IStorage targetStorage, IList<string> sourceCodePaths)
        {
            Init();

            var filesList = FileHelper.GetParsedFilesFromPaths(sourceCodePaths);

#if DEBUG

#endif

            var defferedLibsList = new List<string>();

            ProcessFilesList(filesList, false, targetStorage, defferedLibsList);

            return defferedLibsList;
        }

        private void ProcessFilesList(List<ParsedFileInfo> filesList, bool detectMainCodeEntity, IStorage targetStorage, List<string> defferedLibsList)
        {

            var parsedFilesList = _context.Parser.Parse(filesList, _defaultSettingsOfCodeEntity);

#if DEBUG

#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG

#endif

            if (detectMainCodeEntity)
            {
                DetectMainCodeEntity(parsedCodeEntitiesList);
            }

#if DEBUG

#endif

            CheckHolderAndTypeOfAccess(parsedCodeEntitiesList);

            AddSystemDefinedSettings(parsedCodeEntitiesList);

#if DEBUG

#endif

            SaveItems(parsedCodeEntitiesList, targetStorage, defferedLibsList);
        }

        private void DetectMainCodeEntity(List<CodeItem> source)
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

        private List<CodeItem> LinearizeSubItems(List<CodeFile> source)
        {
            var result = new List<CodeItem>();

            foreach (var item in source)
            {
                EnumerateSubItems(item.CodeEntities, result);
            }

            return result.Distinct().ToList();
        }

        private List<CodeItem> LinearizeSubItems(CodeItem codeItem)
        {
            var result = new List<CodeItem>();

            EnumerateSubItems(codeItem.SubItems, result);

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

        private void CheckHolderAndTypeOfAccess(List<CodeItem> source)
        {
            foreach (var item in source)
            {
                CheckHolderAndTypeOfAccess(item);
            }
        }

        private void CheckHolderAndTypeOfAccess(CodeItem codeEntity)
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

        private void AddSystemDefinedSettings(List<CodeItem> source)
        {
            foreach (var item in source)
            {
                AddSystemDefinedSettings(item);
            }
        }

        private void AddSystemDefinedSettings(CodeItem codeEntity)
        {
            switch (codeEntity.Kind)
            {
                case KindOfCodeEntity.World:
                    AddSystemDefinedSettingsToWorld(codeEntity);
                    break;

                case KindOfCodeEntity.App:
                    AddSystemDefinedSettingsToApp(codeEntity);
                    break;

                case KindOfCodeEntity.Lib:
                    break;

                case KindOfCodeEntity.Class:
                    AddSystemDefinedSettingsToClass(codeEntity);
                    break;

                case KindOfCodeEntity.Action:
                    AddSystemDefinedSettingsToAction(codeEntity);
                    break;

                case KindOfCodeEntity.State:
                    AddSystemDefinedSettingsToState(codeEntity);
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

        private void AddSystemDefinedSettingsToWorld(CodeItem codeEntity)
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

        private void AddSystemDefinedSettingsToApp(CodeItem codeEntity)
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

        private void AddSystemDefinedSettingsToClass(CodeItem codeEntity)
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

        private void AddSystemDefinedSettingsToAction(CodeItem codeEntity)
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

        private void AddSystemDefinedSettingsToState(CodeItem codeEntity)
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

        private void SaveItems(List<CodeItem> source, IStorage targetStorage, List<string> defferedLibsList)
        {
            foreach (var item in source)
            {
                SaveItem(item, targetStorage, defferedLibsList);
            }
        }

        private void SaveItem(CodeItem codeItem, IStorage targetStorage, List<string> defferedLibsList)
        {
            var codeEntityName = codeItem.Name;

            var metadataStorage = targetStorage.MetadataStorage;

            metadataStorage.Append(codeItem);

            var inheritanceStorage = targetStorage.InheritanceStorage;

            foreach (var inheritanceItem in codeItem.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(inheritanceItem);
            }

            var kindOfEntity = codeItem.Kind;

            switch (kindOfEntity)
            {
                case KindOfCodeEntity.World:
                    ProcessImport(codeItem, targetStorage, defferedLibsList);
                    GeneratePreConstructor(codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.App:
                    ProcessImport(codeItem, targetStorage, defferedLibsList);
                    GeneratePreConstructor(codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.Lib:
                    ProcessImport(codeItem, targetStorage, defferedLibsList);
                    GeneratePreConstructor(codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.Class:
                    GeneratePreConstructor(codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.AnonymousObject:
                    GeneratePreConstructor(codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.InlineTrigger:
                    targetStorage.TriggersStorage.Append(codeItem.AsInlineTrigger);
                    break;

                case KindOfCodeEntity.RelationDescription:
                    targetStorage.RelationsStorage.Append(codeItem.AsRelationDescription);
                    break;

                case KindOfCodeEntity.RuleOrFact:
                    {
                        var ruleInstance = codeItem.AsRuleInstance;

                        if (ruleInstance.IsParameterized)
                        {
                            throw new Exception($"SymOntoClay does not support parameterized rule or facts on object declaration.");
                        }

                        targetStorage.LogicalStorage.Append(ruleInstance);

                    }
                    break;

                case KindOfCodeEntity.LinguisticVariable:
                    targetStorage.FuzzyLogicStorage.Append(codeItem.AsLinguisticVariable);
                    break;

                case KindOfCodeEntity.Function:
                    targetStorage.MethodsStorage.Append(codeItem.AsNamedFunction);
                    break;

                case KindOfCodeEntity.Constructor:
                    targetStorage.ConstructorsStorage.Append(codeItem.AsConstructor);
                    break;

                case KindOfCodeEntity.Action:
                    targetStorage.ActionsStorage.Append(codeItem.AsAction);
                    GeneratePreConstructor(codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.State:
                    targetStorage.StatesStorage.Append(codeItem.AsState);
                    GeneratePreConstructor(codeItem, targetStorage);
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
                    targetStorage.StatesStorage.Append(codeItem.AsMutuallyExclusiveStatesSet);
                    break;

                case KindOfCodeEntity.Synonym:
                    targetStorage.SynonymsStorage.Append(codeItem.AsSynonym);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfEntity), kindOfEntity, null);
            }

            CheckCodeDirectives(codeItem);

            var idleActionItems = codeItem.IdleActionItems;

            if (!idleActionItems.IsNullOrEmpty())
            {
                var idleActionItemsStorage = targetStorage.IdleActionItemsStorage;

                foreach (var idleActionItem in idleActionItems)
                {
                    idleActionItemsStorage.Append(idleActionItem);
                }
            }

        }

        private void GeneratePreConstructor(CodeItem codeItem, IStorage targetStorage)
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

                    targetStorage.ConstructorsStorage.AppendPreConstructor(preConstructor);
                }
            }
        }

        private void CheckCodeDirectives(CodeItem codeItem)
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

        private void ProcessImport(CodeItem codeItem, IStorage targetStorage, List<string> defferedLibsList)
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

            var libsList = _context.ModulesStorage.Import(importsList);

            if(!libsList.Any())
            {
                return;
            }

            var existingStorages = targetStorage.GetStorages();

            var storagesForAdding = libsList.Except(existingStorages);

            foreach(var storageForAdding in storagesForAdding)
            {
                targetStorage.AddParentStorage(storageForAdding);
            }
        }
    }
}
