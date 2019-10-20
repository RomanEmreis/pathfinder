using Pathfinder.Application.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Models {
    public class ProjectViewModel {
        [Required]
        [DisplayName("Project")]
        [RegularExpression(@"^\s*\S+\s*$")]
        public string Name { get; set; } = null!;

        [DisplayName("Collaborators")]
        public HashSet<IProjectCollaborator> Collaborators { get; } = new HashSet<IProjectCollaborator>();
    }
}