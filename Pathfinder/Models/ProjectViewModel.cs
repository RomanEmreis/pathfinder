using Pathfinder.Application.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Pathfinder.Models {
    public class ProjectViewModel {
        [Required]
        [DisplayName("Project")]
        [RegularExpression(@"^\s*\S+\s*$")]
        public string Name { get; set; } = null!;

        [Required]
        public string Code { get; set; } = null!;


        [DisplayName("Collaborators")]
        public HashSet<IProjectCollaborator> Collaborators { get; } = new HashSet<IProjectCollaborator>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void GenerateTemplateCode() {
            if (!string.IsNullOrWhiteSpace(Code)) return;

            Code = "using System;\n\n"
            + $"namespace {Name} {{\n"
            + "    class Program {\n"
            + "        static void Main(string[] args) {\n"
            + "            Console.WriteLine(\"Hello World!\");\n"
            + "        }\n"
            + "    }\n"
            + "}\n";
        }
    }
}