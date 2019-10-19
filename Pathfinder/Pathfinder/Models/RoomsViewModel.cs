using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Models {
    public class RoomsViewModel {
        [Required]
        public IEnumerable<RoomViewModel> Rooms { get; set; }
    }
}