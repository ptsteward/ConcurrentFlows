namespace ConcurrentFlows.DataflowHelpers {
    using System.Threading.Tasks.Dataflow;

    public class DataflowEndPoints<TInput, TOutput> {
        public ITargetBlock<TInput> Input { get; set; }
        public ISourceBlock<TOutput> Output { get; set; }
    }
}
