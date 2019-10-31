using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Implementation of <see cref="IRunner"/> for C# language
    /// </summary>
    internal class CSharpRunner : IRunner {
        public Task<BuildingResult> ExecuteAsync(byte[] compiledAssembly, string[] arguments) {
            if (compiledAssembly.Length == 0) return Task.FromResult(new BuildingResult());

            var compiledAssemblyBytes = compiledAssembly;
            var args                  = arguments;

            return Task.Run(() => Execute(compiledAssemblyBytes, args));
        }

        private BuildingResult Execute(byte[] compiledAssembly, string[] args) {
            var (success, assemblyLoadContextWeakRef) = LoadAndExecute(compiledAssembly, args);

            for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++) {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return new BuildingResult { 
                Success = success, 
                IsAssemblyAlive = assemblyLoadContextWeakRef.IsAlive 
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static (bool, WeakReference) LoadAndExecute(byte[] compiledAssembly, string[] args) {
            using var asm                 = new MemoryStream(compiledAssembly);
            var       assemblyLoadContext = new UnloadableAssemblyLoadContext();
            var       success             = false;

            try {
                var assembly = assemblyLoadContext.LoadFromStream(asm);
                var entry    = assembly.EntryPoint;

                _ = entry is { } && entry.GetParameters().Length > 0
                    ? entry.Invoke(null, new object[] { args })
                    : entry?.Invoke(null, null);

                success = true;
            } catch (Exception ex) {
                Console.Error.WriteLine(
                    "Unhandled exception {0}: {1}{2}",
                    ex.InnerException?.GetType()?.Name ?? string.Empty,
                    ex.InnerException?.Message ?? "Unknown error", 
                    $"\n{ex.InnerException?.StackTrace ?? string.Empty}" );
            } finally {
                assemblyLoadContext.Unload();
            }

            return (success, new WeakReference(assemblyLoadContext));
        }
    }
}
