using Microsoft.CodeAnalysis.Scripting;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public interface IDebugger {
        Task DebugAsync(CSharpDebuggingContext context);
    }
}
