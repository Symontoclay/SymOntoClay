/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorage : BaseLoggedComponent, IStorage
    {
        public RealStorage(KindOfStorage kind, RealStorageSettings settings)
            : base(settings.MainStorageContext.Logger)
        {
#if DEBUG
            //Log($"kind = {kind}");
            //Log($"settings = {settings}");
#endif

            _kind = kind;
            _realStorageContext = new RealStorageContext();
            _realStorageContext.Storage = this;
            _realStorageContext.MainStorageContext = settings.MainStorageContext;

            DefaultSettingsOfCodeEntity = settings.DefaultSettingsOfCodeEntity;

            var parents = settings.ParentsStorages;

            if(parents == null)
            {
                _realStorageContext.Parents = new List<IStorage>();
            }
            else
            {
                _realStorageContext.Parents = parents;
            }

            _realStorageContext.LogicalStorage = new LogicalStorage.LogicalStorage(_kind, _realStorageContext);
            _realStorageContext.MethodsStorage = new MethodsStorage.MethodsStorage(_kind, _realStorageContext);
            _realStorageContext.TriggersStorage = new TriggersStorage.TriggersStorage(_kind, _realStorageContext);
            _realStorageContext.InheritanceStorage = new InheritanceStorage.InheritanceStorage(_kind, _realStorageContext);
            _realStorageContext.SynonymsStorage = new SynonymsStorage.SynonymsStorage(_kind, _realStorageContext);
            _realStorageContext.OperatorsStorage = new OperatorsStorage.OperatorsStorage(_kind, _realStorageContext);
            _realStorageContext.ChannelsStorage = new ChannelsStorage.ChannelsStorage(_kind, _realStorageContext);
            _realStorageContext.MetadataStorage = new MetadataStorage.MetadataStorage(_kind, _realStorageContext);
            _realStorageContext.VarStorage = new VarStorage.VarStorage(_kind, _realStorageContext);
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public ILogicalStorage LogicalStorage => _realStorageContext.LogicalStorage;

        /// <inheritdoc/>
        public IMethodsStorage MethodsStorage => _realStorageContext.MethodsStorage;

        /// <inheritdoc/>
        public ITriggersStorage TriggersStorage => _realStorageContext.TriggersStorage;

        /// <inheritdoc/>
        public IInheritanceStorage InheritanceStorage => _realStorageContext.InheritanceStorage;
   
        /// <inheritdoc/>
        public ISynonymsStorage SynonymsStorage => _realStorageContext.SynonymsStorage;

        /// <inheritdoc/>
        public IOperatorsStorage OperatorsStorage => _realStorageContext.OperatorsStorage;

        /// <inheritdoc/>
        public IChannelsStorage ChannelsStorage => _realStorageContext.ChannelsStorage;

        /// <inheritdoc/>
        public IMetadataStorage MetadataStorage => _realStorageContext.MetadataStorage;

        /// <inheritdoc/>
        public IVarStorage VarStorage => _realStorageContext.VarStorage;

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<StorageUsingOptions> result, int level)
        {
#if DEBUG
            //Log($"result?.Count = {result?.Count}");
            //Log($"level = {level}");
#endif

            level++;

            var item = new StorageUsingOptions()
            {
                Priority = level,
                Storage = this,
                UseFacts = true,
                UseProductions = true,
                UseAdditionalInstances = true
            };

            result.Add(item);

            var parentsList = _realStorageContext.Parents;

            if (parentsList.Any())
            {
                foreach(var parent in parentsList)
                {
                    parent.CollectChainOfStorages(result, level);
                }
            }
        }

        /// <inheritdoc/>
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            return sb.ToString();
        }
    }
}
