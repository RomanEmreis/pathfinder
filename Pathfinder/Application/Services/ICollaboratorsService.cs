using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    /// <summary>
    ///     Service that manages collaborators authorization
    /// </summary>
    public interface ICollaboratorsService {
        /// <summary>
        ///     Online collaborators
        /// </summary>
        IEnumerable<IProjectCollaborator> Collaborators { get; }

        /// <summary>
        ///     Sign in the <paramref name="identityUser"/> as collaborator
        ///     and returns <see cref="IProjectCollaborator"/> instance assosiated with current <seealso cref="IdentityUser"/>
        /// </summary>
        /// <param name="identityUser">User in identity system</param>
        /// <returns><see cref="IProjectCollaborator"/> instance assosiated with current <seealso cref="IdentityUser"/></returns>
        /// <exception cref="System.ArgumentNullException"/>
        ValueTask<IProjectCollaborator?> SignIn(IdentityUser identityUser);

        /// <summary>
        ///     Sign out the <paramref name="identityUser"/>
        /// </summary>
        /// <param name="identityUser">User in identity system</param>
        /// <exception cref="System.ArgumentNullException"/>
        ValueTask SignOut(IdentityUser identityUser);

        /// <summary>
        ///     Tries to get the collaborator by <paramref name="id"/> of current <see cref="IdentityUser"/>.
        ///     Returns <see langword="true"/> if the collaborator exists and the <seealso cref="IProjectCollaborator"/>
        ///     instance as <see langword="out"/> parameter. Otherwise returns <see langword="false"/>
        /// </summary>
        /// <param name="id">identity of current <see cref="IdentityUser"/></param>
        /// <param name="collaborator">
        ///     <see cref="IProjectCollaborator"/> instance assosiated with <paramref name="id"/>
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the collaborator exists and the <seealso cref="IProjectCollaborator"/>
        ///     instance as <see langword="out"/> parameter. Otherwise returns <see langword="false"/>
        /// </returns>
        bool TryGetCollaborator(string id, out IProjectCollaborator collaborator);
    }
}