namespace Pathfinder.Application.BuildingTools {
    public sealed class BuildingResult {
        public bool Success { get; set; }

        public bool IsAssemblyAlive { get; set; }

        public string ResultMessage { get; set; } = null!;

        public int CurrentLine { get; set; }
    }
}