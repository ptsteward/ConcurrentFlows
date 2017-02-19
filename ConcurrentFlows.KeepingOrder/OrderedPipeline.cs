namespace ConcurrentFlows.KeepingOrder {
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public class OrderedPipeline {

        private static int scale = 10;
        private static Random rnd = new Random();

        private BufferBlock<Job> buffer;
        private TransformBlock<Job, Job> step1;
        private TransformBlock<Job, JobComplete> step2;
        private IObservable<JobComplete> finalStep;
        private IDisposable finalStepSubscription;
        
        public ConcurrentQueue<JobComplete> FinalList { get; }

        private TaskCompletionSource<bool> tcs;
        public Task Completion { get; }

        public OrderedPipeline() {
            CreatePipeline();
            LinkPipeline();
            FinalList = new ConcurrentQueue<JobComplete>();
            tcs = new TaskCompletionSource<bool>();
            Completion = tcs.Task;
        }        

        public void Complete() {
            buffer.Complete();          
        }

        public Task<bool> SendAsync(Job job) {
            return buffer.SendAsync(job);
        }

        private void CreatePipeline() {
            buffer = new BufferBlock<Job>(new DataflowBlockOptions() {
                BoundedCapacity = 20
            });

            step1 = new TransformBlock<Job, Job>(x => {
                Task.Delay(rnd.Next(100, 500)).Wait();
                Console.WriteLine($"Step1,{x.Id}");
                return new Job(x.Id, x.StartValue * scale);
            }, new ExecutionDataflowBlockOptions() {
                MaxDegreeOfParallelism = 8,
                BoundedCapacity = 20,
                EnsureOrdered = true
            });

            step2 = new TransformBlock<Job, JobComplete>(x => {
                Task.Delay(rnd.Next(100, 500)).Wait();
                Console.WriteLine($"Step2,{x.Id}");
                return new JobComplete(x.Id, x.StartValue + scale);
            }, new ExecutionDataflowBlockOptions() {
                MaxDegreeOfParallelism = 8,
                BoundedCapacity = 20,
                EnsureOrdered = true
            });
        }     

        private void LinkPipeline() {
            var linkOptions = new DataflowLinkOptions() {
                PropagateCompletion = true
            };

            buffer.LinkTo(step1, linkOptions);
            step1.LinkTo(step2, linkOptions);
            finalStep = step2.AsObservable();
            finalStepSubscription = finalStep.Subscribe(
                x => {
                    Task.Delay(rnd.Next(100, 500)).Wait();
                    Console.WriteLine($"Final,{x.Id}");
                    FinalList.Enqueue(x);
                }, () => {
                    tcs.SetResult(true);
                });
        }
    }
}
