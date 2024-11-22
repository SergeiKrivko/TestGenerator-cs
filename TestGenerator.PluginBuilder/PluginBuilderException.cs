namespace TestGenerator.PluginBuilder;

public class PluginBuilderException : Exception
{
    public PluginBuilderException() : base()
    {
    }

    public PluginBuilderException(string message) : base(message)
    {
    }

    public PluginBuilderException(string message, Exception inner) : base(message, inner)
    {
    }
}