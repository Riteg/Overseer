using System.Threading.Tasks;
using BPG.Overseer.DevConsole;

namespace Samples.Overseer
{
    /// <summary>
    /// Example developer console command included as a sample for Overseer.
    /// Prints a friendly message to the console when executed.
    /// </summary>
    [DevCommand("hello", "Prints a friendly hello message")]
    public sealed class ExampleCommand : IDevCommand
    {
        /// <inheritdoc/>
        public string Name => "hello";

        /// <inheritdoc/>
        public string Help => "Usage: hello";

        /// <inheritdoc/>
        public Task<string> ExecuteAsync(CommandContext ctx)
        {
            return Task.FromResult("Hello from Overseer! Your custom command is working.");
        }
    }
}
