using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace ConcurrentFlows.EncapsulateDataflow {
    public class SomeDomainObject {

        private RateCalculator rateCalculator;

        public SomeDomainObject(RateCalculator rateCalculator) {
            this.rateCalculator = rateCalculator;
        }

        public IEnumerable<RateCalcResult> SimulateCalcs(IEnumerable<RateCalcConfig> data) {

            var results = new ConcurrentBag<RateCalcResult>();
            var printResult = new ActionBlock<RateCalcResult>(x => {
                Console.WriteLine(x.Result);
                results.Add(x);
            });
            printResult.Completion.ContinueWith(_ => Console.WriteLine("Finished Calcs"));
            rateCalculator.LinkTo(printResult, new DataflowLinkOptions { PropagateCompletion = true });

            Console.WriteLine("Starting Calcs");

            foreach (var dataPoint in data) {
                rateCalculator.Post(dataPoint);
            }

            Console.WriteLine("DataPosted");

            rateCalculator.Complete();

            printResult.Completion.Wait();
            return results;
        }
    }
}