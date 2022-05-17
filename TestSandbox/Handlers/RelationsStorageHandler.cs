﻿using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class RelationsStorageHandler
    {
        public RelationsStorageHandler()
        {
            _engineContext = TstEngineContextHelper.CreateAndInitContext().EngineContext;
        }

        private readonly EngineContext _engineContext;
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Case1();

            _logger.Log("End");
        }

        private void Case1()
        {
            var relation = new RelationDescription();
            relation.Name = NameHelper.CreateName("like");

            var argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x1");
            argument.MeaningRolesList.Add(NameHelper.CreateName("subject"));
            argument.MeaningRolesList.Add(NameHelper.CreateName("agent"));

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

            _logger.Log($"relation = {relation}");
            _logger.Log($"relation = {relation.ToHumanizedString()}");

            var globalStorage = _engineContext.Storage.GlobalStorage;

            var inheritanceStorage = globalStorage.InheritanceStorage;

            foreach (var item in relation.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(item);
            }

            globalStorage.RelationsStorage.Append(relation);

            var relationsResolver = _engineContext.DataResolversFactory.GetRelationsResolver();


        }
    }
}
