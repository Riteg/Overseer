/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: LogEntry.cs
  Description: Represents a single log entry with metadata such as level, channel, timestamp, and optional context.
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
    /// Represents a single log entry captured by the Overseer diagnostics system.
    /// Stores message details, severity, channel, timestamp, and optional context information.
    /// </summary>
    [Serializable]
    public struct LogEntry
    {
        /// <summary>
        /// The UTC timestamp of when the log entry was created.
        /// </summary>
        public DateTime TimeUtc;

        /// <summary>
        /// The severity level of this log entry.
        /// </summary>
        public LogLevel Level;

        /// <summary>
        /// The channel categorizing the source or subsystem of this log entry.
        /// </summary>
        public LogChannel Channel;

        /// <summary>
        /// The human-readable log message content.
        /// </summary>
        public string Message;

        /// <summary>
        /// The call stack trace captured at the time of logging, if available.
        /// May be <c>null</c> or empty if not provided.
        /// </summary>
        public string StackTrace;

        /// <summary>
        /// The name of the thread that generated this log entry, if provided.
        /// Useful for debugging multithreaded systems.
        /// </summary>
        public string ThreadName;

        /// <summary>
        /// Optional contextual identifier, such as a GameObject or Entity name.
        /// Provides additional reference for the log entry's origin.
        /// </summary>
        public string ContextName;
    }
}
