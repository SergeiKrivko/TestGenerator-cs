using System.Text.Json;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

class Event : IEvent
{
    public string Key { get; }
    
    private interface IHandler
    {
        public void Call(object? data);
    }
    
    private class HandlerNoParam : IHandler
    {
        private readonly Action _func;

        public HandlerNoParam(Action func)
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
        private readonly Action<T> _func;

        public Handler(Action<T> func)
        {
            _func = func;
        }
        
        public void Call(object? data)
        {
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

    public ISubscription Subscribe<T>(Action<T> handler)
    {
        var h = new Handler<T>(handler);
        _handlers.Add(h);
        return new Subscription(() => _handlers.Remove(h));
    }

    public ISubscription Subscribe(Action handler)
    {
        var h = new HandlerNoParam(handler);
        _handlers.Add(h);
        return new Subscription(() => _handlers.Remove(h));
    }
}