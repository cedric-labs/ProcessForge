# ProcessForge

Cross-platform .NET process inspection framework for extracting command-line metadata, resolving underlying execution targets (Java wrappers, script interpreters), and evaluating exclusion rules.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Framework](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Documentation: English](https://img.shields.io/badge/Docs-English-blue.svg)](FRAMEWORK.md)
[![Documentation: Français](https://img.shields.io/badge/Docs-Français-green.svg)](FRAMEWORK.fr.md)

> 📖 **Framework Documentation**: [English Specification](FRAMEWORK.md) | [Spécification en français](FRAMEWORK.fr.md)

---

## Key Capabilities

* **Cross-Platform Process Inspection**: Inspects active processes across Windows (Win32 Toolhelp32/NtQuery) and Linux (`/proc/<pid>/cmdline`), resolving executable paths, process names, owner accounts, and start times.
* **Execution Wrapper Target Resolution**: Automatically inspects command-line arguments to determine the true underlying application target:
  * Java wrappers: extracts target `.jar` files executed via `java -jar app.jar`.
  * Script interpreters: extracts executing scripts (`.ps1`, `.bat`, `.cmd`, `.sh`, `.py`, `.js`).
* **Multi-Criteria Dynamic Filtering (`ProcessExclusionFilter`)**:
  * Evaluates exclusions by process numeric ID (PID), exact name, prefix, suffix, and substring matches.
  * Filters out operating system service accounts (`SYSTEM`, `NT AUTHORITY`, `root`).
  * Thread-safe on-the-fly rule additions (`ExcludeExactName`).

---

## Installation

Add the package via the .NET CLI:

```powershell
dotnet add package ProcessForge --version 1.1.2
```

---

## Code Example

```csharp
using ProcessForge.Inspection;
using ProcessForge.Rules;

var inspector = new LocalProcessInspector();
var filter = new ProcessExclusionFilter();
filter.ExcludeExactName("svchost");

if (inspector.TryGetProcessDetails(4812, out var details))
{
    Console.WriteLine($"Process: {details.ProcessName}");
    Console.WriteLine($"Command Line: {details.CommandLine}");
    Console.WriteLine($"Target Executable: {details.TargetExecutable}");
}
```

---

## License

This project is licensed under the [MIT License](LICENSE).
