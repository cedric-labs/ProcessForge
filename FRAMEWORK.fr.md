<p align="right">
  <a href="FRAMEWORK.md">English</a> | <b>Français</b>
</p>

# ProcessForge — Spécification & Documentation IA du Framework

Ce document est le manifeste technique destiné à l'agent IA et aux développeurs. Il récapitule l'ensemble des capacités, composants et contrats publics exposés par **ProcessForge**.

---

## 🎯 Périmètre & Responsabilité

ProcessForge est le framework partagé .NET d'**inspection système, d'analyse de métadonnées de processus et de résolution fine des cibles d'exécution réelles** (wrappers Java, scripts CMD, PowerShell, Bash, Python).

---

## 🚀 Capacités Clés

* **Inspection multiplateforme** : Résolution du chemin absolu de l'exécutable, ligne de commande (`/proc/<pid>/cmdline` ou Win32), et compte qui exécute le processus (`UserName`) : jeton d'accès du processus sous Windows (`DOMAINE\compte`, ou le SID quand le compte n'a plus de nom), UID réel de `/proc/<pid>/status` traduit par `/etc/passwd` sous Linux (l'UID numérique quand le compte n'y figure pas). Le compte reste vide quand le processus ne peut pas être ouvert (processus protégé, droits insuffisants).
* **Résolution de wrappers** : Extraction de la cible applicative sous-jacente (ex: fichier `.jar` exécuté via `java -jar`, scripts `.bat`/`.cmd`/`.ps1`/`.sh` sous les interpréteurs d'ordres).
* **Filtrage multicritère** : Moteur d'évaluation (`ProcessExclusionFilter`) par PID, nom exact, préfixe, suffixe, sous-chaîne (`Contains`) et filtrage des comptes système (`SYSTEM`, `NT AUTHORITY`, `root`), en tenant compte du nom de processus, du fichier exécutable et de la cible réelle (`TargetExecutable`).
* **Exclusion dynamique thread-safe** : Méthode `ExcludeExactName(string processName)` permettant d'ajouter des exclusions de processus ou de cibles à chaud au cours de l'exécution sans recréer le filtre.

---

## 📋 Protocole de Maintenance par l'Agent IA

Toute modification de ProcessForge doit respecter :
1. **Rétrocompatibilité :** Conserver tous les contrats publics existants. Utiliser des surcharges ou des paramètres optionnels.
2. **Qualité :** Zéro warning (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`), zéro pragma/suppress.
3. **Tests :** Nommage `MethodeCible_ScenarioOuCondition_ResultatAttendu`.
4. **Mise à jour :** Compléter immédiatement ce fichier `FRAMEWORK.md` en cas d'ajout d'une nouvelle capacité.
5. **Validation :** Valider par `./verify.ps1`.
