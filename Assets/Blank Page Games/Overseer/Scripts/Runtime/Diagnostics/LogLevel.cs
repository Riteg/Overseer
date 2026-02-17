/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: LogLevel.cs
  Description: Defines severity levels for log entries, ranging from verbose tracing to critical errors.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

namespace BPG.Overseer.Diagnostics
{
    /// <summary>
    /// Represents the severity level of a log entry.
    /// Levels provide filtering and control over which logs are recorded or displayed.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Highly detailed information, typically only useful for step-by-step tracing
        /// of internal operations during development or debugging.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Diagnostic information intended to help developers track application flow
        /// and state changes, less verbose than <see cref="Trace"/>.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// General informational messages that describe the normal runtime state
        /// of the application. Useful for monitoring system health.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Indicates a potential issue or unexpected state that does not stop execution.
        /// Often used for recoverable problems or deprecation notices.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Represents a serious issue that may prevent normal program execution
        /// or result in data loss. Requires immediate attention.
        /// </summary>
        Error = 4
    }
}
