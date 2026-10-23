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
    /// Ajoute dynamiquement un nom exact de processus ou de cible exécutable à exclure au cours de l'exécution.
    /// </summary>
    /// <param name="processName">Nom exact du processus ou de la cible à exclure.</param>
    public void ExcludeExactName(string processName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(processName);
        lock (_syncLock)
        {
            _dynamicallyExcludedExactNames.Add(processName.Trim());
        }
    }

    /// <summary>
    /// Détermine si un processus répond aux critères d'exclusion (par PID, compte système, nom de processus ou exécutable cible).
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

        var candidateNames = ExtractCandidateNames(process);
        if (candidateNames.Count == 0)
        {
            return false;
        }

        lock (_syncLock)
        {
            for (var i = 0; i < candidateNames.Count; i++)
            {
                if (_dynamicallyExcludedExactNames.Contains(candidateNames[i]))
                {
                    return true;
                }
            }
        }

        for (var i = 0; i < candidateNames.Count; i++)
        {
            if (MatchesNameRules(candidateNames[i]))
            {
                return true;
            }
        }

        return false;
    }

    private static List<string> ExtractCandidateNames(ProcessMetadata process)
    {
        var list = new List<string>(6);

        AddCandidateIfValid(list, process.ProcessName);

        if (!string.IsNullOrWhiteSpace(process.TargetExecutable))
        {
            var rawTarget = process.TargetExecutable.Trim().Trim('"', '\'');
            AddCandidateIfValid(list, rawTarget);

            if (rawTarget.Contains('/') || rawTarget.Contains('\\'))
            {
                var targetFile = Path.GetFileName(rawTarget);
                AddCandidateIfValid(list, targetFile);

                var targetBase = Path.GetFileNameWithoutExtension(rawTarget);
                AddCandidateIfValid(list, targetBase);
            }
            else
            {
                var targetBase = Path.GetFileNameWithoutExtension(rawTarget);
                AddCandidateIfValid(list, targetBase);
            }
        }

        if (!string.IsNullOrWhiteSpace(process.ExecutablePath))
        {
            var exeFile = Path.GetFileName(process.ExecutablePath);
            AddCandidateIfValid(list, exeFile);

            var exeBase = Path.GetFileNameWithoutExtension(process.ExecutablePath);
            AddCandidateIfValid(list, exeBase);
        }

        return list;
    }

    private static void AddCandidateIfValid(List<string> list, string? candidate)
    {
        if (!string.IsNullOrWhiteSpace(candidate) && !list.Contains(candidate, StringComparer.OrdinalIgnoreCase))
        {
            list.Add(candidate);
        }
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
