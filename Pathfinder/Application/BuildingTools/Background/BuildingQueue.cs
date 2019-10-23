using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools.Background {
    internal sealed class BuildingQueue : IBuildingQueue, IDisposable {
        private ConcurrentQueue<BuildingTask> _buildingTasks = new ConcurrentQueue<BuildingTask>();
        private SemaphoreSlim                 _signal        = new SemaphoreSlim(0,10);

        public ValueTask QueueAsync(BuildingTask buildingTask) {
            if (buildingTask is null) throw new ArgumentNullException(nameof(buildingTask));

            _buildingTasks.Enqueue(buildingTask);
            _signal.Release();

            return new ValueTask();
        }

        public async Task<BuildingTask> DequeueAsync(CancellationToken cancellationToken) {
            await _signal.WaitAsync(cancellationToken);

            if (_buildingTasks.TryDequeue(out var uploadingFile))
                return uploadingFile;

            throw new InvalidOperationException();
        }

        public ValueTask<BuildingTask[]> GetQueuedTasksAsync() =>
            new ValueTask<BuildingTask[]>(_buildingTasks.ToArray());

        public void Dispose() => _signal.Dispose();
    }
}
