using System.IO;
using AvaluxUI.Utils;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Types;

namespace TestGenerator.FilesTab;

public class DirectoryCreator : IFileCreator
{
    public string Key => "CreateDirectory";
    public string Name => "Папку";
    public int Priority => 100;
    public string? Icon => "M2.34225e-05 2.50001C5.01254e-05 1.50001 0.999996 2.61779e-05 2.50002 6.2865e-06H6.50002C7.5 -8.86825e-06 8.5 1.50001 9.50002 1.50001H20C22 1.50001 22.5 3.50001 22.5 3.50001V15.5C22.5 17.5 20.5 18 20 18H2.50002C0.500011 18 5.88746e-05 16 2.34225e-05 15.5V2.50001ZM2.50002 1.50001C2.50002 1.50001 1.5 1.5 1.5 2.5V5.5H21V4C21 4 20.8246 3 20 3H9.50002C8 3 7 1.5 6.50002 1.50001H2.50002ZM21 7H1.5V15.5C1.5 16.5 2.5 16.5 2.5 16.5H20C21 16.5 21 15.5 21 15.5V7Z";

    public SettingsControl? GetSettingsControl()
    {
        var settingsControl = new SettingsControl();
        settingsControl.Add(new StringField{Key = "Name", FieldName = "Имя папки"});
        return settingsControl;
    }

    public void Create(string root, ISettingsSection options)
    {
        var filename = options.Get<string>("Name");
        if (!string.IsNullOrWhiteSpace(filename))
            Directory.CreateDirectory(Path.Join(root, filename.Trim()));
    }
}