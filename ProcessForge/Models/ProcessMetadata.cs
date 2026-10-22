namespace ProcessForge.Models;

/// <summary>
/// Métadonnées détaillées d'un processus exécutable résolu par le système.
/// </summary>
/// <param name="ProcessId">Identifiant unique du processus (PID).</param>
/// <param name="ProcessName">Nom court du processus.</param>
/// <param name="ExecutablePath">Chemin d'accès absolu complet au binaire exécutable.</param>
/// <param name="CommandLine">Ligne de commande complète de lancement.</param>
/// <param name="TargetExecutable">Cible réelle détectée (ex: fichier JAR, script .bat, .cmd, .ps1, .sh).</param>
/// <param name="UserName">Compte d'exécution du processus.</param>
public sealed record ProcessMetadata(
    int ProcessId,
    string ProcessName,
    string ExecutablePath,
    string CommandLine,
    string TargetExecutable,
    string UserName);
