# ProcessForge

Framework partagé .NET d'inspection système, d'analyse de métadonnées de processus et de résolution fine des cibles d'exécution réelles (wrappers Java, scripts CMD, PowerShell, Bash, Python).

## 🚀 Capacités Clés

* **Inspection multiplateforme** : Résolution du chemin absolu de l'exécutable, ligne de commande (`/proc/<pid>/cmdline` ou Win32), et compte d'exécution.
* **Résolution de wrappers** : Extraction de la cible applicative sous-jacente (ex: fichier `.jar` exécuté via `java -jar`, scripts `.bat`/`.cmd`/`.ps1`/`.sh` sous les interpréteurs d'ordres).
* **Filtrage multicritère** : Moteur d'évaluation (`ProcessExclusionFilter`) par PID, nom exact, préfixe, suffixe, sous-chaîne (`Contains`) et filtrage des comptes système (`SYSTEM`, `NT AUTHORITY`, `root`).
