using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NUnit.Framework;
using System.Threading.Tasks.Dataflow;
using System.Linq;
using System.Collections.Concurrent;

namespace ConcurrentFlows.EncapsulateDataflow {
    public class RateCalcResult {
        public int Result { get; set; }

        public override bool Equals(object obj) {
            return ((RateCalcResult)obj).Result == this.Result;
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ Result.GetHashCode();
        }
    }
}