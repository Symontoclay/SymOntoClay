using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
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
            _isDefferedImport = isDefferedImport;
        }

        private readonly IMainStorageContext _context;
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

        public List<string> LoadFromSourceFiles(IStorage targetStorage, string projectFile)
        {
            return LoadFromSourceFiles(targetStorage, projectFile, string.Empty);
        }

        public List<string> LoadFromSourceFiles(IStorage targetStorage, string projectFile, string id)
        {
            Init();

            var filesList = FileHelper.GetParsedFilesInfo(projectFile, id);

#if DEBUG
            //Log($"filesList.Count = {filesList.Count}");

            Log($"filesList = {filesList.WriteListToString()}");
#endif

            var defferedLibsList = new List<string>();

            ProcessFilesList(filesList, true, targetStorage, defferedLibsList);

            return defferedLibsList;
        }

        public List<string> LoadFromPaths(IStorage targetStorage, IList<string> sourceCodePaths)
        {
            Init();

#if DEBUG
            //Log("Begin");
            //Log($"sourceCodePaths = {sourceCodePaths.WritePODListToString()}");
#endif

            var filesList = FileHelper.GetParsedFilesFromPaths(sourceCodePaths);

#if DEBUG
            //Log($"filesList.Count = {filesList.Count}");

            //Log($"filesList = {filesList.WriteListToString()}");
#endif

            var defferedLibsList = new List<string>();

            ProcessFilesList(filesList, false, targetStorage, defferedLibsList);

            return defferedLibsList;
        }

        private void ProcessFilesList(List<ParsedFileInfo> filesList, bool detectMainCodeEntity, IStorage targetStorage, List<string> defferedLibsList)
        {
            //var globalStorage = _context.Storage.GlobalStorage;

#if DEBUG
            //Log($"globalStorage.Kind = {globalStorage.Kind}");
#endif

            var parsedFilesList = _context.Parser.Parse(filesList, _defaultSettingsOfCodeEntity);

#if DEBUG
            //Log($"parsedFilesList.Count = {parsedFilesList.Count}");

            //Log($"parsedFilesList = {parsedFilesList.WriteListToString()}");
#endif

            var parsedCodeEntitiesList = LinearizeSubItems(parsedFilesList);

#if DEBUG
            //Log($"parsedCodeEntitiesList.Count = {parsedCodeEntitiesList.Count}");

            //Log($"parsedCodeEntitiesList = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            if (detectMainCodeEntity)
            {
                DetectMainCodeEntity(parsedCodeEntitiesList);
            }

#if DEBUG
            //Log($"parsedCodeEntitiesList.Count (2) = {parsedCodeEntitiesList.Count}");

            //Log($"parsedCodeEntitiesList (2) = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            CheckHolderAndTypeOfAccess(parsedCodeEntitiesList);

            AddSystemDefinedSettings(parsedCodeEntitiesList);

#if DEBUG
            //Log($"parsedCodeEntitiesList.Count (3) = {parsedCodeEntitiesList.Count}");

            //Log($"parsedCodeEntitiesList (3) = {parsedCodeEntitiesList.WriteListToString()}");
#endif

            SaveItems(parsedCodeEntitiesList, targetStorage, defferedLibsList);
        }

        private void DetectMainCodeEntity(List<CodeItem> source)
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

#if DEBUG
            //Log($"codeEntity = {codeEntity}");
#endif

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
#if DEBUG
            //Log($"codeEntity = {codeEntity}");
#endif

            switch (codeEntity.Kind)
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

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

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

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

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

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

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

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

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

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

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
#if DEBUG
            //Log($"codeItem = {codeItem}");
#endif

            var codeEntityName = codeItem.Name;

            var metadataStorage = targetStorage.MetadataStorage;

            metadataStorage.Append(codeItem);

            var inheritanceStorage = targetStorage.InheritanceStorage;

            foreach (var inheritanceItem in codeItem.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(inheritanceItem);
            }

#if DEBUG
            //Log($"codeItem (2) = {codeItem}");
#endif

            var kindOfEntity = codeItem.Kind;

            switch (kindOfEntity)
            {
                case KindOfCodeEntity.World:
                    ProcessImport(codeItem, defferedLibsList);
                    break;

                case KindOfCodeEntity.App:
                    ProcessImport(codeItem, defferedLibsList);
                    break;

                case KindOfCodeEntity.Class:
                    break;

                case KindOfCodeEntity.InlineTrigger:
                    targetStorage.TriggersStorage.Append(codeItem.AsInlineTrigger);
                    break;

                case KindOfCodeEntity.RelationDescription:
#if DEBUG
                    //Log($"targetStorage.Kind = {targetStorage.Kind}");
                    //Log($"codeItem.AsRelationDescription.ToHumanizedString() = {codeItem.AsRelationDescription.ToHumanizedString()}");
#endif

                    targetStorage.RelationsStorage.Append(codeItem.AsRelationDescription);
                    break;

                case KindOfCodeEntity.RuleOrFact:
                    {
                        var ruleInstance = codeItem.AsRuleInstance;

#if DEBUG
                        //Log($"ruleInstance = {ruleInstance}");
#endif

                        if (ruleInstance.IsParameterized)
                        {
                            throw new Exception($"SymOntoClay does not support parameterized rule or facts on object declaration.");
                        }

#if DEBUG
                        //if (targetStorage.Kind == KindOfStorage.World)
                        //{
                        //    Log($"targetStorage.Kind = {targetStorage.Kind}");
                        //    Log($"targetStorage.LogicalStorage.GetHashCode() = {targetStorage.LogicalStorage.GetHashCode()}");
                        //    Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");
                        //}
#endif

                        targetStorage.LogicalStorage.Append(ruleInstance);

#if DEBUG
                        //if (globalStorage.Kind == KindOfStorage.World)
                        //{
                        //    globalStorage.LogicalStorage.DbgPrintFactsAndRules();
                        //}
#endif
                    }
                    break;

                case KindOfCodeEntity.LinguisticVariable:
                    targetStorage.FuzzyLogicStorage.Append(codeItem.AsLinguisticVariable);
                    break;

                case KindOfCodeEntity.Function:
                    targetStorage.MethodsStorage.Append(codeItem.AsNamedFunction);
                    break;

                case KindOfCodeEntity.Action:
                    targetStorage.ActionsStorage.Append(codeItem.AsAction);
                    break;

                case KindOfCodeEntity.State:
                    targetStorage.StatesStorage.Append(codeItem.AsState);
                    break;

                case KindOfCodeEntity.Operator:
                    if (codeItem.AsOperator.KindOfOperator == KindOfOperator.CallFunction)
                    {
                        break;
                    }
                    throw new NotImplementedException();

                case KindOfCodeEntity.Field:
                    {
                        var varResolver = _context.DataResolversFactory.GetVarsResolver();

                        var localCodeExecutionContext = new LocalCodeExecutionContext();
                        localCodeExecutionContext.Holder = codeItem.Holder;
                        localCodeExecutionContext.Storage = _globalStorage;

                        var field = codeItem.AsField;

#if DEBUG
                        //Log($"field = {field}");
#endif

                        varResolver.CheckFitVariableAndValue(field, field.Value, localCodeExecutionContext);

                        targetStorage.VarStorage.Append(field);
                    }
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

#if DEBUG
            //Log("End");
#endif
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
                case KindOfCodeEntity.Class:
                case KindOfCodeEntity.InlineTrigger:
                case KindOfCodeEntity.RelationDescription:
                case KindOfCodeEntity.RuleOrFact:
                case KindOfCodeEntity.LinguisticVariable:
                case KindOfCodeEntity.Function:
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

        private void ProcessImport(CodeItem codeItem, List<string> defferedLibsList)
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

#if DEBUG
            Log($"libsList.Count = {libsList.Count}");
#endif

            throw new NotImplementedException();
        }
    }
}
