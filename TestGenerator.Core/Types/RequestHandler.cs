using System.Text.Json;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

public class RequestHandler : IRequestHandler
{
    public string Key { get; }

    private Func<object?, CancellationToken, Task<object?>>? _func = null;

    public RequestHandler(string key)
    {
        Key = key;
    }

    public void SetHandler<TI, TO>(Func<TI, Task<TO>> handler)
    {
        async Task<object?> F(object? obj, CancellationToken token)
        {
            if (obj is TI tObj)
                return await handler(tObj);
            var data = JsonSerializer.Deserialize<TI>(JsonSerializer.Serialize(obj));
            if (data == null)
                throw new InvalidCastException();
            return await handler(data);
        }

        _func = F;
    }

    public void SetHandler<TO>(Func<Task<TO>> handler)
    {
        async Task<object?> F(object? obj, CancellationToken token)
        {
            return await handler();
        }

        _func = F;
    }

    public void SetHandler<TI, TO>(Func<TI, CancellationToken, Task<TO>> handler)
    {
        async Task<object?> F(object? obj, CancellationToken token)
        {
            if (obj is TI tObj)
                return await handler(tObj, token);
            var data = JsonSerializer.Deserialize<TI>(JsonSerializer.Serialize(obj));
            if (data == null)
                throw new InvalidCastException();
            return await handler(data, token);
        }

        _func = F;
    }

    public void SetHandler<TO>(Func<CancellationToken, Task<TO>> handler)
    {
        async Task<object?> F(object? obj, CancellationToken token)
        {
            return await handler(token);
        }

        _func = F;
    }

    public async Task<object?> Call(object? data, CancellationToken token = new())
    {
        if (_func == null)
            throw new Exception("Handler not set");
        return await _func(data, token);
    }
}