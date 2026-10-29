namespace ProcessForge.Filtering;

/// <summary>
/// Configurable options for process filtering and exclusion.
/// </summary>
public sealed record ProcessExclusionOptions
{
    /// <summary>
    /// Process identifiers (PIDs) to exclude.
    /// </summary>
    public IReadOnlyList<int> ExcludedProcessIds { get; init; } = [0, 4];

    /// <summary>
    /// Exact process names to exclude.
    /// </summary>
    public IReadOnlyList<string> ExcludedExactNames { get; init; } = ["System", "Idle"];

    /// <summary>
    /// Name prefixes to exclude.
    /// </summary>
    public IReadOnlyList<string> ExcludedNamePrefixes { get; init; } = [];

    /// <summary>
    /// Name suffixes to exclude.
    /// </summary>
    public IReadOnlyList<string> ExcludedNameSuffixes { get; init; } = [];

    /// <summary>
    /// Name fragments to exclude.
    /// </summary>
    public IReadOnlyList<string> ExcludedNameContains { get; init; } = [];

    /// <summary>
    /// Indicates whether system accounts (NT AUTHORITY, SYSTEM, root) are excluded.
    /// </summary>
    public bool ExcludeSystemAccounts { get; init; } = true;
}
