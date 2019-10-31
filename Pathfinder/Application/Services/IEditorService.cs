using Pathfinder.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    /// <summary>
    ///     Service that manages user interactions with editor
    /// </summary>
    public interface IEditorService {
        /// <summary>
        ///     Joins user assosiated with the <paramref name="principal"/> to the project with <paramref name="projectName"/>.
        ///     Returns the tuple of <see langword="bool"/> that takes the value <see langword="true"/> if joining successful 
        ///     and the project joined <seealso cref="ProjectViewModel"/>
        /// </summary>
        /// <param name="projectName">Name of the project to be joined</param>
        /// <param name="principal">User assosiated with the <paramref name="principal"/></param>
        /// <returns>
        ///     The tuple of <see langword="bool"/> that takes the value <see langword="true"/> if joining successful 
        ///     and the project joined <seealso cref="ProjectViewModel"/>
        /// </returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        ValueTask<(bool, ProjectViewModel?)> Join(string projectName, ClaimsPrincipal principal);

        /// <summary>
        ///     User assosiated with the <paramref name="principal"/> leaves from the project with <paramref name="projectName"/>
        ///     Returns the tuple of <see langword="bool"/> that takes the value <see langword="true"/> if leaving successful 
        ///     and the project left <seealso cref="ProjectViewModel"/>
        /// </summary>
        /// <param name="projectName">Name of the project that is leaving</param>
        /// <param name="principal">User assosiated with the <paramref name="principal"/></param>
        /// <returns>
        ///     The tuple of <see langword="bool"/> that takes the value <see langword="true"/> if leaving successful 
        ///     and the project left <seealso cref="ProjectViewModel"/>
        /// </returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        ValueTask<(bool, ProjectViewModel?)> Leave(string projectName, ClaimsPrincipal principal);
    }
}