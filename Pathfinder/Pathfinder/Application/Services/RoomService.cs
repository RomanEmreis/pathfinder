using Pathfinder.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    public sealed class RoomService : IRoomService {
        private ConcurrentDictionary<string, RoomViewModel> _activeRooms = new ConcurrentDictionary<string, RoomViewModel>();

        public ValueTask<RoomsViewModel> GetCurrentRooms() {
            var roomsViewModel = new RoomsViewModel { Rooms = _activeRooms.Values };
            return new ValueTask<RoomsViewModel>(roomsViewModel);
        }

        public ValueTask Add(RoomViewModel room) {
            _activeRooms.TryAdd(room.Name, room);
            return new ValueTask();
        }
    }
}