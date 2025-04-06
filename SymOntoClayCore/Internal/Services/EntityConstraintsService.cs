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
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Services
{
    public class EntityConstraintsService : BaseContextComponent, IEntityConstraintsService
    {
        public EntityConstraintsService(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _synonymsResolver = _context.DataResolversFactory.GetSynonymsResolver();
        }

        private readonly IMainStorageContext _context;

        private ILocalCodeExecutionContext _globalExecutionContext;
        private SynonymsResolver _synonymsResolver;

        private StrongIdentifierValue _randomConstraintName;

        private List<StrongIdentifierValue> _randomConstraintsList;

        private StrongIdentifierValue _nearestConstraintName;
        private List<StrongIdentifierValue> _nearestConstraintsList;

        private List<StrongIdentifierValue> _constraintsList;

        /// <inheritdoc/>
        public void LoadFromSourceFiles()
        {
            var commonNamesStorage = _context.CommonNamesStorage;

            var globalExecutionContext = new LocalCodeExecutionContext(true);
            globalExecutionContext.Storage = _context.Storage.GlobalStorage;
            globalExecutionContext.Holder = commonNamesStorage.DefaultHolder;

            _globalExecutionContext = globalExecutionContext;

            _constraintsList = new List<StrongIdentifierValue>();

            _randomConstraintName = commonNamesStorage.RandomConstraintName;

            _randomConstraintsList = new List<StrongIdentifierValue>() { _randomConstraintName };

            var synonymsOfRandomConstraintName = _synonymsResolver.GetSynonyms(Logger, _randomConstraintName, _globalExecutionContext);

            if (!synonymsOfRandomConstraintName.IsNullOrEmpty())
            {
                _randomConstraintsList.AddRange(synonymsOfRandomConstraintName);
            }

            _constraintsList.AddRange(_randomConstraintsList);

            _nearestConstraintName = commonNamesStorage.NearestConstraintName;

            _nearestConstraintsList = new List<StrongIdentifierValue> { _nearestConstraintName };

            var synonymsOfNearestConstraintName = _synonymsResolver.GetSynonyms(Logger, _nearestConstraintName, _globalExecutionContext);

            if (!synonymsOfNearestConstraintName.IsNullOrEmpty())
            {
                _nearestConstraintsList.AddRange(synonymsOfNearestConstraintName);
            }

            _constraintsList.AddRange(_nearestConstraintsList);
        }

        /// <inheritdoc/>
        public IList<StrongIdentifierValue> GetConstraintsList()
        {
            return _constraintsList;
        }

        /// <inheritdoc/>
        public EntityConstraints ConvertToEntityConstraint(StrongIdentifierValue name)
        {
            if(_randomConstraintsList.Contains(name))
            {
                return EntityConstraints.Random;
            }

            if(_nearestConstraintsList.Contains(name))
            {
                return EntityConstraints.Nearest;
            }

            throw new NotImplementedException("AA5984CD-A156-4E5E-9DE9-88AE2169080B");
        }
    }
}
