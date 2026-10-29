namespace ProcessForge.Models;

/// <summary>
/// Detailed metadata of an executable process resolved by the system.
/// </summary>
/// <param name="ProcessId">Unique process identifier (PID).</param>
/// <param name="ProcessName">Short process name.</param>
/// <param name="ExecutablePath">Full absolute path of the executable binary.</param>
/// <param name="CommandLine">Full launch command line.</param>
/// <param name="TargetExecutable">Actual target detected (for example a JAR file or a .bat, .cmd, .ps1 or .sh script).</param>
/// <param name="UserName">Account the process runs under.</param>
public sealed record ProcessMetadata(
    int ProcessId,
    string ProcessName,
    string ExecutablePath,
    string CommandLine,
    string TargetExecutable,
    string UserName);
