using Pathfinder.Application.Entities;
using Pathfinder.Models;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    /// <summary>
    ///     Service that manages collaborators interaction with projects
    /// </summary>
    public interface IProjectService {
        /// <summary>
        ///     Gets all created projects
        /// </summary>
        /// <returns>
        ///     <see cref="ProjectsViewModel"/> that holds created projects
        /// </returns>
        ValueTask<ProjectsViewModel> GetProjects();

        /// <summary>
        ///     Adds the new project and returns <see langword="true"/> if project successful added
        /// </summary>
        /// <param name="project">project to be added</param>
        /// <returns>
        ///     <see langword="true"/> if project successful added
        /// </returns>
        /// <exception cref="System.ArgumentNullException"/>
        ValueTask<bool> Add(ProjectViewModel project);

        /// <summary>
        ///     Joins the <paramref name="collaborator"/> to project with <paramref name="projectName"/>
        /// </summary>
        /// <param name="projectName">Name of the project to be joined</param>
        /// <param name="collaborator"><see cref="IProjectCollaborator"/> instance that will join</param>
        /// <exception cref="System.ArgumentNullException"/>
        ValueTask Join(string projectName, IProjectCollaborator collaborator);

        /// <summary>
        ///     <paramref name="collaborator"/> leaves from project with <paramref name="projectName"/>
        /// </summary>
        /// <param name="projectName">Name of the project that is leaving</param>
        /// <param name="collaborator"><see cref="IProjectCollaborator"/> instance that will left</param>
        /// <exception cref="System.ArgumentNullException"/>
        ValueTask Leave(string projectName, IProjectCollaborator collaborator);

        /// <summary>
        ///     Tries to get the project by <paramref name="projectName"/>.
        ///     Returns <see langword="true"/> if the project exists and the <seealso cref="ProjectViewModel"/>
        ///     instance as <see langword="out"/> parameter. Otherwise returns <see langword="false"/>
        /// </summary>
        /// <param name="projectName">Name of project</param>
        /// <param name="project"><see cref="ProjectViewModel"/> instance assosiated with <paramref name="projectName"/></param>
        /// <returns>
        ///     <see langword="true"/> if the project exists and the <seealso cref="ProjectViewModel"/>
        ///     instance as <see langword="out"/> parameter. Otherwise returns <see langword="false"/>
        /// </returns>
        bool TryGetProject(string projectName, out ProjectViewModel project);
    }
}