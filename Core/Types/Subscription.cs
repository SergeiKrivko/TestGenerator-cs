using TestGenerator.Shared.Types;

namespace Core.Types;

public class Subscription : ISubscription
{
    private Action _action;

    public Subscription(Action action)
    {
        _action = action;
    }

    public void Unsubscribe()
    {
        _action();
    }
}