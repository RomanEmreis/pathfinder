using Pathfinder.Models;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    public interface IRoomService {
        ValueTask<RoomsViewModel> GetCurrentRooms();

        ValueTask Add(RoomViewModel room);
    }
}
