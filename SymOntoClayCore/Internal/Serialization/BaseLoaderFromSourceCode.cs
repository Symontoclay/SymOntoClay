/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
                if(count > 1)
                {
                    throw new NotImplementedException();
                }                
            }
        }

        private List<CodeItem> LinearizeSubItems(List<CodeFile> source)
        {
            var result = new List<CodeItem>();

            foreach(var item in source)
            {
                EnumerateSubItems(item.CodeEntities, result);
            }

            return result.Distinct().ToList();
        }

        private void EnumerateSubItems(List<CodeItem> source, List<CodeItem> result)
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

        private void AddSystemDefinedSettings(List<CodeItem> source)
        {
            foreach(var item in source)
            {
                AddSystemDefinedSettings(item);
            }
        }

        private void AddSystemDefinedSettings(CodeItem codeEntity)
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

                case KindOfCodeEntity.Action:
                    AddSystemDefinedSettingsToAction(codeEntity);
                    break;

                case KindOfCodeEntity.State:
                    AddSystemDefinedSettingsToState(codeEntity);
                    break;

                case KindOfCodeEntity.InlineTrigger:
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
            inheritanceItem.SuperName = _context.CommonNamesStorage.WorldName;
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
            inheritanceItem.SuperName = _context.CommonNamesStorage.AppName;
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
            inheritanceItem.SuperName = _context.CommonNamesStorage.ClassName;
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
            inheritanceItem.SuperName = _context.CommonNamesStorage.ActionName;
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
            inheritanceItem.SuperName = _context.CommonNamesStorage.StateName;
            inheritanceItem.Rank = new LogicalValue(1.0F);

#if DEBUG
            //Log($"inheritanceItem = {inheritanceItem}");
#endif

            codeEntity.InheritanceItems.Add(inheritanceItem);
        }

        private void SaveItems(List<CodeItem> source)
        {
            foreach (var item in source)
            {
                SaveItem(item);
            }
        }

        private void SaveItem(CodeItem codeItem)
        {
#if DEBUG
            //Log($"codeItem = {codeItem}");
#endif

            var codeEntityName = codeItem.Name;

            var globalStorage = _context.Storage.GlobalStorage;

            var metadataStorage = globalStorage.MetadataStorage;

            metadataStorage.Append(codeItem);

            var inheritanceStorage = globalStorage.InheritanceStorage;

            foreach(var inheritanceItem in codeItem.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(inheritanceItem);
            }

#if DEBUG
            //Log($"codeItem (2) = {codeItem}");
#endif

            var kindOfEntity = codeItem.Kind;

            switch(kindOfEntity)
            {
                case KindOfCodeEntity.World:
                    break;

                case KindOfCodeEntity.App:
                    break;

                case KindOfCodeEntity.Class:
                    break;

                case KindOfCodeEntity.InlineTrigger:
                    globalStorage.TriggersStorage.Append(codeItem.AsInlineTrigger);
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

                        globalStorage.LogicalStorage.Append(ruleInstance);
                    }                    
                    break;

                case KindOfCodeEntity.LinguisticVariable:
                    globalStorage.FuzzyLogicStorage.Append(codeItem.AsLinguisticVariable);
                    break;

                case KindOfCodeEntity.Function:
                    globalStorage.MethodsStorage.Append(codeItem.AsNamedFunction);
                    break;

                case KindOfCodeEntity.Action:
                    globalStorage.ActionsStorage.Append(codeItem.AsAction);
                    break;

                case KindOfCodeEntity.State:
                    globalStorage.StatesStorage.Append(codeItem.AsState);
                    break;

                case KindOfCodeEntity.Operator:
                    if(codeItem.AsOperator.KindOfOperator == KindOfOperator.CallFunction)
                    {
                        break;
                    }
                    throw new NotImplementedException();

                case KindOfCodeEntity.Field:
                    {
                        var varResolver = _context.DataResolversFactory.GetVarsResolver();

                        var localCodeExecutionContext = new LocalCodeExecutionContext();
                        localCodeExecutionContext.Holder = codeItem.Holder;
                        localCodeExecutionContext.Storage = globalStorage;

                        var field = codeItem.AsField;

#if DEBUG
                        //Log($"field = {field}");
#endif

                        varResolver.CheckFitVariableAndValue(field, field.Value, localCodeExecutionContext);

                        globalStorage.VarStorage.Append(field);
                    }                    
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfEntity), kindOfEntity, null);
            }

            CheckCodeDirectives(codeItem);

#if DEBUG
            //Log("End");
#endif
        }

        private void CheckCodeDirectives(CodeItem codeItem)
        {
            var directives = codeItem.Directives;

            var kindOfEntity = codeItem.Kind;

            switch(kindOfEntity)
            {
                case KindOfCodeEntity.App:
                    foreach(var directive in directives)
                    {
                        var kindOfDirective = directive.KindOfCodeItemDirective;

                        switch(kindOfDirective)
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
                case KindOfCodeEntity.RuleOrFact:
                case KindOfCodeEntity.LinguisticVariable:
                case KindOfCodeEntity.Function:
                case KindOfCodeEntity.Action:
                case KindOfCodeEntity.State:
                case KindOfCodeEntity.Operator:
                case KindOfCodeEntity.Field:
                    if(directives.Any())
                    {
                        throw new Exception($"Directives does not allowed for {kindOfEntity}.");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfEntity), kindOfEntity, null);
            }
        }
    }
}
