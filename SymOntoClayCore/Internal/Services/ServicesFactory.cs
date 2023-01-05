using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Services
{
    public class ServicesFactory : BaseComponent, IServicesFactory
    {
        public ServicesFactory(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        private EntityConstraintsService _entityConstraintsService;
        private readonly object _entityConstraintsServiceLockObj = new object();

        private CodeFrameService _codeFrameService;
        private readonly object _codeFrameServiceLockObj = new object();

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _entityConstraintsService.Dispose();

            base.OnDisposed();
        }

        /// <inheritdoc/>
        public IEntityConstraintsService GetEntityConstraintsService()
        {
            lock(_entityConstraintsServiceLockObj)
            {
                if (_entityConstraintsService == null)
                {
                    _entityConstraintsService = new EntityConstraintsService(_context);
                }

                return _entityConstraintsService;
            }
        }

        /// <inheritdoc/>
        public ICodeFrameService GetCodeFrameService()
        {
            lock (_codeFrameServiceLockObj)
            {
                if (_codeFrameService == null)
                {
                    _codeFrameService = new CodeFrameService(_context);
                }

                return _codeFrameService;
            }
        }
    }
}
