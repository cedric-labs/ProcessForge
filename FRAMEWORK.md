<p align="right">
  <b>English</b> | <a href="FRAMEWORK.fr.md">Français</a>
</p>

# ProcessForge — Framework Specification & AI Documentation

This document is the technical manifest intended for AI agents and software engineers. It outlines all capabilities, architectural components, and public contracts exposed by **ProcessForge**.

---

## 🎯 Scope & Responsibility

ProcessForge is a cross-platform .NET system engineering library dedicated to **process inspection, execution metadata analysis, and fine-grained resolution of underlying execution targets** (Java wrappers, script interpreters including CMD, PowerShell, Bash, and Python).

---

## 🚀 Key Capabilities

* **Cross-Platform Inspection**: Resolution of executable absolute paths, complete command lines (`/proc/<pid>/cmdline` on Linux, Win32 APIs on Windows), and execution accounts/usernames.
* **Wrapper Resolution**: Extracts underlying application targets (e.g., `.jar` archives executed via `java -jar`, scripts such as `.bat`/`.cmd`/`.ps1`/`.sh` hosted inside shell interpreters).
* **Multi-Criteria Filtering**: Evaluation engine (`ProcessExclusionFilter`) matching by PID, exact name, prefix, suffix, substring (`Contains`), and system execution accounts (`SYSTEM`, `NT AUTHORITY`, `root`), seamlessly considering process name, executable path, and underlying target (`TargetExecutable`).
* **Thread-Safe Dynamic Exclusion**: Method `ExcludeExactName(string processName)` allows dynamic runtime additions of process or target exclusions without rebuilding the filter.

---

## 📋 Maintenance Protocol for AI Agents

All enhancements to ProcessForge must comply with:
1. **Backward Compatibility:** Preserve all existing public contracts. Use overloads or optional parameters.
2. **Code Cleanliness:** Zero warnings (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`), zero pragmas/suppressions.
3. **Testing Standards:** Test naming convention `TargetMethod_ScenarioOrCondition_ExpectedResult`.
4. **Documentation:** Update both `FRAMEWORK.md` and `FRAMEWORK.fr.md` immediately upon introducing or modifying capabilities.
5. **Validation:** Full pass of `./verify.ps1`.
