using System.Reflection;
using System.Text.Json;
using CommandLine;
using Testenerator.PluginBuilder.Args;
using Testenerator.PluginBuilder.Plugins;

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
            if (o.Url != null)
            {
                var pluginConfig = JsonSerializer.Deserialize<PluginConfig>(
                    File.ReadAllText(Path.Join(o.Path ?? Directory.GetCurrentDirectory(), "Config.json")));
                if (pluginConfig == null)
                    throw new Exception("Invalid config");
                await PluginPublisher.PublishByUrl(pluginConfig, o.Url, o.Token);
            }

            break;
        case PluginBuildOptions o:
            Builder.Build(o.Path ?? Directory.GetCurrentDirectory(), o.Output, o.Install);
            break;
    }
}