using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConcurrentFlows.Dataflow.Helpers {

    public abstract class Dataflow<TInput, TOutput> : IPropagatorBlock<TInput, TOutput> {

        protected IPropagatorBlock<TInput, TOutput> internalBlock;

        public Dataflow(Func<DataflowEndPoints<TInput, TOutput>> createDataflow)
            : this(createDataflow.Invoke()) {
        }

        public Dataflow(DataflowEndPoints<TInput, TOutput> endPoints) {
            internalBlock = DataflowBlock.Encapsulate(endPoints.Input, endPoints.Output);
        }

        public Task Completion {
            get {
                return internalBlock.Completion;
            }
        }

        public void Complete() {
            internalBlock.Complete();
        }

        public void Fault(Exception exception) {
            internalBlock.Fault(exception);
        }

        public IDisposable LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions) {
            return internalBlock.LinkTo(target, linkOptions);
        }

        DataflowMessageStatus ITargetBlock<TInput>.OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept) {
            return internalBlock.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        TOutput ISourceBlock<TOutput>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed) {
            return internalBlock.ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        void ISourceBlock<TOutput>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target) {
            internalBlock.ReleaseReservation(messageHeader, target);
        }

        bool ISourceBlock<TOutput>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target) {
            return internalBlock.ReserveMessage(messageHeader, target);
        }
    }
}
