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
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Services
{
    public class EntityConstraintsService : BaseComponent, IEntityConstraintsService
    {
        public EntityConstraintsService(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        private LocalCodeExecutionContext _globalExecutionContext;
        private SynonymsResolver _synonymsResolver;

        private StrongIdentifierValue _randomConstraintName;

        private List<StrongIdentifierValue> _randomConstraintsList;

        private StrongIdentifierValue _nearestConstraintName;
        private List<StrongIdentifierValue> _nearestConstraintsList;

        private List<StrongIdentifierValue> _constraintsList;

        /// <inheritdoc/>
        public void Init()
        {
#if DEBUG
            //Log("Begin");
#endif

            _synonymsResolver = _context.DataResolversFactory.GetSynonymsResolver();

            var commonNamesStorage = _context.CommonNamesStorage;

            _globalExecutionContext = new LocalCodeExecutionContext();
            _globalExecutionContext.Storage = _context.Storage.GlobalStorage;
            _globalExecutionContext.Holder = commonNamesStorage.DefaultHolder;

            _constraintsList = new List<StrongIdentifierValue>();

            _randomConstraintName = commonNamesStorage.RandomConstraintName;

            _randomConstraintsList = new List<StrongIdentifierValue>() { _randomConstraintName };

            var synonymsOfRandomConstraintName = _synonymsResolver.GetSynonyms(_randomConstraintName, _globalExecutionContext);

#if DEBUG
            //Log($"synonymsOfRandomConstraintName = {synonymsOfRandomConstraintName.WriteListToString()}");
#endif

            if(!synonymsOfRandomConstraintName.IsNullOrEmpty())
            {
                _randomConstraintsList.AddRange(synonymsOfRandomConstraintName);
            }

            _nearestConstraintName = commonNamesStorage.NearestConstraintName;

            _nearestConstraintsList = new List<StrongIdentifierValue> { _nearestConstraintName };

            var synonymsOfNearestConstraintName = _synonymsResolver.GetSynonyms(_nearestConstraintName, _globalExecutionContext);

#if DEBUG
            //Log($"synonymsOfNearestConstraintName = {synonymsOfNearestConstraintName.WriteListToString()}");
#endif

            if (!synonymsOfNearestConstraintName.IsNullOrEmpty())
            {
                _nearestConstraintsList.AddRange(synonymsOfNearestConstraintName);
            }

            _constraintsList.AddRange(_nearestConstraintsList);

#if DEBUG
            //Log($"_constraintsList = {_constraintsList.WriteListToString()}");
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        public IList<StrongIdentifierValue> GetConstraintsList()
        {
#if DEBUG
            //Log($"_constraintsList = {_constraintsList.WriteListToString()}");
#endif

            return _constraintsList;
        }

        /// <inheritdoc/>
        public EntityConstraints ConvertToEntityConstraint(StrongIdentifierValue name)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            if(_randomConstraintsList.Contains(name))
            {
                return EntityConstraints.Random;
            }

            if(_nearestConstraintsList.Contains(name))
            {
                return EntityConstraints.Nearest;
            }

            throw new NotImplementedException();
        }
    }
}
