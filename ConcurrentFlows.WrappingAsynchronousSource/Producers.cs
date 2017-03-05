//namespace ConcurrentFlows.WrappingAsynchronousSource {
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;
//    using System.Threading.Tasks.Dataflow;

//    using ConcurrentFlows.DataflowHelpers;

//    public interface IProduce<TInput, TResult> : ISourceBlock<TInput> {

//        Task<TResult> Produce(TInput data);
//    }    

//    public sealed class Job<TInput, TResult> {        

//        public Job(TInput data) {

//        }
//    }

//    public class JobHandler<TInput, TResult> : Dataflow<Job<TInput, TResult>, TResult> {

//    }

//    public abstract class Producer<TInput, TResult> : IProduce<KeyValuePair<TaskCompletionSource<TResult>, TInput>, <KeyValuePair<Guid, TResult>> {

//        private BufferBlock<>
//        private Dataflow<KeyValuePair<Guid, TInput>, KeyValuePair<Guid, TResult>> ResultCalculator { get; }
//        private bool ShouldProduce { get; set; }

//        public Task Completion { get { return ResultCalculator.Completion; } }

//        public Producer(Dataflow<KeyValuePair<Guid, TInput>, KeyValuePair<Guid, TResult>> resultCalculator) {
//            if (resultCalculator == null) { throw new ArgumentException("Argument cannot be null.", "resultCalculator"); }

//            this.ResultCalculator = resultCalculator;
//            this.ShouldProduce = false;
//        }

//        public async Task<TResult> Produce(TInput data) {
//            var id = Guid.NewGuid();
//            var dataWithId = new KeyValuePair<Guid, TInput>(id, data)
//            await Task.Run(Produce);
//        }

//        public void StopProducing() {
//            ShouldProduce = false;
//        }

//        private async Task Produce() {
//            var rnd = new Random();
//            while (ShouldProduce) {
//                await Task.Delay(rnd.Next(1000, 2000));
//                InternalBuffer.Post(rnd.Next(1, 100));
//            }
//        }

//        public IDisposable LinkTo(ITargetBlock<int> target, DataflowLinkOptions linkOptions) {
//            return InternalBuffer.LinkTo(target, linkOptions);
//        }

//        public void Complete() {
//            InternalBuffer.Complete();
//        }

//        void IDataflowBlock.Fault(Exception exception) {
//            ((ISourceBlock<int>)InternalBuffer).Fault(exception);
//        }

//        int ISourceBlock<int>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<int> target, out bool messageConsumed) {
//            return ((ISourceBlock<int>)InternalBuffer).ConsumeMessage(messageHeader, target, out messageConsumed);
//        }

//        bool ISourceBlock<int>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<int> target) {
//            return ((ISourceBlock<int>)InternalBuffer).ReserveMessage(messageHeader, target);
//        }

//        void ISourceBlock<int>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<int> target) {
//            ((ISourceBlock<int>)InternalBuffer).ReleaseReservation(messageHeader, target);
//        }
//    }

//    public class PositiveProducer : Producer {

//        protected override int Maximum { get; }
//        protected override int Minimum { get; }

//        public PositiveProducer() {
//            this.Minimum = 1;
//            this.Maximum = 100;
//        }
//    }

//    public class NegativeProducer : Producer {

//        protected override int Maximum { get; }
//        protected override int Minimum { get; }

//        public NegativeProducer() {
//            this.Minimum = -100;
//            this.Maximum = -1;
//        }
//    }
//}
