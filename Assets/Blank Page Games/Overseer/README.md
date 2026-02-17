# Overseer

```
 _______ ___ ___ _______ ______ _______ _______ _______ ______ 
|       |   |   |    ___|   __ \     __|    ___|    ___|   __ \
|   -   |   |   |    ___|      <__     |    ___|    ___|      <
|_______|\_____/|_______|___|__|_______|_______|_______|___|__|
```

**Overseer** is a lightweight, developer-focused logging and console overlay for Unity projects.  
It provides real-time log visualization, filtering, and an extensible developer console with custom commands.

---

## ✨ Features

- 📜 **Log Overlay**
  - View Unity logs in-game with filters by level & channel.
  - Expand entries to inspect stack traces and context.
  - Copy logs or export to clipboard.
  - Pause automatically on errors.

- 💻 **Developer Console**
  - Toggle in-game console (default key: `F1`).
  - Built-in commands: `help`, `clear`, `echo`, `gc`, `timescale`, `getvar`, `setvar`.
  - Auto-complete suggestions for registered commands.
  - Command history navigation.

- ⚙️ **Extensible**
  - Add new commands by implementing `IDevCommand`.
  - Auto-register commands with `[DevCommand("name")]` attribute.
  - Commands can use arguments, named options, and context objects.

- 🔌 **Unity Integration**
  - Works in Unity **2022.3 LTS** and above (including Unity 6 / 6.2).
  - Compatible with `.NET Standard 2.1` scripting runtime.
  - Minimal setup, just drop it in.

---

## 🚀 Getting Started

### 1. Installation
- Copy the `BPG.Overseer` folder into your Unity project’s `Assets/` directory.
- Or import the Overseer package from the Unity Asset Store (when published).

### 2. Usage
At runtime, Overseer auto-initializes via `NSLogBootstrap`.  
A `[DevTools]` GameObject is created with:
- `UnityLogBridge` (captures Unity logs)
- `LogOverlayUI` (in-game logs & console)

Press **F1** to toggle the overlay.

### 3. Developer Console Commands
| Command       | Description                                    | Example                        |
|---------------|------------------------------------------------|--------------------------------|
| `help`        | Lists commands or shows help for one           | `help timescale`               |
| `clear`       | Clears the log buffer                          | `clear`                        |
| `echo`        | Prints arguments back                          | `echo hello world`             |
| `gc`          | Forces garbage collection                      | `gc`                           |
| `timescale`   | Gets or sets Unity `Time.timeScale`            | `timescale 0.5`                |
| `getvar`      | Reads a static field or property via reflection| `getvar UnityEngine.Time timeScale` |
| `setvar`      | Sets a static field or property via reflection | `setvar UnityEngine.Time timeScale 1` |

### 4. Writing Custom Commands
Create a new class implementing `IDevCommand`:

```csharp
[DevCommand("hello", "Prints hello world")]
public sealed class HelloCommand : IDevCommand
{
    public string Name => "hello";
    public string Help => "hello";
    public Task<string> ExecuteAsync(CommandContext ctx)
    {
        return Task.FromResult("Hello, world!");
    }
}
```

It will be auto-registered when the game starts.

---

## 📂 Project Structure

```
BPG.Overseer/
 └── Scripts/
     ├── BuiltInCommands/   # getvar, setvar gibi hazır konsol komutları
     ├── Runtime/
     │   ├── DevConsole/    # Developer console (commands, registry, attributes)
     │   ├── Diagnostics/   # Logging system (LogEntry, LogStore, OSLog, etc.)
     │   └── UI/            # IMGUI overlay & bootstrap
     └── Samples/           # Example scripts
```

---

## 🛠️ Requirements

- Unity **2022.3 LTS** or higher  
- .NET Standard 2.1 runtime

---

## 📜 License

```
Copyright (c) 2025 Blank Page Games

Permission is granted to use, modify, and distribute this software in personal or commercial projects.
However, you may not sell, sublicense, or redistribute the software itself or any modified version as a standalone product.
```

---

## 🧩 Future Improvements

- Argument auto-completion for console commands  
- Configurable overlay hotkey  
- Persistent command history  
- UI Toolkit integration (optional alternative to IMGUI)  
- Richer log filtering (channels, threads, contexts)  

---

## 📞 Support

For bug reports or feature requests, please contact **Blank Page Games**.  
