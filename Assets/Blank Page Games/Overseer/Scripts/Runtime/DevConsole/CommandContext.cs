/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: CommandContext.cs
  Description: Represents a parsed developer console command, including arguments, options, and context.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using System.Collections.Generic;
using UnityEngine;

namespace BPG.Overseer.DevConsole
{
    /// <summary>
    /// Encapsulates all information related to a single console command invocation.
    /// </summary>
    /// <remarks>
    /// A <see cref="CommandContext"/> is generated after parsing user input entered into the developer console.
    /// It contains the raw command string, the command identifier, positional arguments, 
    /// named arguments (flags/options), and optional runtime context such as the <see cref="Player"/> object.
    /// </remarks>
    public sealed class CommandContext
    {
        /// <summary>
        /// The original unparsed command string entered by the user.
        /// </summary>
        public string Raw;

        /// <summary>
        /// The root command keyword (e.g., "spawn", "teleport").
        /// </summary>
        public string Command;

        /// <summary>
        /// The list of positional arguments supplied with the command.
        /// </summary>
        public List<string> Args = new List<string>();

        /// <summary>
        /// The collection of named arguments in the form <c>--key=value</c>.
        /// </summary>
        public Dictionary<string, string> Named = new Dictionary<string, string>();

        /// <summary>
        /// Optional reference to the <see cref="GameObject"/> representing the player.
        /// Can be used by commands that require player context.
        /// </summary>
        public GameObject Player;
    }
}
