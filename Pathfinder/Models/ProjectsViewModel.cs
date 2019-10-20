using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Models {
    public class ProjectsViewModel {
        [Required]
        public IEnumerable<ProjectViewModel> Projects { get; set; }
    }
}