using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FindingCompletion.Challenge2 {
    public interface IProduce<T> : ISourceBlock<T> {

        Task StartProducing();
        void StopProducing();
    }

    public abstract class Producer : IProduce<int> {

        private BufferBlock<int> InternalBuffer { get; }
        private bool ShouldProduce { get; set; }

        protected abstract int Minimum { get; }
        protected abstract int Maximum { get; }

        public Task Completion { get { return InternalBuffer.Completion; } }       

        public Producer() {
            this.InternalBuffer = new BufferBlock<int>();
            this.ShouldProduce = false;
        }

        public async Task StartProducing() {
            ShouldProduce = true;
            await Task.Run(Produce);
        }

        public void StopProducing() {
            ShouldProduce = false;
        }

        private async Task Produce() {
            var rnd = new Random();
            while (ShouldProduce) {
                await Task.Delay(rnd.Next(1000, 2000));
                InternalBuffer.Post(rnd.Next(1, 100));
            }
        }

        public IDisposable LinkTo(ITargetBlock<int> target, DataflowLinkOptions linkOptions) {
            return InternalBuffer.LinkTo(target, linkOptions);
        }

        public void Complete() {
            InternalBuffer.Complete();
        }

        void IDataflowBlock.Fault(Exception exception) {
            ((ISourceBlock<int>)InternalBuffer).Fault(exception);
        }

        int ISourceBlock<int>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<int> target, out bool messageConsumed) {
            return ((ISourceBlock<int>)InternalBuffer).ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        bool ISourceBlock<int>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<int> target) {
            return ((ISourceBlock<int>)InternalBuffer).ReserveMessage(messageHeader, target);
        }

        void ISourceBlock<int>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<int> target) {
            ((ISourceBlock<int>)InternalBuffer).ReleaseReservation(messageHeader, target);
        }
    }

    public class PositiveProducer : Producer {

        protected override int Maximum { get; }
        protected override int Minimum { get; }

        public PositiveProducer() {
            this.Minimum = 1;
            this.Maximum = 100;
        }
    }

    public class NegativeProducer : Producer {

        protected override int Maximum { get; }
        protected override int Minimum { get; }

        public NegativeProducer() {
            this.Minimum = -100;
            this.Maximum = -1;
        }
    }
}
