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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;

namespace SymOntoClay.Core.Internal.CommonNames
{
    public class CommonNamesStorage : BaseContextComponent, ICommonNamesStorage
    {
        public CommonNamesStorage(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            WorldName = NameHelper.CreateName(StandardNamesConstants.WorldTypeName);

            AppName = NameHelper.CreateName(StandardNamesConstants.AppTypeName);

            ClassName = NameHelper.CreateName(StandardNamesConstants.ClassTypeName);

            ActionName = NameHelper.CreateName(StandardNamesConstants.ActionTypeName);

            StateName = NameHelper.CreateName(StandardNamesConstants.StateTypeName);

            DefaultHolder = new StrongIdentifierValue();

            SelfSystemVarName = NameHelper.CreateName(StandardNamesConstants.SelfSystemVarName);

            HostSystemVarName = NameHelper.CreateName(StandardNamesConstants.HostSystemVarName);

            AnonymousLogicalVarName = NameHelper.CreateName(StandardNamesConstants.AnonymousLogicalVarName);

            TargetLogicalVarName = NameHelper.CreateName(StandardNamesConstants.TargetLogicalVarName);

            SelfName = _context.SelfName;

            DefaultCtorName = NameHelper.CreateName(StandardNamesConstants.DefaultCtorName);

            RandomConstraintName = NameHelper.CreateName(StandardNamesConstants.RandomConstraintName);

            NearestConstraintName = NameHelper.CreateName(StandardNamesConstants.NearestConstraintName);

            TimeoutAttributeName = NameHelper.CreateName(StandardNamesConstants.TimeoutAttributeName);
            PriorityAttributeName = NameHelper.CreateName(StandardNamesConstants.PriorityAttributeName);

            AnyTypeName = NameHelper.CreateName(StandardNamesConstants.AnyTypeName);
            BooleanTypeName = NameHelper.CreateName(StandardNamesConstants.BooleanTypeName);
            FuzzyTypeName = NameHelper.CreateName(StandardNamesConstants.FuzzyTypeName);
            NumberTypeName = NameHelper.CreateName(StandardNamesConstants.NumberTypeName);

            TrueValueLiteral = NameHelper.CreateName(StandardNamesConstants.TrueValueLiteral);
            FalseValueLiteral = NameHelper.CreateName(StandardNamesConstants.FalseValueLiteral);
        }

        private readonly IMainStorageContext _context;

        /// <inheritdoc/>
        public StrongIdentifierValue WorldName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue AppName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue ClassName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue ActionName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue StateName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue DefaultHolder { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue SelfSystemVarName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue HostSystemVarName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue AnonymousLogicalVarName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue TargetLogicalVarName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue SelfName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue DefaultCtorName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue RandomConstraintName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue NearestConstraintName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue TimeoutAttributeName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue PriorityAttributeName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue AnyTypeName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue BooleanTypeName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue FuzzyTypeName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue NumberTypeName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue TrueValueLiteral { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue FalseValueLiteral { get; private set; }
    }
}
