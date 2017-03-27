namespace ConcurrentFlows.FindingCompletion {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Threading.Tasks.Dataflow;

    using NUnit.Framework;

    [TestFixture]
    public class ObservableDataflowTests {

        [Test]
        public async Task Can_TransformBlock_Be_Observable_And_Linked() {
            var totalValuesRecievedByActionBlock = new List<int>();
            var totalValuesRecievedInStream = new List<int>();
            var streamCompletionSource = new TaskCompletionSource<bool>();

            var transformBlock = new TransformBlock<int, int>(_ => _);
            var actionBlock = new ActionBlock<int>(x => Task.Delay(1000).ContinueWith(_ => totalValuesRecievedByActionBlock.Add(x)), new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 });         

            var subscription = transformBlock.AsObservable().Subscribe(x => totalValuesRecievedInStream.Add(x),
                                                                       () => streamCompletionSource.SetResult(true));

            var actionLink = transformBlock.LinkTo(actionBlock, new DataflowLinkOptions() { PropagateCompletion = true });


            foreach (var item in Enumerable.Range(0, 100)) {
                transformBlock.Post(item);
            }
            await Task.Delay(2000);
            transformBlock.Complete();
            await actionBlock.Completion;
            await streamCompletionSource.Task;                

            Assert.AreEqual(totalValuesRecievedByActionBlock, totalValuesRecievedInStream);
        }
    }
}
