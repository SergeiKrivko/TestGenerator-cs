using System.IO;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Utils;

namespace TestGenerator.FilesTab;

public class FileCreator : IFileCreator
{
    public string Key => "CreateFile";
    public string Name => "Файл";
    public int Priority => 100;
    public string? Icon => "M6.5 3.05176e-05C3 3.05176e-05 3 3.50003 3 3.50003V20.5C3 20.5 3 24.0001 6.5 24.0001L17.5 24C21 24.0001 21 20.5 21 20.5V7.00003C21 6 20.5 5 20.5 5L16 0.5C16 0.5 15.5 0 14 0L6.5 3.05176e-05ZM6.5 1.00003C4 1.00003 4 3.50003 4 3.50003C4 3.50003 4.05165 18 4 20.5C3.94835 23 6.5 23 6.5 23H17.5C17.5 23 20 23.0549 20 20.5V8H16.5C16.5 8 13 8 13 4.5V1L6.5 1.00003ZM12 3H6V5H12V3ZM12 7L6 7.00003V9H12V7ZM6 11V13H18V11H6ZM6 15V17H18V15H6ZM6 19V21H18V19H6ZM14 1C15 1 15.5 1.50003 15.5 1.50003L19.5 5.50003C19.5 5.50003 20 6.00003 20 7.00003H16.5C16.5 7.00003 14 7.05161 14 4.5V1Z";

    public SettingsControl? GetSettingsControl()
    {
        var settingsControl = new SettingsControl();
        settingsControl.Add(new StringField{Key = "Name", FieldName = "Имя файла"});
        return settingsControl;
    }

    public void Create(string root, SettingsSection options)
    {
        var filename = options.Get<string>("Name");
        if (!string.IsNullOrWhiteSpace(filename))
            File.Create(Path.Join(root, filename.Trim()));
    }
}