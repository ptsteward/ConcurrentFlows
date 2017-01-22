using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflow.Series.One.Part2 {

    public class RateCalcConfig {
        public int Id { get; set; }
        public int InputValue { get; set; }
    }

    public class RateCalcResult {
        public int Id { get; set; }
        public int Result { get; set; }
    }
}
