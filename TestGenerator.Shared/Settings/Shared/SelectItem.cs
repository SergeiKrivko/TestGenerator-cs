namespace TestGenerator.Shared.Settings.Shared;

public class SelectItem<T> : ISelectItem
{
   public string Name { get; set; } = "";
   public string? Icon { get; set; }
   public T? Value { get; set; }

   public bool HasIcon => Icon != null;
}