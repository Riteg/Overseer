/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: GetSetVarCommands.cs
  Description: Developer console commands for inspecting and modifying static fields and properties via reflection.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BPG.Overseer.DevConsole
{
    /// <summary>
    /// Developer console command that inspects the value of a static field or property
    /// on a specified type using reflection.
    /// </summary>
    [DevCommand("getvar", "getvar <typeName> <fieldOrProp>")]
    public sealed class GetVarCommand : IDevCommand
    {
        /// <inheritdoc/>
        public string Name => "getvar";

        /// <inheritdoc/>
        public string Help => "Usage: getvar <typeName> <fieldOrProp>";

        /// <inheritdoc/>
        public Task<string> ExecuteAsync(CommandContext ctx)
        {
            if (ctx.Args.Count < 2)
                return Task.FromResult("Usage: getvar <type> <member>");

            var t = Type.GetType(ctx.Args[0]);
            if (t == null)
                return Task.FromResult("Type not found.");

            var m = ctx.Args[1];

            // Try field first
            var f = t.GetField(m, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null)
                return Task.FromResult($"{m} = {f.GetValue(null)}");

            // Try property next
            var p = t.GetProperty(m, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null)
                return Task.FromResult($"{m} = {p.GetValue(null)}");

            return Task.FromResult("Member not found.");
        }
    }

    /// <summary>
    /// Developer console command that sets the value of a static field or property
    /// on a specified type using reflection.
    /// </summary>
    [DevCommand("setvar", "setvar <typeName> <fieldOrProp> <value>")]
    public sealed class SetVarCommand : IDevCommand
    {
        /// <inheritdoc/>
        public string Name => "setvar";

        /// <inheritdoc/>
        public string Help => "Usage: setvar <typeName> <fieldOrProp> <value>";

        /// <inheritdoc/>
        public Task<string> ExecuteAsync(CommandContext ctx)
        {
            if (ctx.Args.Count < 3)
                return Task.FromResult("Usage: setvar <type> <member> <value>");

            var t = Type.GetType(ctx.Args[0]);
            if (t == null)
                return Task.FromResult("Type not found.");

            var m = ctx.Args[1];
            var val = ctx.Args[2];

            // Try field first
            var f = t.GetField(m, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null)
            {
                object boxed = Convert.ChangeType(val, f.FieldType, System.Globalization.CultureInfo.InvariantCulture);
                f.SetValue(null, boxed);
                return Task.FromResult($"{m} set.");
            }

            // Try property next
            var p = t.GetProperty(m, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null && p.CanWrite)
            {
                object boxed = Convert.ChangeType(val, p.PropertyType, System.Globalization.CultureInfo.InvariantCulture);
                p.SetValue(null, boxed);
                return Task.FromResult($"{m} set.");
            }

            return Task.FromResult("Member not found or not settable.");
        }
    }
}
