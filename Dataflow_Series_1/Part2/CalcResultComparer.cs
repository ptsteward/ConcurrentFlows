using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflow.Series.One.Part2 {
    class CalcResultComparer : IComparer {
        public int Compare(object x, object y) {
            var xIn = (RateCalcResult)x;
            var yIn = (RateCalcResult)y;
            return Compare(xIn, yIn);
        }

        public int Compare(RateCalcResult x, RateCalcResult y) {
            if (x.Id > y.Id) { return 1; }
            if (x.Id < x.Id) { return -1; }
            if (x.Result > y.Result) { return 1; }
            if (x.Result < y.Result) { return -1; }

            return 0;
        }
    }
}
