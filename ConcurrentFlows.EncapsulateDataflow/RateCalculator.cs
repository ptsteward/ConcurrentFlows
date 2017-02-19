using System;
using System.Linq;

namespace ConcurrentFlows.EncapsulateDataflow {
    public class RateCalculator : Dataflow<RateCalcConfig, RateCalcResult> {

        public RateCalculator(Func<DataflowEndPoints<RateCalcConfig, RateCalcResult>> createFlow) : base(createFlow) {
        }
    }
}