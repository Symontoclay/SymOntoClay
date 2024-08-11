//using SymOntoClay.ActiveObject.Threads;
//using SymOntoClay.Monitor.Common;
//using SymOntoClay.Threading;
//using System;

//namespace SymOntoClay.ActiveObject.Functors
//{
//    public class LoggedFunctorWithoutResult : BaseFunctorWithoutResult<IMonitorLogger>
//    {
//        public static LoggedFunctorWithoutResult Run(IMonitorLogger logger, Action<IMonitorLogger> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//        {
//            var functor = new LoggedFunctorWithoutResult(logger, action, context, threadPool);
//            functor.Run();
//            return functor;
//        }

//        public LoggedFunctorWithoutResult(IMonitorLogger logger, Action<IMonitorLogger> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//            : base(logger, logger, action, context, threadPool)
//        {
//        }
//    }
    
//    public class LoggedFunctorWithoutResult<T> : BaseFunctorWithoutResult<IMonitorLogger, T>
//    {
//        public static LoggedFunctorWithoutResult<T> Run(IMonitorLogger logger, T arg, Action<IMonitorLogger, T> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//        {
//            var functor = new LoggedFunctorWithoutResult<T>(logger, arg, action, context, threadPool);
//            functor.Run();
//            return functor;
//        }

//        public LoggedFunctorWithoutResult(IMonitorLogger logger, T arg, Action<IMonitorLogger, T> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//            : base(logger, logger, arg, action, context, threadPool)
//        {
//        }
//    }

//    public class LoggedFunctorWithoutResult<T1, T2> : BaseFunctorWithoutResult<IMonitorLogger, T1, T2>
//    {
//        public static LoggedFunctorWithoutResult<T1, T2> Run(IMonitorLogger logger, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//        {
//            var functor = new LoggedFunctorWithoutResult<T1, T2>(logger, arg1, arg2, action, context, threadPool);
//            functor.Run();
//            return functor;
//        }

//        public LoggedFunctorWithoutResult(IMonitorLogger logger, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, IActiveObjectContext context, ICustomThreadPool threadPool)
//            : base(logger, logger, arg1, arg2, action, context, threadPool)
//        {
//        }
//    }
//}
