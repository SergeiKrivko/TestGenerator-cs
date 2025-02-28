using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TestGenerator.PluginInstaller;

public static class PermissionsService
{
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
        SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
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