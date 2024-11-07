using System.Text.Json;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

public class RequestHandler : IRequestHandler
{
    public string Key { get; }

    private delegate Task<object?> Func(object? obj);

    private Func? _func = null;

    public RequestHandler(string key)
    {
        Key = key;
    }

    public void SetHandler<TI, TO>(AAppService.RequestHandler<TI, TO> handler)
    {
        async Task<object?> F(object? obj)
        {
            if (obj is TI)
                return await handler((TI)obj);
            var data = JsonSerializer.Deserialize<TI>(JsonSerializer.Serialize(obj));
            if (data == null)
                throw new InvalidCastException();
            return await handler(data);
        }

        _func = F;
    }

    public void SetHandler<TO>(AAppService.RequestHandler<TO> handler)
    {
        async Task<object?> F(object? obj)
        {
            return await handler();
        }

        _func = F;
    }

    public async Task<object?> Call(object? data)
    {
        if (_func == null)
            throw new Exception("Handler not set");
        return await _func(data);
    }
}