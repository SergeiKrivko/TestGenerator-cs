using System.Reflection.Metadata;
using System.Text.Json;
using TestGenerator.Shared;
using TestGenerator.Shared.Types;

namespace Core.Types;

class Event : IEvent
{
    public string Key { get; }
    
    private interface IHandler
    {
        public void Call(object? data);
    }
    
    private class HandlerNoParam : IHandler
    {
        private readonly AAppService.Handler _func;

        public HandlerNoParam(AAppService.Handler func)
        {
            _func = func;
        }
        
        public void Call(object? data)
        {
            _func();
        }
    }
    
    private class Handler<T> : IHandler
    {
        private readonly AAppService.Handler<T> _func;

        public Handler(AAppService.Handler<T> func)
        {
            _func = func;
        }
        
        public void Call(object? data)
        {
            Console.WriteLine($"{data?.GetType()} {typeof(T)}");
            if (data is T)
                _func((T)data);
            else
            {
                var obj = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(data));
                if (obj != null)
                    _func(obj);
            }
        }
    }

    private class InvalidTypeException : Exception;

    private readonly List<IHandler> _handlers = [];

    public Event(string key)
    {
        Key = key;
    }

    public void Emit(object? obj = null)
    {
        foreach (var handler in _handlers)
        {
            handler.Call(obj);
        }
    }

    public ISubscription Subscribe<T>(AAppService.Handler<T> handler)
    {
        var h = new Handler<T>(handler);
        _handlers.Add(h);
        return new Subscription(() => _handlers.Remove(h));
    }

    public ISubscription Subscribe(AAppService.Handler handler)
    {
        var h = new HandlerNoParam(handler);
        _handlers.Add(h);
        return new Subscription(() => _handlers.Remove(h));
    }
}