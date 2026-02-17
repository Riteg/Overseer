/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: DevCommandAttribute.cs
  Description: Attribute used to mark classes as developer console commands, 
               providing metadata such as command name and help text.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using System;

namespace BPG.Overseer.DevConsole
{
    /// <summary>
    /// Marks a class as a developer console command.
    /// </summary>
    /// <remarks>
    /// Classes decorated with <see cref="DevCommandAttribute"/> are automatically
    /// discovered and registered in the <see cref="CommandRegistry"/>.
    /// The <see cref="Name"/> property defines the keyword used to invoke the command,
    /// and the <see cref="Help"/> property provides a description for help listings.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DevCommandAttribute : Attribute
    {
        /// <summary>
        /// The keyword name that identifies the command in the console.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A human-readable description of the command, used for help output.
        /// May be empty if not provided.
        /// </summary>
        public string Help { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DevCommandAttribute"/>.
        /// </summary>
        /// <param name="name">The keyword name for invoking the command.</param>
        /// <param name="help">Optional help text describing the command.</param>
        public DevCommandAttribute(string name, string help = "")
        {
            Name = name;
            Help = help;
        }
    }
}
