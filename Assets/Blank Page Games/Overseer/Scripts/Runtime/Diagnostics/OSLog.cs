/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: OSLog.cs
  Description: Provides a static logging API for recording log entries and mirroring them to Unity's console.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using UnityEngine;

namespace BPG.Overseer.Diagnostics
{
    using UDebug = UnityEngine.Debug;

    /// <summary>
    /// Provides a static logging interface that records entries into the <see cref="LogStore"/> 
    /// while also forwarding them to Unity's built-in console.
    /// </summary>
    /// <remarks>
    /// This class acts as the primary entry point for logging within the Overseer framework.
    /// Each call records a <see cref="LogEntry"/> with metadata (timestamp, channel, level, context, etc.)
    /// and mirrors the message to Unitys console for real-time debugging.
    /// </remarks>
    public static class OSLog
    {
        /// <summary>
        /// Writes a log entry to the <see cref="LogStore"/> and Unity console.
        /// </summary>
        /// <param name="level">Severity level of the log.</param>
        /// <param name="channel">Logical channel categorizing the log.</param>
        /// <param name="msg">Message text to record.</param>
        /// <param name="ctx">Optional Unity context object (e.g., <see cref="GameObject"/>). 
        /// This appears as a clickable reference in the Unity console.</param>
        /// <param name="stack">Optional stack trace string, if available.</param>
        public static void Write(LogLevel level, LogChannel channel, string msg, Object ctx = null, string stack = null)
        {
            // Record into Overseer's log store.
            LogStore.Instance.Add(new LogEntry
            {
                TimeUtc = System.DateTime.UtcNow,
                Level = level,
                Channel = channel,
                Message = msg,
                StackTrace = stack,
                ThreadName = System.Threading.Thread.CurrentThread.Name ?? "Thread",
                ContextName = ctx ? ctx.name : string.Empty
            });

            // Forward to Unity console based on severity.
            switch (level)
            {
                case LogLevel.Error:
                    UDebug.LogError(msg, ctx);
                    break;
                case LogLevel.Warning:
                    UDebug.LogWarning(msg, ctx);
                    break;
                default:
                    UDebug.Log(msg, ctx);
                    break;
            }
        }

        /// <summary>
        /// Records a verbose <see cref="LogLevel.Trace"/> message.
        /// </summary>
        /// <param name="ch">Channel of the log.</param>
        /// <param name="m">Message content.</param>
        /// <param name="c">Optional Unity context object.</param>
        public static void Trace(LogChannel ch, string m, Object c = null) => Write(LogLevel.Trace, ch, m, c);

        /// <summary>
        /// Records a <see cref="LogLevel.Debug"/> message for development diagnostics.
        /// </summary>
        /// <param name="ch">Channel of the log.</param>
        /// <param name="m">Message content.</param>
        /// <param name="c">Optional Unity context object.</param>
        public static void Debug(LogChannel ch, string m, Object c = null) => Write(LogLevel.Debug, ch, m, c);

        /// <summary>
        /// Records a general <see cref="LogLevel.Info"/> message.
        /// </summary>
        /// <param name="ch">Channel of the log.</param>
        /// <param name="m">Message content.</param>
        /// <param name="c">Optional Unity context object.</param>
        public static void Info(LogChannel ch, string m, Object c = null) => Write(LogLevel.Info, ch, m, c);

        /// <summary>
        /// Records a <see cref="LogLevel.Warning"/> message to indicate potential issues.
        /// </summary>
        /// <param name="ch">Channel of the log.</param>
        /// <param name="m">Message content.</param>
        /// <param name="c">Optional Unity context object.</param>
        public static void Warn(LogChannel ch, string m, Object c = null) => Write(LogLevel.Warning, ch, m, c);

        /// <summary>
        /// Records a <see cref="LogLevel.Error"/> message for critical issues.
        /// </summary>
        /// <param name="ch">Channel of the log.</param>
        /// <param name="m">Message content.</param>
        /// <param name="c">Optional Unity context object.</param>
        public static void Error(LogChannel ch, string m, Object c = null) => Write(LogLevel.Error, ch, m, c);
    }
}
