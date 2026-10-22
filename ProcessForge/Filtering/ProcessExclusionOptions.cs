namespace ProcessForge.Filtering;

/// <summary>
/// Options paramétrables pour le filtrage et l'exclusion de processus.
/// </summary>
public sealed record ProcessExclusionOptions
{
    /// <summary>
    /// Identifiants de processus (PID) à exclure.
    /// </summary>
    public IReadOnlyList<int> ExcludedProcessIds { get; init; } = [0, 4];

    /// <summary>
    /// Noms exacts de processus à exclure.
    /// </summary>
    public IReadOnlyList<string> ExcludedExactNames { get; init; } = ["System", "Idle"];

    /// <summary>
    /// Préfixes de noms à exclure.
    /// </summary>
    public IReadOnlyList<string> ExcludedNamePrefixes { get; init; } = [];

    /// <summary>
    /// Suffixes de noms à exclure.
    /// </summary>
    public IReadOnlyList<string> ExcludedNameSuffixes { get; init; } = [];

    /// <summary>
    /// Segments de noms à exclure.
    /// </summary>
    public IReadOnlyList<string> ExcludedNameContains { get; init; } = [];

    /// <summary>
    /// Indique si les comptes systèmes (NT AUTHORITY, SYSTEM, root) doivent être exclus.
    /// </summary>
    public bool ExcludeSystemAccounts { get; init; } = true;
}
