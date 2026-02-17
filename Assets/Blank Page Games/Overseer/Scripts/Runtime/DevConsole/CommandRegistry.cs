/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: CommandRegistry.cs
  Description: Maintains a registry of developer console commands. 
               Supports auto-registration via reflection and async execution.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BPG.Overseer.DevConsole
{
    /// <summary>
    /// Central registry for all available developer console commands.
    /// </summary>
    /// <remarks>
    /// Commands implementing <see cref="IDevCommand"/> and marked with <see cref="DevCommandAttribute"/>
    /// are automatically discovered via reflection when the registry is initialized.
    /// <para>
    /// Consumers can query available command names, retrieve command instances,
    /// and execute commands asynchronously using a <see cref="CommandContext"/>.
    /// </para>
    /// </remarks>
    public sealed class CommandRegistry
    {
        /// <summary>
        /// Singleton instance of the <see cref="CommandRegistry"/>.
        /// </summary>
        public static readonly CommandRegistry Instance = new CommandRegistry();

        private readonly Dictionary<string, IDevCommand> _map = new();

        /// <summary>
        /// Initializes the registry by scanning all currently loaded assemblies
        /// for types implementing <see cref="IDevCommand"/> with a <see cref="DevCommandAttribute"/>.
        /// </summary>
        private CommandRegistry()
        {
            AutoRegisterFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Scans the given assemblies for developer command implementations and registers them.
        /// </summary>
        /// <param name="asms">The assemblies to scan.</param>
        /// <remarks>
        /// Any class that:
        /// <list type="bullet">
        ///   <item>Implements <see cref="IDevCommand"/></item>
        ///   <item>Is not abstract</item>
        ///   <item>Is decorated with <see cref="DevCommandAttribute"/></item>
        /// </list>
        /// will be instantiated and added to the registry.
        /// </remarks>
        public void AutoRegisterFromAssemblies(IEnumerable<Assembly> asms)
        {
            foreach (var asm in asms)
            {
                foreach (var t in asm.GetTypes())
                {
                    if (!typeof(IDevCommand).IsAssignableFrom(t) || t.IsAbstract)
                        continue;

                    var attr = t.GetCustomAttribute<DevCommandAttribute>();
                    if (attr == null)
                        continue;

                    var inst = (IDevCommand)Activator.CreateInstance(t);
                    _map[attr.Name.ToLowerInvariant()] = inst;
                }
            }
        }

        /// <summary>
        /// Gets the list of all registered command names in alphabetical order.
        /// </summary>
        public IEnumerable<string> Names => _map.Keys.OrderBy(k => k);

        /// <summary>
        /// Attempts to retrieve a registered command by name.
        /// </summary>
        /// <param name="name">The command name to search for.</param>
        /// <param name="cmd">The resulting command instance, if found.</param>
        /// <returns><c>true</c> if the command was found; otherwise, <c>false</c>.</returns>
        public bool TryGet(string name, out IDevCommand cmd) =>
            _map.TryGetValue(name.ToLowerInvariant(), out cmd);

        /// <summary>
        /// Executes a developer console command asynchronously.
        /// </summary>
        /// <param name="ctx">The command context containing input, arguments, and optional player reference.</param>
        /// <returns>
        /// The command's result message, or a default error string if the command is not found.
        /// </returns>
        public async Task<string> ExecuteAsync(CommandContext ctx)
        {
            if (!TryGet(ctx.Command, out var cmd))
                return $"Unknown command: {ctx.Command}. Try 'help'.";

            return await cmd.ExecuteAsync(ctx);
        }
    }
}
