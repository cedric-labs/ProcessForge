using ProcessForge.Models;

namespace ProcessForge.Filtering;

/// <summary>
/// Évaluateur de filtrage et d'exclusion de processus basé sur des règles paramétrables et des ajouts dynamiques thread-safe.
/// </summary>
public sealed class ProcessExclusionFilter(ProcessExclusionOptions options)
{
    private readonly ProcessExclusionOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly HashSet<string> _dynamicallyExcludedExactNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _syncLock = new();

    /// <summary>
    /// Ajoute dynamiquement un nom exact de processus à exclure au cours de l'exécution.
    /// </summary>
    /// <param name="processName">Nom exact du processus à exclure.</param>
    public void ExcludeExactName(string processName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(processName);
        lock (_syncLock)
        {
            _dynamicallyExcludedExactNames.Add(processName.Trim());
        }
    }

    /// <summary>
    /// Détermine si un processus répond aux critères d'exclusion.
    /// </summary>
    /// <param name="process">Métadonnées du processus.</param>
    /// <returns>True si le processus doit être exclu, False sinon.</returns>
    public bool ShouldExclude(ProcessMetadata process)
    {
        ArgumentNullException.ThrowIfNull(process);

        if (_options.ExcludedProcessIds.Contains(process.ProcessId))
        {
            return true;
        }

        if (_options.ExcludeSystemAccounts && IsSystemAccount(process.UserName))
        {
            return true;
        }

        var processName = process.ProcessName;
        if (string.IsNullOrWhiteSpace(processName))
        {
            return false;
        }

        lock (_syncLock)
        {
            if (_dynamicallyExcludedExactNames.Contains(processName))
            {
                return true;
            }
        }

        return MatchesNameRules(processName);
    }

    private static bool IsSystemAccount(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return false;
        }

        return userName.Contains("NT AUTHORITY", StringComparison.OrdinalIgnoreCase) ||
               userName.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase) ||
               userName.Equals("root", StringComparison.OrdinalIgnoreCase);
    }

    private bool MatchesNameRules(string name)
    {
        for (var i = 0; i < _options.ExcludedExactNames.Count; i++)
        {
            if (name.Equals(_options.ExcludedExactNames[i], StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        for (var i = 0; i < _options.ExcludedNamePrefixes.Count; i++)
        {
            if (name.StartsWith(_options.ExcludedNamePrefixes[i], StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        for (var i = 0; i < _options.ExcludedNameSuffixes.Count; i++)
        {
            if (name.EndsWith(_options.ExcludedNameSuffixes[i], StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        for (var i = 0; i < _options.ExcludedNameContains.Count; i++)
        {
            if (name.Contains(_options.ExcludedNameContains[i], StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
