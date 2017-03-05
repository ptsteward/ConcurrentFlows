namespace ConcurrentFlows.FindingCompletion.DataflowLoop.Failing {    
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    using ConcurrentFlows.EncapsulateDataflow;
    using ConcurrentFlows.FindingCompletion;

    public class MultipleProducersFlow : Dataflow<Message, Message> {

        private static Random rnd = new Random();
        //private static TransformBlock<Message, Message> negativeDataProducer;
        //private static TransformBlock<Message, Message> positiveDataProducer;
        private ActionBlock<Message> writeToConsole;

        public static CancellationTokenSource cancellationTokenSource { get; } = new CancellationTokenSource();
        public static Task NegativeProducerCompletion { get; private set; }
        public static Task PositiveProducerCompletion { get; private set; }

        public Task FlowCompletion { get; }

        public MultipleProducersFlow() : base(CreatePipeline) {
            writeToConsole = new ActionBlock<Message>(msg => {
                Console.WriteLine($"Finished Msg: {msg.Id} Data: {msg.Data}");
            }, new ExecutionDataflowBlockOptions() {
                BoundedCapacity = 10,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationTokenSource.Token
            });
            this.InternalBlock.LinkTo(writeToConsole, new DataflowLinkOptions() { PropagateCompletion = true });
            FlowCompletion = writeToConsole.Completion;
        }

        private static DataflowEndPoints<Message, Message> CreatePipeline() {
            var buffer = new BufferBlock<Message>();

            var executionOptions = new ExecutionDataflowBlockOptions() {
                BoundedCapacity = 10,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationTokenSource.Token
            };

            var scaleInputData = new TransformBlock<Message, Message>(msg => {
                msg.Data *= 10;
                return msg;
            }, executionOptions);

            var outputBuffer = new BufferBlock<Message>();

            return new DataflowEndPoints<Message, Message>(buffer, outputBuffer);
        }
    }
}
