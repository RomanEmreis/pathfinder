using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Entities;
using Pathfinder.Models;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    public interface IProjectService {
        ValueTask<ProjectsViewModel> GetProjects();

        ValueTask<bool> Add(ProjectViewModel project);

        ValueTask Join(string projectName, IProjectCollaborator collaborator);

        ValueTask Leave(string projectName, IProjectCollaborator collaborator);

        bool TryGetProject(string projectName, out ProjectViewModel project);
    }
}
