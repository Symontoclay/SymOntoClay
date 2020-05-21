using SymOntoClay.Core.Internal.CodeModel;
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

        public IName ApplicationName { get; private set; }

        public void LoadFromSourceCode()
        {
            var applicationName = new Name();
            applicationName.OriginalValue = StandardNamesConstants.ApplicationTypeName;
            applicationName.OriginalKey = _context.Dictionary.GetKey(applicationName.OriginalValue);
            applicationName.Key = applicationName.OriginalKey;

            ApplicationName = applicationName;

#if IMAGINE_WORKING
            Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}
