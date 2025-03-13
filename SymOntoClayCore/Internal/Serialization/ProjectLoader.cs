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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Compiling.Internal;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Serialization
{
    public class ProjectLoader : BaseComponent
    {
        public ProjectLoader(IMainStorageContext context)
            : this(context, false)
        {
        }

        public ProjectLoader(IMainStorageContext context, bool isDeferredImport)
            : base(context.Logger)
        {
            _context = context;
            _compiler = context.Compiler;
            _isDeferredImport = isDeferredImport;
        }

        private readonly IMainStorageContext _context;
        private readonly ICompiler _compiler;
        private readonly bool _isDeferredImport;
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
            var deferredLibsList = new List<string>();

            SaveItem(logger, codeItem, targetStorage, deferredLibsList);

            var subItems = LinearizeSubItems(logger, codeItem);

            if(subItems.Any())
            {
                SaveItems(logger, subItems, targetStorage, deferredLibsList);
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

            var deferredLibsList = new List<string>();

            ProcessFilesList(logger, filesList, true, targetStorage, deferredLibsList);

            return deferredLibsList;
        }

        public List<string> LoadFromPaths(IMonitorLogger logger, IStorage targetStorage, IList<string> sourceCodePaths)
        {
            Init();

            var filesList = FileHelper.GetParsedFilesFromPaths(logger, sourceCodePaths);

            var deferredLibsList = new List<string>();

            ProcessFilesList(logger, filesList, false, targetStorage, deferredLibsList);

            return deferredLibsList;
        }

        private void ProcessFilesList(IMonitorLogger logger, List<ParsedFileInfo> filesList, bool detectMainCodeEntity, IStorage targetStorage, List<string> deferredLibsList)
        {

            var parsedFilesList = _context.Parser.Parse(filesList, _defaultSettingsOfCodeEntity);

            var parsedCodeEntitiesList = LinearizeSubItems(logger, parsedFilesList);

            if (detectMainCodeEntity)
            {
                DetectMainCodeEntity(logger, parsedCodeEntitiesList);
            }

            CheckHolderAndTypeOfAccess(logger, parsedCodeEntitiesList);

            AddSystemDefinedSettings(logger, parsedCodeEntitiesList);

            SaveItems(logger, parsedCodeEntitiesList, targetStorage, deferredLibsList);
        }

        private void DetectMainCodeEntity(IMonitorLogger logger, List<CodeItem> source)
        {
            var possibleMainCodeEntities = source.Where(p => p.Kind == KindOfCodeEntity.App || p.Kind == KindOfCodeEntity.World);

            var count = possibleMainCodeEntities.Count();

            if (count == 1)
            {
                var possibleMainCodeEntity = possibleMainCodeEntities.Single();
                possibleMainCodeEntity.CodeFile.IsMain = true;
            }
            else
            {
                if (count > 1)
                {
                    throw new NotImplementedException("B2A082CC-352C-4C98-8269-C36A4E434BB8");
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

                case KindOfCodeEntity.Property:
                    break;

                case KindOfCodeEntity.MutuallyExclusiveStatesSet:
                    break;

                case KindOfCodeEntity.Synonym:
                    break;

                case KindOfCodeEntity.PrimitiveTask:
                    break;

                case KindOfCodeEntity.CompoundTask:
                    break;

                case KindOfCodeEntity.TacticalTask:
                    break;

                case KindOfCodeEntity.StrategicTask:
                    break;

                case KindOfCodeEntity.RootTask:
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

            inheritanceItem.SubType = codeEntity.TypeInfo;
            inheritanceItem.SuperType = _commonNamesStorage.WorldTypeInfo;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToApp(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubType = codeEntity.TypeInfo;
            inheritanceItem.SuperType = _commonNamesStorage.AppTypeInfo;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToClass(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubType = codeEntity.TypeInfo;
            inheritanceItem.SuperType = _commonNamesStorage.ClassTypeInfo;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToAction(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubType = codeEntity.TypeInfo;
            inheritanceItem.SuperType = _commonNamesStorage.ActionTypeInfo;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void AddSystemDefinedSettingsToState(IMonitorLogger logger, CodeItem codeEntity)
        {
            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubType = codeEntity.TypeInfo;
            inheritanceItem.SuperType = _commonNamesStorage.StateTypeInfo;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void SaveItems(IMonitorLogger logger, List<CodeItem> source, IStorage targetStorage, List<string> deferredLibsList)
        {
            foreach (var item in source)
            {
                SaveItem(logger, item, targetStorage, deferredLibsList);
            }
        }

        private void SaveItem(IMonitorLogger logger, CodeItem codeItem, IStorage targetStorage, List<string> deferredLibsList)
        {
            var codeEntityName = codeItem.TypeInfo;

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
                    ProcessImport(logger, codeItem, targetStorage, deferredLibsList);
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.App:
                    ProcessImport(logger, codeItem, targetStorage, deferredLibsList);
                    GeneratePreConstructor(logger, codeItem, targetStorage);
                    break;

                case KindOfCodeEntity.Lib:
                    ProcessImport(logger, codeItem, targetStorage, deferredLibsList);
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
                    throw new NotImplementedException("385ECFA7-F331-4554-9667-D9313EF3AE88");

                case KindOfCodeEntity.Field:
                    break;

                case KindOfCodeEntity.Property:
                    break;

                case KindOfCodeEntity.MutuallyExclusiveStatesSet:
                    targetStorage.StatesStorage.Append(logger, codeItem.AsMutuallyExclusiveStatesSet);
                    break;

                case KindOfCodeEntity.Synonym:
                    targetStorage.SynonymsStorage.Append(logger, codeItem.AsSynonym);
                    break;

                case KindOfCodeEntity.RootTask:
                    targetStorage.TasksStorage.Append(logger, codeItem.AsRootTask);
                    break;

                case KindOfCodeEntity.StrategicTask:
                    targetStorage.TasksStorage.Append(logger, codeItem.AsStrategicTask);
                    break;

                case KindOfCodeEntity.TacticalTask:
                    targetStorage.TasksStorage.Append(logger, codeItem.AsTacticalTask);
                    break;

                case KindOfCodeEntity.CompoundTask:
                    targetStorage.TasksStorage.Append(logger, codeItem.AsCompoundTask);
                    break;

                case KindOfCodeEntity.PrimitiveTask:
                    targetStorage.TasksStorage.Append(logger, codeItem.AsPrimitiveTask);
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
                var propertiesList = subItems.Where(p => p.IsProperty).Select(p => p.AsProperty).ToList();

                if(fieldsList.Any() || propertiesList.Any())
                {
                    var preConstructor = new PreConstructor();
                    preConstructor.Holder = codeItem.TypeInfo;

                    var intermediateCommandsList = new List<IntermediateScriptCommand>();

                    if (fieldsList.Any())
                    {
                        intermediateCommandsList.AddRange(_compiler.CompileToIntermediateCommands(fieldsList));
                    }

                    if (propertiesList.Any())
                    {
                        intermediateCommandsList.AddRange(_compiler.CompileToIntermediateCommands(propertiesList));
                    }

                    var compiledBody = _compiler.ConvertToCompiledFunctionBody(intermediateCommandsList);

#if DEBUG
                    //Info("558FAFA7-8244-47B2-8E4B-AAFA24A1104A", $"compiledBody = {compiledBody.ToDbgString()}");
#endif

                    preConstructor.CompiledFunctionBody = compiledBody;

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
                case KindOfCodeEntity.Property:
                case KindOfCodeEntity.MutuallyExclusiveStatesSet:
                case KindOfCodeEntity.Synonym:
                case KindOfCodeEntity.RootTask:
                case KindOfCodeEntity.StrategicTask:
                case KindOfCodeEntity.TacticalTask:
                case KindOfCodeEntity.CompoundTask:
                case KindOfCodeEntity.PrimitiveTask:
                    if (directives.Any())
                    {
                        throw new Exception($"Directives does not allowed for {kindOfEntity}.");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfEntity), kindOfEntity, null);
            }
        }

        private void ProcessImport(IMonitorLogger logger, CodeItem codeItem, IStorage targetStorage, List<string> deferredLibsList)
        {
            var importsList = codeItem.ImportsList;

            if (importsList.IsNullOrEmpty())
            {
                return;
            }

            if(_isDeferredImport)
            {
                deferredLibsList.AddRange(importsList.Select(p => p.NameValue));
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
