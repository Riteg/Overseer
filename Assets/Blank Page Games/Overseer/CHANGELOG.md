# Changelog

All notable changes to **Overseer** will be documented in this file.

---

## [1.0.0] - 2025-09-18
### Added
- Initial release of **Overseer** by Blank Page Games.
- In-game log overlay with filtering, search, stack trace expansion, and clipboard export.
- Developer console with command history, suggestions, and extensible command system.
- Built-in commands:
  - `help` — lists commands or shows help for a command.
  - `clear` — clears in-game logs.
  - `echo` — echoes arguments back.
  - `gc` — forces garbage collection.
  - `timescale` — gets or sets `Time.timeScale`.
  - `getvar` — inspects static fields and properties via reflection.
  - `setvar` — modifies static fields and properties via reflection.
- Automatic bootstrapper (`NSLogBootstrap`) that initializes Overseer on startup.
- Samples~ folder with `ExampleCommand.cs` for quick extension reference.

---

## [Unreleased]
### Planned
- Command argument auto-completion.
- Configurable overlay hotkey.
- Persistent command history between sessions.
- UI Toolkit version of the overlay.
- Advanced log filtering (by thread, context, etc.).
