namespace ConcurrentFlows.FindingCompletion.Challenge2 {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public class LoopDataflow2 {

        public LoopDataflow2() {
            var options = new ExecutionDataflowBlockOptions() { BoundedCapacity = 100 };
            InputMessageBlock = new TransformBlock<Message, Message>(async msg => await InputMessage(msg));
            HandleMessageBlock = new TransformManyBlock<Message, Message>(async msg => await HandleMessage(msg), options);
            HandleMessageBlock2 = new TransformManyBlock<Message, Message>(async msg => await HandleMessage2(msg), options);
            OutputMessageBlock = new ActionBlock<Message>(msg => OutputMessage(msg), options);

            var linkOptions = new DataflowLinkOptions() { PropagateCompletion = false };
            InputMessageBlock.LinkTo(HandleMessageBlock, linkOptions);
            HandleMessageBlock.LinkTo(OutputMessageBlock, linkOptions, msg => msg.WasProcessed == true);
            HandleMessageBlock.LinkTo(HandleMessageBlock2, linkOptions, msg => msg.WasProcessed == false);
            HandleMessageBlock2.LinkTo(OutputMessageBlock, linkOptions, msg => msg.WasProcessed == true);
            HandleMessageBlock2.LinkTo(HandleMessageBlock, linkOptions, msg => msg.WasProcessed == false);

            InputMessageBlock.Completion.ContinueWith(async tsk => {
                await BothMessageHandlersAreComplete();
                HandleMessageBlock.Complete();
            });

            HandleMessageBlock.Completion.ContinueWith(tsk => {
                HandleMessageBlock2.Complete();
            });

            HandleMessageBlock2.Completion.ContinueWith(tsk => {
                OutputMessageBlock.Complete();
            });
            DebuggingLoop();
        }

        private async Task<bool> BothMessageHandlersAreComplete() {
            while (!(HandleMessageIsComplete() &&
                     HandleMessage2IsComplete())) {
                await Task.Delay(100);
            }
            return true;
        }

        private bool HandleMessageIsComplete() {
            return handlingMessages == 0 &&
                   HandleMessageBlock.InputCount == 0 &&
                   HandleMessageBlock.OutputCount == 0;
        }

        private bool HandleMessage2IsComplete() {
            return handlingMessages2 == 0 &&
                   HandleMessageBlock2.InputCount == 0 &&
                   HandleMessageBlock2.OutputCount == 0;
        }

        private int handlingMessages;

        private int handlingMessages2;

        public async void DebuggingLoop() {
            while (true) {
                await Task.Delay(1000);
            }
        }

        public Task Completion {
            get { return OutputMessageBlock.Completion; }
        }

        public IList<Message> Output {
            get;
            private set;
        }

        public void Post(IEnumerable<Message> data) {
            foreach (var item in data) {
                InputMessageBlock.Post(item);
            }
        }

        public void Complete() {
            InputMessageBlock.Complete();
        }

        private TransformBlock<Message, Message> InputMessageBlock {
            get;
        }

        private async Task<Message> InputMessage(Message message) {
            await Task.Delay(10);
            return message;
        }

        private TransformManyBlock<Message, Message> HandleMessageBlock {
            get;
        }

        private async Task<IEnumerable<Message>> HandleMessage(Message message) {
            Interlocked.Increment(ref handlingMessages);
            await Task.Delay(10);
            var result = Enumerable.Range(0, message.GenerateNewMessages + 1)
                                   .Select(x => new Message(x, x % 2 == 1 ? 1 : 0, x % 2 == 0));
            Interlocked.Decrement(ref handlingMessages);
            return result;
        }

        private TransformManyBlock<Message, Message> HandleMessageBlock2 {
            get;
        }

        private async Task<IEnumerable<Message>> HandleMessage2(Message message) {
            Interlocked.Increment(ref handlingMessages2);
            await Task.Delay(10);
            var result = Enumerable.Range(0, message.GenerateNewMessages + 1)
                                   .Select(x => new Message(x, 0, x % 2 == 0));
            Interlocked.Decrement(ref handlingMessages2);
            return result;
        }

        private ActionBlock<Message> OutputMessageBlock {
            get;
        }

        private void OutputMessage(Message message) {
            if (Output == null) Output = new List<Message>();
            Output.Add(message);
        }
    }
}
