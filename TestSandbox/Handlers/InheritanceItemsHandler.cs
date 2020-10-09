/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

            var list = inheritanceStorage.GetItemsDirectly(subName.GetIndexed(context).NameKey);

            _logger.Log($"list.Count = {list.Count}");
            _logger.Log($"inheritanceItem = {list.WriteListToString()}");

            _logger.Log("End");
        }
    }
}
