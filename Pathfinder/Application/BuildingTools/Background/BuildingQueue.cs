using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools.Background {
    /// <summary>
    ///     Default implementation of <see cref="IBuildingQueue"/>
    /// </summary>
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

        public ValueTask<IReadOnlyCollection<BuildingTask>> GetQueuedTasksAsync() =>
            new ValueTask<IReadOnlyCollection<BuildingTask>>(_buildingTasks.ToArray());

        public void Dispose() => _signal.Dispose();
    }
}
