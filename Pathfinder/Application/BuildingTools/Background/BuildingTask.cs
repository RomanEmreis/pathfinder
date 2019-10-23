using System.Collections.Generic;

namespace Pathfinder.Application.BuildingTools.Background {
    public sealed class BuildingTask {
        public BuildingTask() {}

        public BuildingTask(string projectName, string sourceCode) =>
            (ProjectName, SourceCode) = (projectName, sourceCode);

        public string ProjectName { get; set; }

        public string SourceCode { get; set; }

        public IReadOnlyCollection<int> Breakpoints { get; set; }
    }
}
