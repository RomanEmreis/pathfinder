using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    public class CollaboratorsService : ICollaboratorsService {
        private ConcurrentDictionary<string, IProjectCollaborator> _activeCollaborators = new ConcurrentDictionary<string, IProjectCollaborator>();

        public CollaboratorsService() {
        }

        public IEnumerable<IProjectCollaborator> Collaborators => _activeCollaborators.Values;

        public ValueTask<IProjectCollaborator?> SignIn(IdentityUser identityUser) {
            if (identityUser is null) throw new ArgumentNullException(nameof(identityUser));

            var collaborator = new UserCollaborator(identityUser);

            return _activeCollaborators.TryAdd(identityUser.Id, collaborator)
                ? new ValueTask<IProjectCollaborator?>(collaborator)
                : new ValueTask<IProjectCollaborator?>(result: default);
        }

        public ValueTask SignOut(IdentityUser identityUser) {
            if (identityUser is null) throw new ArgumentNullException(nameof(identityUser));

            _activeCollaborators.TryRemove(identityUser.Id, out var _);

            return new ValueTask();
        }

        public bool TryGetCollaborator(string id, out IProjectCollaborator collaborator) =>
            _activeCollaborators.TryGetValue(id, out collaborator!);
    }
}