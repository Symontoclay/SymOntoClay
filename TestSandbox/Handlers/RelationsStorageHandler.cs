/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using TestSandbox.Helpers;

namespace TestSandbox.Handlers
{
    public class RelationsStorageHandler
    {
        public RelationsStorageHandler()
        {
            _engineContext = TstEngineContextHelper.CreateAndInitContext().EngineContext;
        }

        private readonly IEngineContext _engineContext;
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public void Run()
        {
            _logger.Info("9706D239-E6B0-423F-A485-29199FB3AEEF", "Begin");

            CreateCommonRelations();

            _logger.Info("36972AEF-3BF9-4238-B41F-AA3F1047E0A5", "End");
        }
        
        private void CreateCommonRelations()
        {
            CreateLikeRelation();
            CreatePossessRelation();
        }

        private void CreatePossessRelation()
        {
            var relationName = NameHelper.CreateName("possess");

            var relation = new RelationDescription();
            relation.Name = relationName;

            var argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x1");
            argument.MeaningRolesList.Add(NameHelper.CreateName("owner"));

            relation.Arguments.Add(argument);

            argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x2");
            argument.MeaningRolesList.Add(NameHelper.CreateName("possessions"));

            relation.Arguments.Add(argument);

            var inheritanceItem = new InheritanceItem();
            relation.InheritanceItems.Add(inheritanceItem);
            inheritanceItem.SubName = relation.Name;
            inheritanceItem.SuperName = NameHelper.CreateName("state");
            inheritanceItem.Rank = LogicalValue.TrueValue;

            _logger.Info("15521A77-F18C-463D-B109-D5739BC6B8F8", $"relation = {relation.ToHumanizedString()}");

            AppendRelationToStorage(relation);
        }

        private void CreateLikeRelation()
        {
            var relationName = NameHelper.CreateName("like");

            var relation = new RelationDescription();
            relation.Name = relationName;

            var argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x1");
            argument.MeaningRolesList.Add(NameHelper.CreateName("experiencer"));

            relation.Arguments.Add(argument);

            argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x2");
            argument.MeaningRolesList.Add(NameHelper.CreateName("object"));

            relation.Arguments.Add(argument);

            var inheritanceItem = new InheritanceItem();
            relation.InheritanceItems.Add(inheritanceItem);
            inheritanceItem.SubName = relation.Name;
            inheritanceItem.SuperName = NameHelper.CreateName("state");
            inheritanceItem.Rank = LogicalValue.TrueValue;

            _logger.Info("93FBC5A3-E139-4A18-9F56-1916556ECDCC", $"relation = {relation.ToHumanizedString()}");

            AppendRelationToStorage(relation);
        }

        private void AppendRelationToStorage(RelationDescription relation)
        {
            var globalStorage = _engineContext.Storage.GlobalStorage;

            var inheritanceStorage = globalStorage.InheritanceStorage;

            foreach (var item in relation.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(_logger, item);
            }

            globalStorage.RelationsStorage.Append(_logger, relation);
        }
    }
}
