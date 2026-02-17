/* 
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|

  Overseer — Blank Page Games
  File: LogOverlayUI.cs
  Description: In-game IMGUI overlay that provides a Logs viewer and Dev Console with command suggestions.
  Unity: 2022.3+ (also compatible with Unity 6 / 6.2)
  .NET: .NET Standard 2.1
  License:
    Copyright (c) 2025 Blank Page Games
    Permission is granted to use, modify, and distribute this software in personal or commercial projects.
    However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.

  This file is part of the 'Overseer' asset by Blank Page Games.
*/

using BPG.Overseer.DevConsole;
using BPG.Overseer.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BPG.Overseer.UI
{
    /// <summary>
    /// IMGUI overlay window that exposes two tabs:
    /// <list type="bullet">
    /// <item><b>Logs</b> — filterable live log feed with per-entry details and clipboard export.</item>
    /// <item><b>Console</b> — developer console with command history and auto-suggest for command names.</item>
    /// </list>
    /// Toggle visibility with <see cref="toggleKey"/>.
    /// </summary>
    [DefaultExecutionOrder(10000)]
    public sealed class LogOverlayUI : MonoBehaviour
    {
        /// <summary>
        /// Keyboard shortcut to toggle the overlay on/off. Default: <see cref="KeyCode.F1"/>.
        /// </summary>
        public KeyCode toggleKey = KeyCode.F1;

        /// <summary>
        /// Whether the overlay is currently visible.
        /// </summary>
        public bool visible = true;

        // --- Window / UI state -------------------------------------------------

        private Rect _windowRect = new Rect(Screen.width - 520, 20, 500, 420);
        private Vector2 _logScroll, _consoleScroll;
        private string _search = string.Empty;
        private bool _autoScroll = true;
        private bool _pauseOnError = true;

        private LogLevel _minLevel = LogLevel.Debug;
        private LogChannel _channels = LogChannel.All;

        // Cached snapshot of logs taken per-frame for drawing.
        private readonly List<LogEntry> _snapshot = new();

        // Expanded rows (by computed id).
        private readonly HashSet<int> _expanded = new();

        // Per-row scroll positions for stack traces.
        private readonly Dictionary<int, Vector2> _detailScroll = new();

        private const float DETAIL_MAX_HEIGHT = 120f;

        // --- Console: suggestion popup state -----------------------------------

        private List<string> _suggestions = new();
        private int _suggestIndex = -1;
        private bool _showSuggest = false;
        private Rect _inputRect;
        private string _prevInput = string.Empty;
        private bool _forceRefocusNextRepaint = false;

        // Suggest overlay draw request
        private Rect _suggestPopupRectScreen;
        private bool _drawSuggestOverlay = false;
        private const float TITLEBAR_H = 24f; // Title bar height used by DragWindow

        // --- Console I/O -------------------------------------------------------

        private string _input = string.Empty;
        private string _consoleOutput = string.Empty;
        private readonly List<string> _history = new();
        private int _historyIndex = -1;

        private int _tab = 0;

        private void OnEnable()
        {
            LogStore.Instance.OnAdded += OnLogAdded;
        }

        private void OnDisable()
        {
            LogStore.Instance.OnAdded -= OnLogAdded;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                visible = !visible;
        }

        private void OnGUI()
        {
            if (!visible) return;

            // Main window (contents are clipped to the window rect)
            _windowRect = GUILayout.Window(GetInstanceID(), _windowRect, DrawWindow, "🛠 Dev Tools", GUI.skin.window);

            // Suggestion overlay draws after the window, outside its clipping region.
            if (_drawSuggestOverlay)
            {
                DrawSuggestOverlay();
                _drawSuggestOverlay = false; // reset each frame
            }
        }

        /// <summary>
        /// Draws the window chrome and selected tab content.
        /// </summary>
        private void DrawWindow(int id)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_tab == 0, "Logs", GUI.skin.button)) _tab = 0;
            if (GUILayout.Toggle(_tab == 1, "Console", GUI.skin.button)) _tab = 1;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("×", GUILayout.Width(28))) visible = false;
            GUILayout.EndHorizontal();

            switch (_tab)
            {
                case 0: DrawLogs(); break;
                case 1: DrawConsole(); break;
            }

            GUI.DragWindow(new Rect(0, 0, _windowRect.width, TITLEBAR_H));
        }

        /// <summary>
        /// Renders the Logs tab: filters, channels, and entry list with expandable details.
        /// </summary>
        private void DrawLogs()
        {
            // Level filter row
            GUILayout.BeginHorizontal();
            GUILayout.Label("Min:", GUILayout.Width(36));
            _minLevel = (LogLevel)GUILayout.SelectionGrid((int)_minLevel, new[] { "Trace", "Debug", "Info", "Warn", "Error" }, 5);
            GUILayout.EndHorizontal();

            // Search / controls row
            GUILayout.BeginHorizontal();
            _search = GUILayout.TextField(_search, GUILayout.MinWidth(100));
            _autoScroll = GUILayout.Toggle(_autoScroll, "Autoscroll");
            _pauseOnError = GUILayout.Toggle(_pauseOnError, "Pause on Error");
            if (GUILayout.Button("Clear", GUILayout.Width(70))) LogStore.Instance.Clear();
            if (GUILayout.Button("Copy", GUILayout.Width(70))) GUIUtility.systemCopyBuffer = BuildClipboard();
            GUILayout.EndHorizontal();

            // Channels row
            GUILayout.BeginHorizontal();
            DrawChannelToggle(LogChannel.UI);
            DrawChannelToggle(LogChannel.Combat);
            DrawChannelToggle(LogChannel.AI);
            DrawChannelToggle(LogChannel.Save);
            DrawChannelToggle(LogChannel.ECS);
            DrawChannelToggle(LogChannel.Network);
            DrawChannelToggle(LogChannel.Audio);
            DrawChannelToggle(LogChannel.WorldGen);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Snapshot & list
            LogStore.Instance.Snapshot(_snapshot);
            _logScroll = GUILayout.BeginScrollView(_logScroll);

            for (int i = 0; i < _snapshot.Count; i++)
            {
                var e = _snapshot[i];
                if (e.Level < _minLevel) continue;
                if ((_channels & e.Channel) == 0) continue;
                if (!string.IsNullOrEmpty(_search) &&
                    (e.Message == null || !e.Message.ToLower().Contains(_search.ToLower()))) continue;

                var id = ComputeRowId(e);

                // Row tint by severity
                var prevColor = GUI.color;
                GUI.color = e.Level switch
                {
                    LogLevel.Error => Color.red,
                    LogLevel.Warning => new Color(1f, 0.7f, 0.2f),
                    LogLevel.Debug => new Color(0.7f, 0.9f, 1f),
                    LogLevel.Trace => new Color(0.8f, 0.8f, 0.8f),
                    _ => Color.white
                };

                // Summary row
                GUILayout.BeginHorizontal();
                var expanded = _expanded.Contains(id);
                var caret = expanded ? "▼" : "▶";
                if (GUILayout.Button(caret, GUILayout.Width(22)))
                {
                    if (expanded) _expanded.Remove(id);
                    else _expanded.Add(id);
                }

                var line = $"[{e.TimeUtc:HH:mm:ss}] [{e.Level}] [{e.Channel}] {e.Message}";
                GUILayout.Label(line, GUI.skin.label, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("⋯", GUILayout.Width(28)))
                    GUIUtility.systemCopyBuffer = line;

                GUILayout.EndHorizontal();

                // Detail area
                if (expanded)
                {
                    // Optional context name
                    if (!string.IsNullOrEmpty(e.ContextName))
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.85f);
                        GUILayout.Label($"Context: {e.ContextName}", GUI.skin.label);
                    }

                    // Optional stack trace with its own scroll
                    if (!string.IsNullOrEmpty(e.StackTrace))
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.75f);

                        if (!_detailScroll.TryGetValue(id, out var s))
                            s = Vector2.zero;

                        s = GUILayout.BeginScrollView(s, GUILayout.Height(DETAIL_MAX_HEIGHT));
                        GUILayout.TextArea(e.StackTrace, GUILayout.ExpandHeight(true));
                        GUILayout.EndScrollView();

                        _detailScroll[id] = s;
                    }
                }

                GUI.color = prevColor;
                GUILayout.Space(4); // small spacing between rows
            }

            GUILayout.EndScrollView();

            if (_autoScroll) _logScroll.y = 999999f;
        }

        /// <summary>
        /// Stable id for a log row based on its salient fields.
        /// </summary>
        private static int ComputeRowId(LogEntry e)
        {
            unchecked
            {
                int h = 17;
                h = h * 31 + e.TimeUtc.Ticks.GetHashCode();
                h = h * 31 + e.Level.GetHashCode();
                h = h * 31 + e.Channel.GetHashCode();
                h = h * 31 + (e.Message?.GetHashCode() ?? 0);
                return h;
            }
        }

        /// <summary>
        /// Draws a toggle button for a given <see cref="LogChannel"/> and mutates the active filter.
        /// </summary>
        private void DrawChannelToggle(LogChannel ch)
        {
            bool has = (_channels & ch) != 0;
            bool next = GUILayout.Toggle(has, ch.ToString(), GUI.skin.button);
            if (next != has)
            {
                if (next) _channels |= ch;
                else _channels &= ~ch;
            }
        }

        /// <summary>
        /// Renders the Console tab: output pane, input line, history, and command suggestions.
        /// </summary>
        private void DrawConsole()
        {
            // Output pane
            _consoleScroll = GUILayout.BeginScrollView(_consoleScroll, GUILayout.ExpandHeight(true));
            GUILayout.TextArea(_consoleOutput, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();

            // Input row
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("consoleInput");

            var e = Event.current;
            bool isEnterPressed = false;
            bool isUpArrowPressed = false;
            bool isDownArrowPressed = false;
            bool isTabPressed = false;
            bool isEscapePressed = false;
            bool isCtrlSpacePressed = false;

            bool inputFocused = (GUI.GetNameOfFocusedControl() == "consoleInput");

            if (inputFocused && (e.type == EventType.KeyDown || e.type == EventType.KeyUp))
            {
                var kc = e.keyCode;

                if (kc == KeyCode.Return || kc == KeyCode.KeypadEnter)
                {
                    isEnterPressed = (e.type == EventType.KeyDown);
                    e.Use();
                    _forceRefocusNextRepaint = true;
                }
                else if (kc == KeyCode.UpArrow)
                {
                    isUpArrowPressed = (e.type == EventType.KeyDown);
                    e.Use();
                    _forceRefocusNextRepaint = true;
                }
                else if (kc == KeyCode.DownArrow)
                {
                    isDownArrowPressed = (e.type == EventType.KeyDown);
                    e.Use();
                    _forceRefocusNextRepaint = true;
                }
                else if (kc == KeyCode.Tab || e.character == '\t')
                {
                    isTabPressed = (e.type == EventType.KeyDown);
                    e.Use();
                    _forceRefocusNextRepaint = true;
                }
                else if (kc == KeyCode.Escape)
                {
                    isEscapePressed = (e.type == EventType.KeyDown);
                    e.Use();
                    _forceRefocusNextRepaint = true;
                }
                else if (kc == KeyCode.Space && (e.control || e.command))
                {
                    isCtrlSpacePressed = (e.type == EventType.KeyDown);
                    e.Use();
                    _forceRefocusNextRepaint = true;
                }
            }

            _input = GUILayout.TextField(_input, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint)
                _inputRect = GUILayoutUtility.GetLastRect();
            if (GUILayout.Button("Run", GUILayout.Width(60))) _ = RunInput();
            GUILayout.EndHorizontal();

            if (_forceRefocusNextRepaint && Event.current.type == EventType.Repaint)
            {
                GUIUtility.keyboardControl = 0;        // clear any stray control
                GUI.FocusControl("consoleInput");      // re-focus our field
                _forceRefocusNextRepaint = false;
            }

            if (_input != _prevInput)
            {
                _prevInput = _input;
                UpdateSuggestionsFor(_input);
            }

            // Manual open with Ctrl+Space
            if (isCtrlSpacePressed)
            {
                UpdateSuggestionsFor(_input);
                _showSuggest = _suggestions.Count > 0;
                e.Use();
            }

            if (_showSuggest && (isUpArrowPressed || isDownArrowPressed))
            {
                if (isUpArrowPressed) _suggestIndex = Mathf.Max(0, _suggestIndex - 1);
                if (isDownArrowPressed) _suggestIndex = Mathf.Min(_suggestions.Count - 1, _suggestIndex + 1);
                e.Use();
            }
            else
            {
                // History navigation when popup is hidden
                if (isUpArrowPressed) { History(-1); e.Use(); }
                if (isDownArrowPressed) { History(+1); e.Use(); }
            }

            if (_showSuggest && isTabPressed)
            {
                AcceptSuggestion();
                e.Use();
                GUI.FocusControl("consoleInput");
            }
            else if (_showSuggest && isEscapePressed)
            {
                _showSuggest = false;
                _suggestions.Clear();
                _suggestIndex = -1;
                e.Use();
                GUI.FocusControl("consoleInput");
            }
            else if (isEnterPressed)
            {
                // If suggestion is open and a row is selected, accept it first; second Enter runs the command.
                if (_showSuggest && _suggestIndex >= 0)
                {
                    AcceptSuggestion();
                    e.Use();
                    GUI.FocusControl("consoleInput");
                }
                else
                {
                    e.Use();
                    _ = RunInput();
                    GUI.FocusControl("consoleInput");
                }
            }

            // Compute popup rect and request an overlay draw (outside window clipping)
            if (_showSuggest && _suggestions.Count > 0 && _inputRect.height > 0f)
            {
                float rowH = 20f;
                int maxRows = Mathf.Min(8, _suggestions.Count);

                // Rect in window-local space (below input)
                var popupLocal = new Rect(_inputRect.x, _inputRect.yMax, _inputRect.width, rowH * maxRows);

                // Convert to absolute screen space (account for title bar)
                _suggestPopupRectScreen = new Rect(
                    _windowRect.x + popupLocal.x,
                    _windowRect.y + TITLEBAR_H + popupLocal.y,
                    popupLocal.width,
                    popupLocal.height
                );

                _drawSuggestOverlay = true;
            }
        }

        /// <summary>
        /// Draws the detached suggestions popup over the screen (outside the window’s clip).
        /// </summary>
        private void DrawSuggestOverlay()
        {
            int oldDepth = GUI.depth;
            GUI.depth = -10000;

            // Fullscreen group so absolute coordinates can be used
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));

            var popupRect = _suggestPopupRectScreen;
            GUI.Box(popupRect, GUIContent.none);

            float rowH = 20f;
            int maxRows = Mathf.Min(8, _suggestions.Count);
            float scrollY = Mathf.Clamp(_suggestIndex - 3, 0, Mathf.Max(0, _suggestions.Count - maxRows)) * rowH;

            for (int i = 0; i < _suggestions.Count && i < 100; i++)
            {
                var r = new Rect(popupRect.x, popupRect.y + (i * rowH - scrollY), popupRect.width, rowH);
                if (r.yMax < popupRect.y || r.yMin > popupRect.y + popupRect.height) continue;

                bool hover = r.Contains(Event.current.mousePosition);
                if (hover && Event.current.type == EventType.MouseMove) _suggestIndex = i;

                // Highlight selected row
                var old = GUI.color;
                if (i == _suggestIndex)
                {
                    GUI.color = new Color(0.20f, 0.55f, 1f, 0.50f);
                    GUI.Box(r, GUIContent.none);
                    GUI.color = Color.white;

                    // Left accent bar
                    var bar = new Rect(r.x + 2, r.y + 2, 3, r.height - 4);
                    GUI.DrawTexture(bar, Texture2D.whiteTexture);
                }
                else
                {
                    GUI.color = new Color(0, 0, 0, 0);
                    GUI.Box(r, GUIContent.none);
                }
                GUI.color = old;

                // Label with left padding so text doesn't sit on the accent bar
                var labelRect = new Rect(r.x + 8, r.y, r.width - 8, r.height);
                GUI.Label(labelRect, _suggestions[i]);

                if (hover && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _suggestIndex = i;
                    AcceptSuggestion();
                    Event.current.Use();
                    GUI.FocusControl("consoleInput");
                    break;
                }
            }

            GUI.EndGroup();
            GUI.depth = oldDepth;
        }

        /// <summary>
        /// Executes the current input in the console, appends the output, and updates history.
        /// </summary>
        private async System.Threading.Tasks.Task RunInput()
        {
            var raw = _input.Trim();
            if (string.IsNullOrEmpty(raw)) return;

            _history.Add(raw);
            _historyIndex = _history.Count;

            var ctx = Parse(raw);
            var result = await CommandRegistry.Instance.ExecuteAsync(ctx);
            AppendConsole($"> {raw}\n{result}\n");
            _input = string.Empty;
        }

        /// <summary>
        /// Navigates the console command history.
        /// </summary>
        /// <param name="dir">-1 for previous, +1 for next.</param>
        private void History(int dir)
        {
            if (_history.Count == 0) return;
            _historyIndex = Mathf.Clamp(_historyIndex + dir, 0, _history.Count);
            if (_historyIndex >= 0 && _historyIndex < _history.Count)
                _input = _history[_historyIndex];
        }

        /// <summary>
        /// Parses a raw command line into a <see cref="CommandContext"/>.
        /// </summary>
        private CommandContext Parse(string raw)
        {
            var ctx = new CommandContext { Raw = raw, Player = null };
            var parts = SplitArgs(raw);
            if (parts.Count == 0) { ctx.Command = string.Empty; return ctx; }

            ctx.Command = parts[0].ToLowerInvariant();
            for (int i = 1; i < parts.Count; i++)
            {
                var p = parts[i];
                if (p.StartsWith("--", StringComparison.Ordinal))
                {
                    var eq = p.IndexOf('=');
                    if (eq > 2) ctx.Named[p.Substring(2, eq - 2)] = p.Substring(eq + 1);
                }
                else
                {
                    ctx.Args.Add(p);
                }
            }
            return ctx;
        }

        /// <summary>
        /// Splits a command line into tokens, supporting quotes to keep spaces.
        /// </summary>
        private static List<string> SplitArgs(string input)
        {
            var res = new List<string>();
            bool inQuote = false;
            var cur = string.Empty;

            foreach (char c in input)
            {
                if (c == '"') { inQuote = !inQuote; continue; }
                if (!inQuote && char.IsWhiteSpace(c))
                {
                    if (cur.Length > 0) { res.Add(cur); cur = string.Empty; }
                }
                else cur += c;
            }
            if (cur.Length > 0) res.Add(cur);
            return res;
        }

        /// <summary>
        /// Builds a clipboard-friendly plain-text dump of all logs currently stored.
        /// </summary>
        private string BuildClipboard()
        {
            var list = new List<LogEntry>();
            LogStore.Instance.Snapshot(list);
            System.Text.StringBuilder sb = new();
            foreach (var e in list)
                sb.AppendLine($"[{e.TimeUtc:O}] {e.Level} {e.Channel} | {e.Message}\n{e.StackTrace}");
            return sb.ToString();
        }

        private void OnLogAdded(LogEntry e)
        {
            if (_pauseOnError && e.Level == LogLevel.Error)
                visible = true;
        }

        private void AppendConsole(string text)
        {
            _consoleOutput += text;
            _consoleScroll.y = 999999f;
        }

        /// <summary>
        /// Updates the command suggestions list for the current input (first token only).
        /// </summary>
        private void UpdateSuggestionsFor(string text)
        {
            string trimmed = text.TrimStart();
            int space = trimmed.IndexOf(' ');
            string prefix = space >= 0 ? trimmed.Substring(0, space) : trimmed;

            if (string.IsNullOrEmpty(prefix))
            {
                _suggestions.Clear();
                _showSuggest = false;
                _suggestIndex = -1;
                return;
            }

            var list = new List<string>();
            foreach (var name in CommandRegistry.Instance.Names)
            {
                if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    list.Add(name);
            }

            _suggestions = list;
            _showSuggest = _suggestions.Count > 0;
            _suggestIndex = _suggestions.Count > 0 ? Mathf.Clamp(_suggestIndex, 0, _suggestions.Count - 1) : -1;
        }

        /// <summary>
        /// Accepts the currently highlighted suggestion and replaces only the first token.
        /// </summary>
        private void AcceptSuggestion()
        {
            if (!_showSuggest || _suggestIndex < 0 || _suggestIndex >= _suggestions.Count) return;

            string trimmed = _input.TrimStart();
            int leadingSpaces = _input.Length - trimmed.Length;
            int space = trimmed.IndexOf(' ');
            string rest = space >= 0 ? trimmed.Substring(space) : string.Empty;

            _input = new string(' ', leadingSpaces) + _suggestions[_suggestIndex] + rest;

            _showSuggest = false;
            _suggestions.Clear();
            _suggestIndex = -1;
        }
    }
}
