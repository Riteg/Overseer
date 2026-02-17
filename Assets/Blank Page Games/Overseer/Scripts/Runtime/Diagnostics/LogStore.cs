/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: LogStore.cs
  Description: Thread-safe ring buffer for storing and managing log entries. 
               Provides snapshotting and events for new entries and clears.
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
using System.Threading;

namespace BPG.Overseer.Diagnostics
{
    /// <summary>
    /// Provides a thread-safe in-memory ring buffer for storing <see cref="LogEntry"/> instances.
    /// Supports capacity limiting, snapshotting, and event notifications for observers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The store enforces a maximum capacity. When the capacity is reached, the oldest entries 
    /// are automatically discarded to make room for new ones. 
    /// </para>
    /// <para>
    /// Thread-safety is ensured via <see cref="ReaderWriterLockSlim"/>, allowing multiple readers 
    /// to safely take snapshots while writers enqueue new entries.
    /// </para>
    /// </remarks>
    public sealed class LogStore
    {
        /// <summary>
        /// Default maximum number of log entries to store in memory.
        /// </summary>
        public const int DefaultCapacity = 2000;

        private readonly int _capacity;
        private readonly Queue<LogEntry> _ring;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Raised whenever a new <see cref="LogEntry"/> is added to the store.
        /// </summary>
        public event Action<LogEntry> OnAdded;

        /// <summary>
        /// Raised whenever the store is cleared.
        /// </summary>
        public event Action OnCleared;

        /// <summary>
        /// Singleton instance of the <see cref="LogStore"/>.
        /// </summary>
        public static readonly LogStore Instance = new LogStore(DefaultCapacity);

        /// <summary>
        /// Initializes a new instance of the <see cref="LogStore"/> with the specified capacity.
        /// </summary>
        /// <param name="capacity">The maximum number of log entries to keep in memory. 
        /// Minimum enforced value is 128.</param>
        private LogStore(int capacity)
        {
            _capacity = Math.Max(128, capacity);
            _ring = new Queue<LogEntry>(_capacity);
        }

        /// <summary>
        /// Adds a new log entry to the store.
        /// </summary>
        /// <param name="entry">The log entry to add.</param>
        /// <remarks>
        /// If the capacity has been reached, the oldest entry is discarded.
        /// This method is thread-safe.
        /// </remarks>
        public void Add(LogEntry entry)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_ring.Count >= _capacity)
                {
                    _ring.Dequeue();
                }

                _ring.Enqueue(entry);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnAdded?.Invoke(entry);
        }

        /// <summary>
        /// Removes all log entries from the store.
        /// </summary>
        /// <remarks>
        /// This method is thread-safe and raises the <see cref="OnCleared"/> event.
        /// </remarks>
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _ring.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnCleared?.Invoke();
        }

        /// <summary>
        /// Copies the current log entries into the provided buffer list.
        /// </summary>
        /// <param name="buffer">The target list to populate with log entries. 
        /// Existing contents will be cleared.</param>
        /// <remarks>
        /// Snapshotting is thread-safe and provides a consistent view of the log entries
        /// at the time of acquisition. Enumeration order matches insertion order.
        /// </remarks>
        public void Snapshot(List<LogEntry> buffer)
        {
            buffer.Clear();

            _lock.EnterReadLock();
            try
            {
                buffer.AddRange(_ring);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
