using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Models {
    public class RoomViewModel {
        [Required]
        [DisplayName("Room name")]
        [RegularExpression(@"^\s*\S+\s*$")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Visitors")]
        public int VisitorsCount { get; set; }
    }
}
