namespace ConcurrentFlows.FindingCompletion.Challenge2 {
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class LoopDataflowTests {

        [Test]
        public void Every_Other_Message_Should_Produce_One_Extra_Message() {
            var intialData = Enumerable.Range(0, 10).Select(x => new Message(x, x % 2 == 0 ? 1 : 0));

            var actualData = intialData.SelectMany(data => {
                if (data.GenerateNewMessages == 0) {
                    return new Message[] { data };
                }
                return Enumerable.Range(0, data.GenerateNewMessages + 1).Select(x => new Message(x, 0));
            });

            var actualCount = actualData.Count();

            Assert.AreEqual(15, actualCount);
        }
        
        [Test]
        public async Task flow1_All_Messages_Should_Be_Produced_And_Completed_Upon_Completion() {
            var loopDataflow = new LoopDataflow1();
            var initialData = Enumerable.Range(0, 100)
                                        .Select(x => new Message(x, x % 2 == 0 ? 1 : 0));
            

            loopDataflow.Post(initialData);

            loopDataflow.Complete();
            await loopDataflow.Completion;
            
            var expectedOutputCount = 150;
            var outputCount = loopDataflow.Output.Count();
            Assert.AreEqual(expectedOutputCount, outputCount);
        }

        [Test]
        public async Task flow2_All_Messages_Should_Be_Produced_And_Completed_Upon_Completion() {
            var loopDataflow = new LoopDataflow2();
            var initialData = Enumerable.Range(0, 100)
                                        .Select(x => new Message(x, x % 2 == 0 ? 1 : 0));


            loopDataflow.Post(initialData);

            loopDataflow.Complete();
            await loopDataflow.Completion;
            
            var expectedOutputCount = 200;
            var outputCount = loopDataflow.Output.Count();
            Assert.AreEqual(expectedOutputCount, outputCount);
        }

        [Test]
        public async Task flow3_All_Messages_Should_Be_Produced_And_Completed_Upon_Completion() {
            var loopDataflow = new LoopDataflow3();
            var initialData = Enumerable.Range(0, 100)
                                        .Select(x => new Message(x, x % 2 == 0 ? 1 : 0));


            loopDataflow.Post(initialData);

            loopDataflow.Complete();
            await loopDataflow.Completion;

            var expectedOutputCount = 150;
            var outputCount = loopDataflow.Output.Count();
            Assert.AreEqual(expectedOutputCount, outputCount);
        }

        [Test]
        public async Task flow4_All_Messages_Should_Be_Produced_And_Completed_Upon_Completion() {
            var loopDataflow = new LoopDataflow4();
            var initialData = Enumerable.Range(0, 100)
                                        .Select(x => new Message(x, x % 2 == 0 ? 1 : 0));


            loopDataflow.Post(initialData);

            loopDataflow.Complete();
            await loopDataflow.Completion;

            var expectedOutputCount = 200;
            var outputCount = loopDataflow.Output.Count();
            Assert.AreEqual(expectedOutputCount, outputCount);
        }

        [Test]
        public async Task flow5_All_Messages_Should_Be_Produced_And_Completed_Upon_Completion() {
            var loopDataflow = new LoopDataflow5();
            var initialData = Enumerable.Range(0, 100)
                                        .Select(x => new Message(x, x % 2 == 0 ? 1 : 0));


            loopDataflow.Post(initialData);

            loopDataflow.Complete();
            await loopDataflow.Completion;

            var expectedOutputCount = 150;
            var outputCount = loopDataflow.Output.Count();
            Assert.AreEqual(expectedOutputCount, outputCount);
        }

        [Test]
        public void flow5_Timeout_On_LoopComplete_Set_To_1ms() {
            var loopDataflow = new LoopDataflow5();
            var initialData = Enumerable.Range(0, 100)
                                        .Select(x => new Message(x, x % 2 == 0 ? 1 : 0));

            loopDataflow.Post(initialData);

            loopDataflow.Complete();

            Assert.ThrowsAsync<TimeoutException>(() => loopDataflow.Completion);
        }
    }
}
