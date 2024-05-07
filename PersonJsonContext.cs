using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(Person))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
public partial class PersonJsonContext : JsonSerializerContext
{
}