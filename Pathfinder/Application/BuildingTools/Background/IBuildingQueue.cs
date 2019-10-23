using System.Threading;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools.Background {
    public interface IBuildingQueue {
        ValueTask QueueAsync(BuildingTask task);

        Task<BuildingTask> DequeueAsync(CancellationToken cancellationToken);

        ValueTask<BuildingTask[]> GetQueuedTasksAsync();
    }
}