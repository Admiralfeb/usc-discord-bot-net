using System.Text.Json.Serialization;

namespace UnitedSystemsCooperative.Bot.Modules.Models;

public class InaraResponse
{
    [JsonPropertyName("eventStatus")]
    public int EventStatus { get; set; }
    [JsonPropertyName("eventStatusText")]
    public string? EventStatusText { get; set; }
    [JsonPropertyName("eventData")]
    public InaraCmdr EventData { get; set; }
}
