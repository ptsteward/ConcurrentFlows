namespace FindingCompletion.ConditionalBranches.Passing {
    using ConcurrentFlows.Dataflow.Helpers;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public class ConditionalBranchingFlow : Dataflow<Message, Message> {

        private static Random rnd = new Random();
        private static TransformBlock<Message, Message> processNegativeData;
        private static TransformBlock<Message, Message> processPositiveData;
        private static ActionBlock<Message> writeToConsole;

        public static CancellationTokenSource cancellationTokenSource { get; } = new CancellationTokenSource();
        public static Task NegativeCompletion { get; private set; }
        public static Task PositiveCompletion { get; private set; }

        public Task FlowCompletion { get; }

        public ConditionalBranchingFlow() : base(CreatePipeline) {
            writeToConsole = new ActionBlock<Message>(msg => {
                Console.WriteLine($"Finished Msg: {msg.Id} Data: {msg.Data}");
            }, new ExecutionDataflowBlockOptions() {
                BoundedCapacity = 10,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationTokenSource.Token
            });
            this.internalBlock.LinkTo(writeToConsole, new DataflowLinkOptions() { PropagateCompletion = true });
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

            processNegativeData = new TransformBlock<Message, Message>(msg => {
                if (msg.Data >= 0) { throw new InvalidOperationException($"Invalid data. Data should be negative but was {msg.Data}."); }
                Task.Delay(rnd.Next(100, 500)).Wait();
                msg.Data *= 5;
                return msg;
            }, executionOptions);
            NegativeCompletion = processNegativeData.Completion;

            processPositiveData = new TransformBlock<Message, Message>(msg => {
                if (msg.Data < 0) { throw new InvalidOperationException($"Invalid data. Data should be positive but was {msg.Data}."); }
                Task.Delay(rnd.Next(100, 500)).Wait();
                msg.Data *= 5;
                return msg;
            }, executionOptions);
            PositiveCompletion = processPositiveData.Completion;

            var outputBuffer = new BufferBlock<Message>();

            var linkOptions = new DataflowLinkOptions() {
                PropagateCompletion = true
            };
            buffer.LinkTo(scaleInputData, linkOptions);
            scaleInputData.LinkTo(processNegativeData, msg => msg.Data < 0);
            scaleInputData.LinkTo(processPositiveData, msg => msg.Data >= 0);            
            processNegativeData.LinkTo(outputBuffer);
            processPositiveData.LinkTo(outputBuffer);
            scaleInputData.Completion.ContinueWith(_ => {
                processNegativeData.Complete();
                processPositiveData.Complete();
            });

            processNegativeData.Completion.ContinueWith(async _ => {
                await processPositiveData.Completion;
                outputBuffer.Complete();
            });

            return new DataflowEndPoints<Message, Message>() {
                Input = buffer,
                Output = outputBuffer
            };
        }
    }
}
