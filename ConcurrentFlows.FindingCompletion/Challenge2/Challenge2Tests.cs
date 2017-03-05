namespace ConcurrentFlows.FindingCompletion.DataflowLoop {
    using System;
    using System.Linq;
    using System.Threading.Tasks;    
    using System.Threading.Tasks.Dataflow;

    using ConcurrentFlows.FindingCompletion.DataflowLoop.Failing;
    using ConcurrentFlows.FindingCompletion;
    using NUnit.Framework;

    [TestFixture]
    public class Challenge2Tests {

        [Test]
        public async Task All_Producers_Are_Complete_When_Pipeline_Completes() {
            var rnd = new Random();
            var conditionalBranchingFlow = new MultipleProducersFlow();
            var messages = Enumerable.Range(0, 20).Select(x => new Message() {
                Id = x,
                Data = rnd.Next(-100, 100),
                ExceededBounds = false
            });

            foreach (var message in messages) {
                conditionalBranchingFlow.Post(message);
            }
            conditionalBranchingFlow.Complete();            

            await conditionalBranchingFlow.FlowCompletion;
                        
            Assert.IsTrue(BothBranchesAreComplete(), PrintWhichBranchIsIncomplete());
        }

        private bool BothBranchesAreComplete() {
            return MultipleProducersFlow.NegativeProducerCompletion.IsCompleted &&
                   MultipleProducersFlow.PositiveProducerCompletion.IsCompleted;
        }

        private string PrintWhichBranchIsIncomplete() {
            return MultipleProducersFlow.NegativeProducerCompletion.IsCompleted ? "Positive branch incomplete" : "Negative branch incomplete";
        }
    }
}
