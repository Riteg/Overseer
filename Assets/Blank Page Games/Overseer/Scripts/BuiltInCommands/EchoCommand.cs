/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: EchoCommand.cs
  Description: Developer console command that echoes input arguments back to the console.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using System.Threading.Tasks;

namespace BPG.Overseer.DevConsole
{
    /// <summary>
    /// Developer console command that echoes the provided arguments back to the console output.
    /// </summary>
    [DevCommand("echo", "Echo arguments back")]
    public sealed class EchoCommand : IDevCommand
    {
        /// <inheritdoc/>
        public string Name => "echo";

        /// <inheritdoc/>
        public string Help => "Usage: echo &lt;text&gt;";

        /// <inheritdoc/>
        public Task<string> ExecuteAsync(CommandContext ctx)
        {
            return Task.FromResult(string.Join(" ", ctx.Args));
        }
    }
}
