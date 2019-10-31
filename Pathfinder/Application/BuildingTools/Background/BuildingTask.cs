using System.Collections.Generic;

namespace Pathfinder.Application.BuildingTools.Background {
    /// <summary>
    ///     Represent the building task description for the project
    /// </summary>
    public sealed class BuildingTask {
        public BuildingTask() {}

        public BuildingTask(string projectName, string sourceCode) =>
            (ProjectName, SourceCode) = (projectName, sourceCode);

        public string ProjectName { get; set; } = default!;

        public string SourceCode { get; set; } = default!;

        public IReadOnlyCollection<int> Breakpoints { get; set; } = default!;

        public bool IsDebugging { get; set; }

        public bool IsRunToNextBreakpoint { get; set; }

        public int CurrentLine { get; set; }
    }
}
