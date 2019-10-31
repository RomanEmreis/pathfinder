using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools.Background {
    /// <summary>
    ///     Represents an queue of building tasks
    /// </summary>
    public interface IBuildingQueue {
        /// <summary>
        ///     Add the building task to queue
        /// </summary>
        /// <param name="task">Queuing building task</param>
        ValueTask QueueAsync(BuildingTask task);

        /// <summary>
        ///     Dequeue first building task
        /// </summary>
        Task<BuildingTask> DequeueAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Gets all queued tasks
        /// </summary>
        /// <returns>collection of all queued tasks</returns>
        ValueTask<IReadOnlyCollection<BuildingTask>> GetQueuedTasksAsync();
    }
}