/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: HelpCommand.cs
  Description: Developer console command that lists all commands or shows help for a specific command.
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
    /// Developer console command that lists all available commands or shows help for a specific command.
    /// </summary>
    [DevCommand("help", "List commands or show help for a command")]
    public sealed class HelpCommand : IDevCommand
    {
        /// <inheritdoc/>
        public string Name => "help";

        /// <inheritdoc/>
        public string Help => "Usage: help [commandName]";

        /// <inheritdoc/>
        public Task<string> ExecuteAsync(CommandContext ctx)
        {
            if (ctx.Args.Count == 0)
            {
                var list = string.Join(", ", CommandRegistry.Instance.Names);
                return Task.FromResult($"Commands: {list}");
            }

            var name = ctx.Args[0].ToLowerInvariant();
            if (CommandRegistry.Instance.TryGet(name, out var cmd))
                return Task.FromResult($"{cmd.Name}: {cmd.Help}");

            return Task.FromResult($"No help available for '{name}'.");
        }
    }
}
