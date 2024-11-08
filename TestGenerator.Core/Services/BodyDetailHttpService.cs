using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestGenerator.Core.Exceptions;

namespace TestGenerator.Core.Services;

public class BodyDetailHttpService: HttpService
{
    protected override async Task<T> ProcessResponseBody<T>(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync();
        var body = JsonSerializer.Deserialize<ResponseBody<T>>(jsonString);
        // var body = await response.Content.ReadFromJsonAsync<ResponseBody<T>>();
        if (body == null)
            throw new UnprocessableResponseException();
        return body.Data;
    }
}

class ResponseBody<T>
{
    [JsonPropertyName("detail")] public required string Detail { get; init; }
    [JsonPropertyName("data")] public required T Data { get; init; }
}
