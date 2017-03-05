namespace ConcurrentFlows.FindingCompletion.Challenge2 {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    using ConcurrentFlows.EncapsulateDataflow;
    using ConcurrentFlows.FindingCompletion;

    public abstract class LoopDataflow<TInput, TOutput> : Dataflow<TInput, TOutput> {

        private BufferBlock<TInput> InputBuffer { get; } = new BufferBlock<TInput>();

        private BufferBlock<TOutput> OutputBuffer { get; } = new BufferBlock<TOutput>();

        protected override IPropagatorBlock<TInput, TOutput> InternalBlock { get; }        

        public LoopDataflow(Func<DataflowEndPoints<TInput, TOutput>> createDataflow)
            : this(createDataflow.Invoke()) {
        }

        public LoopDataflow(DataflowEndPoints<TInput, TOutput> endPoints) : base(endPoints) {            
            InternalBlock = AddBuffersToLoop();
        }

        private IPropagatorBlock<TInput, TOutput> AddBuffersToLoop() {
            InputBuffer.LinkTo(base.InternalBlock, new DataflowLinkOptions() { PropagateCompletion = false });
            base.InternalBlock.LinkTo(OutputBuffer, new DataflowLinkOptions() { PropagateCompletion = true });
            ConfigureCompletion();
            return DataflowBlock.Encapsulate(InputBuffer, OutputBuffer);
        }

        private void ConfigureCompletion() {
                        
        }
        
    }
}
