namespace ListItClient.Services;

using MongoDB.Bson;
using System.Text.Json.Serialization;
public class ItemApiDTO
{
[JsonPropertyName("id")]
public ObjectId Id { get; set; }
[JsonPropertyName("done")]
public bool Done { get; set; }
[JsonPropertyName("amount")]
public int? Amount { get; set; }
[JsonPropertyName("name")]
public string? Name { get; set; }
[JsonPropertyName("type")]
public string? Type { get; set; }
}