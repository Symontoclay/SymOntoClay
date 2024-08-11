//using SymOntoClay.ActiveObject.Threads;
//using SymOntoClay.Monitor.Common;
//using SymOntoClay.Threading;
//using System;
//using System.Threading;

//namespace SymOntoClay.ActiveObject.Functors
//{
//    public class BaseFunctorWithoutResult : BaseFunctor
//    {
//        public static BaseFunctorWithoutResult Run(IMonitorLogger logger, Action action, IActiveObjectContext context, ICustomThreadPool threadPool)
//        {
//            var functor = new BaseFunctorWithoutResult(logger, action, context, threadPool);
//            functor.Run();
//            return functor;
//        }

//        public BaseFunctorWithoutResult(IMonitorLogger logger, Action action, IActiveObjectContext context, ICustomThreadPool threadPool)
//            : base(logger, context, threadPool)
//        {
//            _action = action;
//        }

//        private readonly Action _action;

//        /// <inheritdoc/>
//        protected override void OnRun(CancellationToken cancellationToken)
//        {
//            _action();
//        }
//    }

//    public class BaseFunctorWithoutResult<T> : BaseFunctor
//    {
//        public BaseFunctorWithoutResult(IMonitorLogger logger, T arg, Action<T> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//            : base(logger, context, threadPool)
//        {
//            _action = action;
//            _arg = arg;
//        }

//        private readonly Action<T> _action;
//        private readonly T _arg;

//        /// <inheritdoc/>
//        protected override void OnRun(CancellationToken cancellationToken)
//        {
//            _action(_arg);
//        }
//    }

//    public class BaseFunctorWithoutResult<T1, T2> : BaseFunctor
//    {
//        public BaseFunctorWithoutResult(IMonitorLogger logger, T1 arg1, T2 arg2, Action<T1, T2> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//            : base(logger, context, threadPool)
//        {
//            _action = action;
//            _arg1 = arg1;
//            _arg2 = arg2;
//        }

//        private readonly Action<T1, T2> _action;
//        private readonly T1 _arg1;
//        private readonly T2 _arg2;

//        /// <inheritdoc/>
//        protected override void OnRun(CancellationToken cancellationToken)
//        {
//            _action(_arg1, _arg2);
//        }
//    }

//    public class BaseFunctorWithoutResult<T1, T2, T3> : BaseFunctor
//    {
//        public BaseFunctorWithoutResult(IMonitorLogger logger, T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//            : base(logger, context, threadPool)
//        {
//            _action = action;
//            _arg1 = arg1;
//            _arg2 = arg2;
//            _arg3 = arg3;
//        }

//        private readonly Action<T1, T2, T3> _action;
//        private readonly T1 _arg1;
//        private readonly T2 _arg2;
//        private readonly T3 _arg3;

//        /// <inheritdoc/>
//        protected override void OnRun(CancellationToken cancellationToken)
//        {
//            _action(_arg1, _arg2, _arg3);
//        }
//    }
//}
