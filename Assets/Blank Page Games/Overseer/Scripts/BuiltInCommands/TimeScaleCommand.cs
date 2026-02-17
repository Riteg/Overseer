/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: TimeScaleCommand.cs
  Description: Developer console command that gets or sets Unity's Time.timeScale.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

namespace BPG.Overseer.DevConsole
{
    /// <summary>
    /// Developer console command that gets or sets <see cref="Time.timeScale"/>.
    /// </summary>
    [DevCommand("timescale", "Set or get Time.timeScale")]
    public sealed class TimeScaleCommand : IDevCommand
    {
        /// <inheritdoc/>
        public string Name => "timescale";

        /// <inheritdoc/>
        public string Help => "Usage: timescale [value]";

        /// <inheritdoc/>
        public Task<string> ExecuteAsync(CommandContext ctx)
        {
            // Query mode
            if (ctx.Args.Count == 0)
                return Task.FromResult($"timeScale = {Time.timeScale:F2}");

            // Parse argument as float
            if (float.TryParse(ctx.Args[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                Time.timeScale = Mathf.Clamp(v, 0f, 20f);
                return Task.FromResult($"timeScale set to {Time.timeScale:F2}");
            }

            return Task.FromResult("Invalid value. Usage: timescale [0.0 - 20.0]");
        }
    }
}
