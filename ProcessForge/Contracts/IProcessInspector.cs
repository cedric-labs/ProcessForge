using ProcessForge.Models;

namespace ProcessForge.Contracts;

/// <summary>
/// Inspects an executable process and reads its metadata.
/// </summary>
public interface IProcessInspector
{
    /// <summary>
    /// Resolves the detailed metadata of a process from its numeric identifier.
    /// </summary>
    /// <param name="processId">Unique process identifier (PID).</param>
    /// <param name="cancellationToken">Cooperative cancellation token.</param>
    /// <returns>The resolved process metadata.</returns>
    Task<ProcessMetadata> ResolveProcessAsync(int processId, CancellationToken cancellationToken = default);
}
