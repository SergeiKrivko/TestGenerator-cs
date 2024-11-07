using TestGenerator.Shared;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

interface IEvent
{
    public void Emit(object? obj = null);
    public ISubscription Subscribe<T>(AAppService.Handler<T> handler);
    public ISubscription Subscribe(AAppService.Handler handler);
}