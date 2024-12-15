using TestGenerator.Shared;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

interface IEvent
{
    public void Emit(object? obj = null);
    public ISubscription Subscribe<T>(Action<T> handler);
    public ISubscription Subscribe(Action handler);
}