using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace Dataflow.Series.One.Part2 {

    [TestFixture]
    public class CompositionRoot {

        [Test]
        public void Main() {
            var rateCalculator = new RateCalculator();
            var domainObject = new SomeDomainObject(rateCalculator);
            var numberOfCalcs = 20;
            var data = Enumerable.Range(1, numberOfCalcs).Select(x => new RateCalcConfig { Id = x, InputValue = x }).ToList();

            var results = domainObject.SimulateCalcs(data);

            var expected = data.Select(x => new RateCalcResult() { Id = x.Id, Result = x.InputValue + 2 }).ToList();
            CollectionAssert.AreEqual(expected, results, new CalcResultComparer());

        }        
    }
}