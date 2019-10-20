using Pathfinder.Models;

namespace Pathfinder.Application.Entities {
    public interface IProjectCollaborator {
        void Collaborate(ProjectViewModel project);

        void Leave(ProjectViewModel project);
    }
}