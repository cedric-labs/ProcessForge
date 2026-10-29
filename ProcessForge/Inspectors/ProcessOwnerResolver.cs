using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace ProcessForge.Inspectors;

/// <summary>
/// Resolves the account that owns a process: access token on Windows (<c>DOMAIN\account</c>),
/// real UID from <c>/proc/&lt;pid&gt;/status</c> translated through <c>/etc/passwd</c> on Linux.
/// </summary>
internal static partial class ProcessOwnerResolver
{
    private const uint ProcessQueryLimitedInformation = 0x1000;
    private const uint TokenQuery = 0x0008;
    private const int TokenUserInformationClass = 1;
    private const string UidField = "Uid:";

    /// <summary>
    /// Returns the process account, or an empty string when it cannot be read.
    /// </summary>
    public static string Resolve(int processId)
    {
        if (OperatingSystem.IsWindows())
        {
            return ResolveOnWindows(processId);
        }

        return OperatingSystem.IsLinux() ? ResolveOnLinux(processId) : string.Empty;
    }

    /// <summary>
    /// Extracts the real UID (first field of the <c>Uid:</c> line) from the content of <c>/proc/&lt;pid&gt;/status</c>.
    /// </summary>
    public static int? ParseRealUserId(string statusContent)
    {
        foreach (var line in statusContent.Split('\n'))
        {
            if (!line.StartsWith(UidField, StringComparison.Ordinal))
            {
                continue;
            }

            var fields = line[UidField.Length..].Split(['\t', ' '], StringSplitOptions.RemoveEmptyEntries);
            return fields.Length > 0 && int.TryParse(fields[0], NumberStyles.None, CultureInfo.InvariantCulture, out var userId)
                ? userId
                : null;
        }

        return null;
    }

    /// <summary>
    /// Looks up the account name of a UID in the content of <c>/etc/passwd</c>; returns the UID as text when it is not listed
    /// (LDAP directory or container accounts missing from the local file).
    /// </summary>
    public static string FindUserName(string passwdContent, int userId)
    {
        var userIdText = userId.ToString(CultureInfo.InvariantCulture);
        foreach (var line in passwdContent.Split('\n'))
        {
            var fields = line.Split(':');
            if (fields.Length > 2 && string.Equals(fields[2], userIdText, StringComparison.Ordinal))
            {
                return fields[0];
            }
        }

        return userIdText;
    }

    [SupportedOSPlatform("windows")]
    private static string ResolveOnWindows(int processId)
    {
        // PROCESS_QUERY_LIMITED_INFORMATION suffices to read the token, including for services of other accounts
        // when the caller is elevated; protected processes still refuse it and keep an empty account.
        using var process = OpenProcess(ProcessQueryLimitedInformation, inheritHandle: false, processId);
        if (process.IsInvalid || !OpenProcessToken(process, TokenQuery, out var token))
        {
            return string.Empty;
        }

        using (token)
        {
            _ = GetTokenInformation(token, TokenUserInformationClass, IntPtr.Zero, 0, out var length);
            if (length <= 0)
            {
                return string.Empty;
            }

            // TOKEN_USER starts with a pointer to the SID inside the same buffer, so the buffer must stay unmanaged
            // and alive until the SID has been copied by SecurityIdentifier.
            var buffer = Marshal.AllocHGlobal(length);
            try
            {
                return GetTokenInformation(token, TokenUserInformationClass, buffer, length, out _)
                    ? TranslateAccount(new SecurityIdentifier(Marshal.ReadIntPtr(buffer)))
                    : string.Empty;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }

    [SupportedOSPlatform("windows")]
    private static string TranslateAccount(SecurityIdentifier sid)
    {
        try
        {
            return sid.Translate(typeof(NTAccount)).Value;
        }
        catch (IdentityNotMappedException)
        {
            // A deleted local account or an unreachable domain account has no name any more; its SID still
            // identifies the owner, which is better than an empty column.
            return sid.Value;
        }
    }

    private static string ResolveOnLinux(int processId)
    {
        try
        {
            var userId = ParseRealUserId(File.ReadAllText($"/proc/{processId}/status"));
            return userId is null ? string.Empty : FindUserName(File.ReadAllText("/etc/passwd"), userId.Value);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // The process may end between the socket scan and this read, or /etc/passwd may be unreadable in a
            // restricted container: the socket stays recorded with an empty account rather than being dropped.
            return string.Empty;
        }
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial SafeProcessHandle OpenProcess(uint desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, int processId);

    [LibraryImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool OpenProcessToken(SafeProcessHandle processHandle, uint desiredAccess, out SafeAccessTokenHandle tokenHandle);

    [LibraryImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetTokenInformation(SafeAccessTokenHandle tokenHandle, int tokenInformationClass, IntPtr tokenInformation, int tokenInformationLength, out int returnLength);
}
