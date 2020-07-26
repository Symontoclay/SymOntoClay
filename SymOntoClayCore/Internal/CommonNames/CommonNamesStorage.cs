using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CommonNames
{
    public class CommonNamesStorage : BaseComponent, ICommonNamesStorage
    {
        public CommonNamesStorage(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        /// <inheritdoc/>
        public Name ApplicationName { get; private set; }

        /// <inheritdoc/>
        public Name ClassName { get; private set; }

        /// <inheritdoc/>
        public Name DefaultHolder { get; private set; }

        public void LoadFromSourceCode()
        {
            ApplicationName = NameHelper.CreateName(StandardNamesConstants.ApplicationTypeName, _context.Dictionary);
            ClassName = NameHelper.CreateName(StandardNamesConstants.ClassTypeName, _context.Dictionary);

            DefaultHolder = new Name();

#if IMAGINE_WORKING
            Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}
