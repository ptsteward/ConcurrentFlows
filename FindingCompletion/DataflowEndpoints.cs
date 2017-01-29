using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace ConcurrentFlows.Dataflow.Helpers {

    public class DataflowEndPoints<TInput, TOutput> {
        public ITargetBlock<TInput> Input { get; set; }
        public ISourceBlock<TOutput> Output { get; set; }
    }
}
