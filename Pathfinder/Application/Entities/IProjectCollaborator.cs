using Pathfinder.Models;

namespace Pathfinder.Application.Entities {
    /// <summary>
    ///     Represents an abstract collaborator
    /// </summary>
    public interface IProjectCollaborator {
        /// <summary>
        ///    Begins collaboration on the <paramref name="project"/>    
        /// </summary>
        /// <param name="project">Project when begins collaboration</param>
        /// <exception cref="System.ArgumentNullException"/>
        void Collaborate(ProjectViewModel project);

        /// <summary>
        ///     Stops collaborating on the <paramref name="project"/>
        /// </summary>
        /// <param name="project">Project when stops collaboration</param>
        /// <exception cref="System.ArgumentNullException"/>
        void Leave(ProjectViewModel project);
    }
}