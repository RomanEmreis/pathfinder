using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    public interface ICollaboratorsService {
        IEnumerable<IProjectCollaborator> Collaborators { get; }

        ValueTask<IProjectCollaborator?> SignIn(IdentityUser identityUser);

        ValueTask SignOut(IdentityUser identityUser);

        bool TryGetCollaborator(string id, out IProjectCollaborator collaborator);
    }
}