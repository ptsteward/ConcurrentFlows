using System;
using System.Linq;

namespace Dataflow.Series.One {
    public class RateCalculator : Dataflow<RateCalcConfig, RateCalcResult> {

        public RateCalculator(Func<DataflowEndPoints<RateCalcConfig, RateCalcResult>> createFlow) : base(createFlow) {
        }
    }
}