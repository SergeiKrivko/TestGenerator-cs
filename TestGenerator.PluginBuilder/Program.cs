using System.Reflection;
using System.Text.Json;
using CommandLine;
using TestGenerator.PluginBuilder.Args;
using TestGenerator.PluginBuilder.Plugins;

var types = LoadVerbs();

await Parser.Default.ParseArguments(args, types)
    .WithParsedAsync(Run);


Type[] LoadVerbs() => Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();


async Task Run(object obj)
{
    switch (obj)
    {
        case PluginPublishOptions o:
            await new PluginPublisher().Publish(o);
            break;
        case PluginBuildOptions o:
            Builder.Build(o.Path ?? Directory.GetCurrentDirectory(), o.Output, o.Install);
            break;
    }
}