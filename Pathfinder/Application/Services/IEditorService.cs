using Pathfinder.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    public interface IEditorService {
        ValueTask<(bool, ProjectViewModel?)> Join(string projectName, ClaimsPrincipal principal);

        ValueTask<(bool, ProjectViewModel?)> Leave(string projectName, ClaimsPrincipal principal);
    }
}