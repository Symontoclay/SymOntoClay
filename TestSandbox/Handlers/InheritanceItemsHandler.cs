using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using SymOntoClay.CoreHelper.DebugHelpers;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class InheritanceItemsHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            var dictionary = context.Dictionary;

            var inheritanceStorage = context.Storage.GlobalStorage.InheritanceStorage;

            var subName = NameHelper.CreateName("SubClass", dictionary);
            var superName = NameHelper.CreateName("SuperClass", dictionary);

            var inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(1);

            _logger.Log($"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(inheritanceItem);

            inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(0.5F);

            _logger.Log($"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(inheritanceItem);

            inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(0);

            _logger.Log($"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(inheritanceItem);

            var list = inheritanceStorage.GetItemsDirectly(subName.GetIndexed(context));

            _logger.Log($"list.Count = {list.Count}");
            _logger.Log($"inheritanceItem = {list.WriteListToString()}");

            _logger.Log("End");
        }
    }
}
