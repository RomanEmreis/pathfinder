using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Pathfinder.Application.BuildingTools.Background;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    internal class CSharpCompiler : ICompiler {
        private const string _missingParametersMessage = "Missing source code or assembly name";

        public Task<byte[]> CompileAsync(BuildingTask buildingTask) {
            if (buildingTask is null) throw new ArgumentNullException(nameof(buildingTask));

            var assemblyName = buildingTask.ProjectName;
            var sourceCode   = buildingTask.SourceCode;

            return string.IsNullOrWhiteSpace(assemblyName) || string.IsNullOrWhiteSpace(sourceCode)
                ? throw new InvalidOperationException(_missingParametersMessage)
                : Task.Run(() => Compile(assemblyName, sourceCode));
        }

        internal byte[] Compile(string assemblyName, string sourceCode) {
            using var peStream = new MemoryStream();
            var       result   = GenerateCode(assemblyName, sourceCode).Emit(peStream);

            if (!result.Success) {
                var failures = result.Diagnostics
                    .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures) {
                    Console.Error.WriteLine(diagnostic.ToString());
                }
                return Array.Empty<byte>();
            }

            peStream.Seek(0, SeekOrigin.Begin);

            return peStream.ToArray();
        }

        private static CSharpCompilation GenerateCode(string assemblyName, string sourceCode) {
            var codeString       = SourceText.From(sourceCode);
            var options          = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new MetadataReference[] {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
            };

            return CSharpCompilation.Create($"{assemblyName}.dll",
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                    optimizationLevel: OptimizationLevel.Debug,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}
