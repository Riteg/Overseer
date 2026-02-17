/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer —
   Blank Page Games
  File: NSLogBootstrap.cs
  Description: Runtime bootstrap that installs the Unity log bridge and in-game log/console overlay.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using BPG.Overseer.Diagnostics;
using UnityEngine;

namespace BPG.Overseer.UI
{
    /// <summary>
    /// Installs Overseer diagnostics at runtime before the first scene loads.
    /// Creates a persistent root GameObject and attaches:
    /// <list type="bullet">
    /// <item><see cref="UnityLogBridge"/> � mirrors Unity logs into <see cref="LogStore"/>.</item>
    /// <item><see cref="LogOverlayUI"/> � in-game IMGUI overlay for logs and console.</item>
    /// </list>
    /// </summary>
    public static class OSLogBootstrap
    {
        /// <summary>
        /// Entry point invoked by Unity during startup (before any scene is loaded).
        /// Creates a <c>[DevTools]</c> root object marked as <see cref="Object.DontDestroyOnLoad(object)"/>.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var root = new GameObject("[DevTools]");
            Object.DontDestroyOnLoad(root);

            root.AddComponent<UnityLogBridge>();
            root.AddComponent<LogOverlayUI>();
        }
    }
}
