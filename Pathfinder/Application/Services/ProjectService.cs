using Pathfinder.Application.Entities;
using Pathfinder.Models;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    public sealed class ProjectService : IProjectService {
        private ConcurrentDictionary<string, ProjectViewModel> _existedProjects = new ConcurrentDictionary<string, ProjectViewModel>();

        public ValueTask<ProjectsViewModel> GetProjects() {
            var roomsViewModel = new ProjectsViewModel { Projects = _existedProjects.Values };
            return new ValueTask<ProjectsViewModel>(roomsViewModel);
        }

        public ValueTask<bool> Add(ProjectViewModel project) {
            if (project is null) throw new ArgumentNullException(nameof(project));

            var added = _existedProjects.TryAdd(project.Name, project);
            return new ValueTask<bool>(added);
        }

        public ValueTask Join(string projectName, IProjectCollaborator collaborator) {
            if (collaborator is null) throw new ArgumentNullException(nameof(collaborator));

            if (_existedProjects.TryGetValue(projectName, out var project))
                collaborator.Collaborate(project);

            return new ValueTask();
        }

        public ValueTask Leave(string projectName, IProjectCollaborator collaborator) {
            if (collaborator is null) throw new ArgumentNullException(nameof(collaborator));

            if (_existedProjects.TryGetValue(projectName, out var project))
                collaborator.Leave(project);

            return new ValueTask();
        }

        public bool TryGetProject(string projectName, out ProjectViewModel project) =>
            _existedProjects.TryGetValue(projectName, out project!);
    }
}