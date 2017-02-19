using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace ConcurrentFlows.EncapsulateDataflow.Part2 {

    public class RateCalculator : Dataflow<RateCalcConfig, RateCalcResult> {

        public RateCalculator() : base(CreateRateCalcPipeline) {
        }

        private static DataflowEndPoints<RateCalcConfig, RateCalcResult> CreateRateCalcPipeline() {
            var executionOptions = new ExecutionDataflowBlockOptions() {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = 5,
                EnsureOrdered = true
            };

            BufferBlock<RateCalcConfig> buffer = new BufferBlock<RateCalcConfig>();
            TransformBlock<RateCalcConfig, int> xform1 = new TransformBlock<RateCalcConfig, int>(x => x.InputValue + 1, executionOptions);
            TransformBlock<int, int> xform2 = new TransformBlock<int, int>(x => x + 1, executionOptions);
            TransformBlock<int, RateCalcResult> xform3 = new TransformBlock<int, RateCalcResult>(x => new RateCalcResult { Id = x - 2, Result = x }, executionOptions);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(xform1, linkOptions);
            xform1.LinkTo(xform2, linkOptions);
            xform2.LinkTo(xform3, linkOptions);

            return new DataflowEndPoints<RateCalcConfig, RateCalcResult>() {
                Input = buffer,
                Output = xform3
            };
        }
    }
}