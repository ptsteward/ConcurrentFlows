namespace KeepingOrder {
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;    

    [TestFixture]
    public class KeepingOrderTests {

        [Test]
        public async Task Given_List_Of_Unprocessed_Items_Should_Return_Processed_Items_In_SameOrder() {
            var data = Enumerable.Range(0, 100).Select(x => new Job(x, x)).ToList();
            var pipeline = new OrderedPipeline();

            foreach (var item in data) {
                await pipeline.SendAsync(item);
            }
            pipeline.Complete();
            await pipeline.Completion;
            
            Assert.IsTrue(Enumerable.SequenceEqual<Entity>(data, pipeline.FinalList));
        }
        
    }

    public class Job : Entity {

        public Job(int id, int startValue) : base(id) {            
            this.StartValue = startValue;
        }
        
        public int StartValue { get; }        
    }

    public class JobComplete : Entity {

        public JobComplete(int id, int endValue) : base(id) {
            this.EndValue = endValue;
        }

        public int EndValue { get; }
    }

    public abstract class Entity : IEquatable<Entity> {

        public Entity(int id) {
            this.Id = id;
        }

        public virtual int Id { get; }

        public bool Equals(Entity other) {
            if (this.Id != other.Id) {
                Console.WriteLine($"**{this.Id},{other.Id}**");
            }
            return this.Id == other.Id;
        }

        public override bool Equals(object obj) {
            return Equals((Entity)obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode() * Id.GetHashCode();
        }
    }
}
