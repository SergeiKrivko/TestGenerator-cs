using System.Collections.ObjectModel;
using TestGenerator.Shared.Settings.Shared;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Settings;

public class SelectField<T> : BaseSelectField where T : class
{
    private ObservableCollection<SelectItem<T>> _items = [];

    public ObservableCollection<SelectItem<T>> Items
    {
        get => _items;
        set
        {
            _items = value;
            Box.ItemsSource = _items;
        }
    }

    public override void Load(SettingsSection section)
    {
        if (Key != null)
        {
            Value = section.Get<T>(Key);
        }
    }

    public override object? Value
    {
        get => (Box.SelectedItem as SelectItem<T>)?.Value;
        set
        {
            Box.SelectedItem = Items.FirstOrDefault(i => (i.Value as IComparable<T>)?.CompareTo(value as T) == 0);
        }
    }
}