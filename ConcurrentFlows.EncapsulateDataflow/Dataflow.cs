namespace ConcurrentFlows.EncapsulateDataflow {
    using System;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public abstract class Dataflow<TInput, TOutput> : IPropagatorBlock<TInput, TOutput> {

        protected virtual IPropagatorBlock<TInput, TOutput> InternalBlock { get; }

        public Dataflow(Func<DataflowEndPoints<TInput, TOutput>> createDataflow)
            : this(createDataflow.Invoke()) {
        }

        public Dataflow(DataflowEndPoints<TInput, TOutput> endPoints) {
            InternalBlock = DataflowBlock.Encapsulate(endPoints.Input, endPoints.Output);
        }

        public virtual Task Completion {
            get {
                return InternalBlock.Completion;
            }
        }

        public virtual void Complete() {
            InternalBlock.Complete();
        }

        public virtual IDisposable LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions) {
            return InternalBlock.LinkTo(target, linkOptions);
        }

        void IDataflowBlock.Fault(Exception exception) {
            InternalBlock.Fault(exception);
        }

        DataflowMessageStatus ITargetBlock<TInput>.OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept) {
            return InternalBlock.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        TOutput ISourceBlock<TOutput>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed) {
            return InternalBlock.ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        void ISourceBlock<TOutput>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target) {
            InternalBlock.ReleaseReservation(messageHeader, target);
        }

        bool ISourceBlock<TOutput>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target) {
            return InternalBlock.ReserveMessage(messageHeader, target);
        }
    }
}
