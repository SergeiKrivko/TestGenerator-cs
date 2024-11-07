namespace TestGenerator.Core.Types;

public interface IRequestHandler
{
    public string Key { get; }
    public Task<object?> Call(object? data);
}