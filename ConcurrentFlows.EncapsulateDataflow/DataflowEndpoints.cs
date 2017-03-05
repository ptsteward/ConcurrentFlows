using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace ConcurrentFlows.EncapsulateDataflow {

    public class DataflowEndPoints<TInput, TOutput> {

        public DataflowEndPoints(ITargetBlock<TInput> input, ISourceBlock<TOutput> output) {
            if (input == null) { throw new ArgumentException("Argument cannot be null.", "input"); }
            if (output == null) { throw new ArgumentException("Argument cannot be null.", "output"); }

            this.Input = input;
            this.Output = output;
        }

        public virtual ITargetBlock<TInput> Input { get; }
        public virtual ISourceBlock<TOutput> Output { get; }
    }
}
