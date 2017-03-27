namespace ConcurrentFlows.FindingCompletion.Challenge2 {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class LoopPropagatorExtesnions {

        public static async Task CompleteLoop(this IReportIdle block, params IReportIdle[] args) {
            var blocks = AddBlockIfNotAllreadyInArgs(block, args);
            await CompleteLoop(TimeSpan.FromMilliseconds(int.MaxValue), blocks);
        }

        public static async Task CompleteLoop(this IReportIdle block, TimeSpan timeout, params IReportIdle[] args) {
            var blocks = AddBlockIfNotAllreadyInArgs(block, args);
            await CompleteLoop(timeout, blocks);
        }

        private static async Task CompleteLoop(TimeSpan timeout, IEnumerable<IReportIdle> blocks) {
            var timeoutTask = Task.Delay(timeout);
            while (!blocks.All(arg => arg.IsIdle)) {
                if (timeoutTask.IsCompleted) {
                    throw new TimeoutException($"The loop did not complete within the specified timeout of {timeout.TotalMilliseconds}ms");
                }
                await Task.Delay(100);
            }
        }

        private static IEnumerable<IReportIdle> AddBlockIfNotAllreadyInArgs(IReportIdle block, IReportIdle[] args) {
            var blocksListed = args.ToList();
            if (!blocksListed.Contains(block)) blocksListed.Add(block);
            return blocksListed;
        }
    }
}
