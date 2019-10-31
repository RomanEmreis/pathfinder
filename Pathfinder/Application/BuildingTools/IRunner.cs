using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Represents an abstract code runner
    /// </summary>
    public interface IRunner {
        /// <summary>
        ///     Executes <paramref name="compiledAssembly"/> with the <paramref name="arguments"/> and
        ///     returns the result as instance of <see cref="BuildingResult"/>
        /// </summary>
        /// <param name="compiledAssembly">bytes array of compiled assembly</param>
        /// <param name="arguments">startup arguments</param>
        /// <returns>instance of <see cref="BuildingResult"/></returns>
        Task<BuildingResult> ExecuteAsync(byte[] compiledAssembly, string[] arguments);
    }
}