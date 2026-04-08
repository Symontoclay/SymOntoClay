//using SymOntoClay.Common.DebugHelpers;
//using System.Text;
//using System.Threading;

//namespace SymOntoClay.CoreHelper.Cancellation
//{
//    public class CancellationLinkedTokenSourceContext : BaseCancellationContext
//    {
//        public CancellationLinkedTokenSourceContext(ICancellationContext cancellationContext1, ICancellationContext cancellationContext2)
//        {
//            _cancellationContext1 = cancellationContext1;
//            _cancellationContext2 = cancellationContext2;

//            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationContext1?.Token ?? CancellationToken.None, _cancellationContext2?.Token ?? CancellationToken.None);
//        }

//        private readonly ICancellationContext _cancellationContext1;
//        private readonly ICancellationContext _cancellationContext2;

//        private readonly CancellationTokenSource _cancellationTokenSource;

//        /// <inheritdoc/>
//        public override bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;

//        /// <inheritdoc/>
//        public override CancellationToken Token => _cancellationTokenSource.Token;

//        /// <inheritdoc/>
//        protected override string PropertiesToString(uint n)
//        {
//            var spaces = DisplayHelper.Spaces(n);
//            var sb = new StringBuilder();

//            sb.PrintObjProp(n, nameof(_cancellationContext1), _cancellationContext1);
//            sb.PrintObjProp(n, nameof(_cancellationContext2), _cancellationContext2);

//            sb.Append(base.PropertiesToString(n));
//            return sb.ToString();
//        }
//    }
//}
