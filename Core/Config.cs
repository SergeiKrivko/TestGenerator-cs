namespace Core;

public class Config
{
    public static string BaseUrl { get; } = Environment.GetEnvironmentVariable("APP_NAME_SUFFIX") == null
        ? "https://gptchat-api.nachert.art/"
        : "http://localhost:8000";
    public static string AppName { get; } =  $"TestGenerator{Environment.GetEnvironmentVariable("APP_NAME_SUFFIX")}";
    public static string Version = "{AppVersion}";
}