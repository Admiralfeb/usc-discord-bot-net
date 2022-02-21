using System.Text.Json.Serialization;

namespace UnitedSystemsCooperative.Bot.Models;

public class InaraResponse
{
    [JsonPropertyName("header")]
    public InaraResponseHeader Header { get; set; }
    [JsonPropertyName("events")]
    public IEnumerable<InaraResponseEvent> Events { get; set; }
}

public class InaraResponseHeader
{
    [JsonPropertyName("eventStatus")]
    public int EventStatus { get; set; }
}

public class InaraResponseEvent
{
    [JsonPropertyName("eventStatus")]
    public int EventStatus { get; set; }
    [JsonPropertyName("eventStatusText")]
    public string? EventStatusText { get; set; }
    [JsonPropertyName("eventData")]
    public InaraCmdr EventData { get; set; }
}
