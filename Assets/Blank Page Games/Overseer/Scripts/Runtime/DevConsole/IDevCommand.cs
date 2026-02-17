/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: IDevCommand.cs
  Description: Defines the interface for developer console commands, 
               including metadata and asynchronous execution.
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
    /// Defines a contract for all developer console commands.
    /// </summary>
    /// <remarks>
    /// A command must provide a <see cref="Name"/> for invocation,
    /// a <see cref="Help"/> string for user guidance, and implement
    /// <see cref="ExecuteAsync"/> to perform its logic.
    /// </remarks>
    public interface IDevCommand
    {
        /// <summary>
        /// Gets the keyword name used to invoke this command in the console.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the help text that describes the purpose and usage of this command.
        /// </summary>
        string Help { get; }

        /// <summary>
        /// Executes the command asynchronously using the provided context.
        /// </summary>
        /// <param name="ctx">The parsed command context containing input, arguments, and options.</param>
        /// <returns>
        /// A task that resolves to a result string, typically displayed back to the console.
        /// </returns>
        Task<string> ExecuteAsync(CommandContext ctx);
    }
}
