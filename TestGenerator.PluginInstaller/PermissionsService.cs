using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TestGenerator.PluginInstaller;

public static class PermissionsService
{
    [SupportedOSPlatform("Windows")]
    private static void ClearWindowsPermissions(DirectoryInfo directory)
    {
        var dirSecurity = directory.GetAccessControl();

        foreach (var el in dirSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            Console.WriteLine(el);
            if (el is SecurityIdentifier rule)
            {
                dirSecurity.RemoveAccessRuleAll(new FileSystemAccessRule(rule, FileSystemRights.FullControl, AccessControlType.Allow));
                dirSecurity.RemoveAccessRuleAll(new FileSystemAccessRule(rule, FileSystemRights.Write, AccessControlType.Allow));
                Console.WriteLine(rule.AccountDomainSid);
            }
        }
        Console.WriteLine("ALL");

        directory.SetAccessControl(dirSecurity);
    }

    [SupportedOSPlatform("Windows")]
    private static void SetWindowsAdminPermissions(DirectoryInfo directory)
    {
        SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
        var account = (NTAccount)sid.Translate(typeof(NTAccount));
        var dirSecurity = directory.GetAccessControl();
        dirSecurity.SetOwner(account);
        directory.SetAccessControl(dirSecurity);
    }

    [SupportedOSPlatform("Windows")]
    private static void AddWindowsUserPermissions(DirectoryInfo directory)
    {
        var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
        var account = (NTAccount)sid.Translate(typeof(NTAccount));
        var dirSecurity = directory.GetAccessControl();

        dirSecurity.SetAccessRuleProtection(true, true);
        dirSecurity.AddAccessRule(new FileSystemAccessRule(account, FileSystemRights.ReadAndExecute,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None,
            AccessControlType.Allow));
    }

    [SupportedOSPlatform("Windows")]
    private static void SetWindowsPermissions(string path)
    {
        ClearWindowsPermissions(new DirectoryInfo(path));
        Thread.Sleep(TimeSpan.FromSeconds(10));
        SetWindowsAdminPermissions(new DirectoryInfo(path));
        AddWindowsUserPermissions(new DirectoryInfo(path));
    }

    public static void SetPermissions(string path)
    {
        if (OperatingSystem.IsWindows())
            SetWindowsPermissions(path);
        else
        {
            File.SetUnixFileMode(path,
                UnixFileMode.UserRead | UnixFileMode.GroupRead | UnixFileMode.OtherRead | UnixFileMode.UserExecute |
                UnixFileMode.GroupExecute | UnixFileMode.OtherExecute | UnixFileMode.UserWrite);
        }
    }
}