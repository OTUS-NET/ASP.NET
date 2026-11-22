using System.Text.Json;

namespace Pcf.Common.Contracts;

public static class JsonOptions
{
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web);
}
