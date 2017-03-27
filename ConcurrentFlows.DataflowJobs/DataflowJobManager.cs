namespace ConcurrentFlows.DataflowJobs {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public interface IJobManager<TInput, TOutput> {
        Task<TOutput> SubmitRequest(TInput data);
    }

    public class DataflowJobManager<TInput, TOutput> : IJobManager<TInput, TOutput> {

        public DataflowJobManager(IPropagatorBlock<KeyValuePair<Guid, TInput>, KeyValuePair<Guid, TOutput>> jobHandler) {
            if (jobHandler == null) { throw new ArgumentException("Argument cannot be null.", "jobHandler"); }
                        
            this.JobHandler = JobHandler;
            if (!AlreadyLinked) {
                AlreadyLinked = true;
                JobHandler.LinkTo(ResultHandler, new DataflowLinkOptions() { PropagateCompletion = true });
            }            
        }

        public async Task<TOutput> SubmitRequest(TInput data) {
            var taggedData = TagInputData(data);
            var job = CreateJob(taggedData);
            Jobs.TryAdd(job.Key, job.Value);
            await JobHandler.SendAsync(taggedData);
            return await job.Value.Task;
        }

        private static bool AlreadyLinked = false;

        private static ConcurrentDictionary<Guid, TaskCompletionSource<TOutput>> Jobs {
            get;
        } = new ConcurrentDictionary<Guid, TaskCompletionSource<TOutput>>();

        private static ExecutionDataflowBlockOptions Options {
            get;
        } = GetResultHandlerOptions();

        private static ITargetBlock<KeyValuePair<Guid, TOutput>> ResultHandler {
            get;
        } = CreateReplyHandler(Options);

        private IPropagatorBlock<KeyValuePair<Guid, TInput>, KeyValuePair<Guid, TOutput>> JobHandler {
            get;
        }

        private KeyValuePair<Guid, TInput> TagInputData(TInput data) {
            var id = Guid.NewGuid();
            return new KeyValuePair<Guid, TInput>(id, data);
        }

        private KeyValuePair<Guid, TaskCompletionSource<TOutput>> CreateJob(KeyValuePair<Guid, TInput> taggedData) {
            var id = taggedData.Key;
            var jobCompletionSource = new TaskCompletionSource<TOutput>();
            return new KeyValuePair<Guid, TaskCompletionSource<TOutput>>(id, jobCompletionSource);
        }

        private static ExecutionDataflowBlockOptions GetResultHandlerOptions() {
            return new ExecutionDataflowBlockOptions() {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = 1000
            };
        }

        private static ITargetBlock<KeyValuePair<Guid, TOutput>> CreateReplyHandler(ExecutionDataflowBlockOptions options) {
            return new ActionBlock<KeyValuePair<Guid, TOutput>>((result) => {
                RecieveOutput(result);
            }, options);
        }

        private static void RecieveOutput(KeyValuePair<Guid, TOutput> result) {
            var jobId = result.Key;
            TaskCompletionSource<TOutput> jobCompletionSource;
            if (!Jobs.TryRemove(jobId, out jobCompletionSource)) {
                throw new InvalidOperationException($"The jobId: {jobId} was not found.");
            }
            var resultValue = result.Value;
            jobCompletionSource.SetResult(resultValue);            
        }
    }
}
