using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CommonNames
{
    public class CommonNamesStorage : BaseComponent, ICommonNamesStorage
    {
        public CommonNamesStorage(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        /// <inheritdoc/>
        public StrongIdentifierValue ApplicationName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedApplicationName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue ClassName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedClassName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue DefaultHolder { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedDefaultHolder { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue SelfSystemVarName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedSelfSystemVarName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue HostSystemVarName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedHostSystemVarName { get; private set; }

        public void LoadFromSourceCode()
        {
            ApplicationName = NameHelper.CreateName(StandardNamesConstants.ApplicationTypeName, _context.Dictionary);

            IndexedApplicationName = ApplicationName.GetIndexed(_context);

            ClassName = NameHelper.CreateName(StandardNamesConstants.ClassTypeName, _context.Dictionary);
            IndexedClassName = ClassName.GetIndexed(_context);

            DefaultHolder = new StrongIdentifierValue();
            IndexedDefaultHolder = DefaultHolder.GetIndexed(_context);

            SelfSystemVarName = NameHelper.CreateName(StandardNamesConstants.SelfSystemVarName, _context.Dictionary);
            IndexedSelfSystemVarName = SelfSystemVarName.GetIndexed(_context);

            HostSystemVarName = NameHelper.CreateName(StandardNamesConstants.HostSystemVarName, _context.Dictionary);
            IndexedHostSystemVarName = HostSystemVarName.GetIndexed(_context);

#if IMAGINE_WORKING
            //Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}
