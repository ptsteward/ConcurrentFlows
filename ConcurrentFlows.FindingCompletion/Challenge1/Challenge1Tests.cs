namespace FindingCompletion.ConditionalBranches {
    using System;
    using System.Linq;
    using System.Threading.Tasks;    
    using System.Threading.Tasks.Dataflow;

    using ConcurrentFlows.FindingCompletion.ConditionalBranches.Passing;
    using ConcurrentFlows.FindingCompletion;

    using NUnit.Framework;

    [TestFixture]
    public class Challenge1Tests {

        [Test]
        public async Task Positive_And_Negative_Branch_Complete_When_Pipeline_Completes() {
            var rnd = new Random();
            var conditionalBranchingFlow = new ConditionalBranchingFlow();
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
            return ConditionalBranchingFlow.NegativeCompletion.IsCompleted &&
                    ConditionalBranchingFlow.PositiveCompletion.IsCompleted;
        }

        private string PrintWhichBranchIsIncomplete() {
            return ConditionalBranchingFlow.NegativeCompletion.IsCompleted ? "Positive branch incomplete" : "Negative branch incomplete";
        }
    }
}
