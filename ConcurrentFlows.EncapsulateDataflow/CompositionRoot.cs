using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace ConcurrentFlows.EncapsulateDataflow {
    [TestFixture]
    public class CompositionRoot {

        [Test]
        public void Main() {
            var rateCalculator = new RateCalculator(CreateRateCalcFlow);
            var domainObject = new SomeDomainObject(rateCalculator);
            var numberOfCalcs = 20;
            var data = Enumerable.Range(1, numberOfCalcs).Select(x => new RateCalcConfig { SomeConfigurationProperty = x });

            var results = domainObject.SimulateCalcs(data);

            var expected = data.Select(x => new RateCalcResult() { Result = x.SomeConfigurationProperty + 2 });
            CollectionAssert.AreEquivalent(expected, results);

        }

        public DataflowEndPoints<RateCalcConfig, RateCalcResult> CreateRateCalcFlow() {
            var executionOptions = new ExecutionDataflowBlockOptions() {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = 5,
                EnsureOrdered = false
            };

            BufferBlock<RateCalcConfig> buffer = new BufferBlock<RateCalcConfig>();
            TransformBlock<RateCalcConfig, int> xform1 = new TransformBlock<RateCalcConfig, int>(x => x.SomeConfigurationProperty + 1, executionOptions);
            TransformBlock<int, int> xform2 = new TransformBlock<int, int>(x => x + 1, executionOptions);
            TransformBlock<int, RateCalcResult> xform3 = new TransformBlock<int, RateCalcResult>(x => new RateCalcResult { Result = x }, executionOptions);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(xform1, linkOptions);
            xform1.LinkTo(xform2, linkOptions);
            xform2.LinkTo(xform3, linkOptions);

            return new DataflowEndPoints<RateCalcConfig, RateCalcResult>(buffer, xform3);
        }
    }
}