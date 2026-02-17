/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: LogChannel.cs
  Description: Defines log channels used for categorizing and filtering log entries.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using System;

namespace BPG.Overseer.Diagnostics
{
    /// <summary>
    /// Represents channels that logs can be associated with.
    /// Channels allow selective filtering of log messages
    /// based on their source or purpose within the application.
    /// </summary>
    /// <remarks>
    /// The <see cref="FlagsAttribute"/> enables combining multiple channels
    /// with bitwise operations. Use <see cref="All"/> to represent all channels.
    /// </remarks>
    [Flags]
    public enum LogChannel
    {
        /// <summary>
        /// No channel specified. Used as a default or "ignore" state.
        /// </summary>
        None = 0,

        /// <summary>
        /// User interface�related logs.
        /// </summary>
        UI = 1 << 0,

        /// <summary>
        /// Logs related to combat systems.
        /// </summary>
        Combat = 1 << 1,

        /// <summary>
        /// Artificial intelligence�related logs.
        /// </summary>
        AI = 1 << 2,

        /// <summary>
        /// Logs related to saving and loading game data.
        /// </summary>
        Save = 1 << 3,

        /// <summary>
        /// Logs associated with Unity DOTS/ECS systems.
        /// </summary>
        ECS = 1 << 4,

        /// <summary>
        /// Networking and connectivity logs.
        /// </summary>
        Network = 1 << 5,

        /// <summary>
        /// Audio system�related logs.
        /// </summary>
        Audio = 1 << 6,

        /// <summary>
        /// Logs related to procedural or world generation systems.
        /// </summary>
        WorldGen = 1 << 7,

        /// <summary>
        /// Represents all channels. Used for global logging and filtering.
        /// </summary>
        All = ~0
    }
}
