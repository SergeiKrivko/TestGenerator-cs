namespace TestGenerator.Shared.Settings.Shared;

internal interface ISelectItem
{
   public string Name { get; set; }
   public string? Icon { get; set; }
   public bool HasIcon { get; }
}