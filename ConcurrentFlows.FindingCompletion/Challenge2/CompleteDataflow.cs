using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using ConcurrentFlows.EncapsulateDataflow;

namespace ConcurrentFlows.FindingCompletion.Challenge2 {
    public class CompleteDataflow<TInput, TOutput> : DataflowEndPoints<TInput, TOutput> {

        public CompleteDataflow(ITargetBlock<TInput> input, ISourceBlock<TOutput> output, IDataflowBlock[] allBlocks) 
            : base(input, output) {
            if (allBlocks == null) { throw new ArgumentException("Argument cannot be null.", "allBlocks"); }
            if (!allBlocks.Contains(input)) { throw new ArgumentException("All blocks collection does not contain input block.", "allBlocks"); }
            if (!allBlocks.Contains(output)) { throw new ArgumentException("All blocks collection does not contain output block.", "allBlocks"); }

            this.AllBlocks = allBlocks;
        }

        public override ITargetBlock<TInput> Input { get; }
        public override ISourceBlock<TOutput> Output { get; }
        public virtual IDataflowBlock[] AllBlocks { get; }
    }
}
