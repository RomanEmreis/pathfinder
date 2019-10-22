using System.Reflection;
using System.Runtime.Loader;

namespace Pathfinder.Application.BuildingTools {
    internal class UnloadableAssemblyLoadContext : AssemblyLoadContext {
        public UnloadableAssemblyLoadContext() : base(true) {}

        protected override Assembly? Load(AssemblyName assemblyName) => null;
    }
}