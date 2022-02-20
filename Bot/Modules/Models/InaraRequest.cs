using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnitedSystemsCooperative.Bot.Modules.Models;

// Disable nullable warning.
#pragma warning disable CS8618


[Serializable]
public class InaraRequest
{
    [JsonPropertyName("header")]
    public InaraRequestHeader Header { get; set; }

    [JsonPropertyName("events")]
    public IEnumerable<InaraRequestEvent> Events { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}


public class InaraRequestHeader
{
    [JsonPropertyName("appName")]
    public string AppName { get; set; }
    [JsonPropertyName("appVersion")]
    public string AppVersion { get; set; }
    [JsonPropertyName("isBeingDeveloped")]
    public bool IsBeingDeveloped { get; set; }
    [JsonPropertyName("APIkey")]
    public string ApiKey { get; set; }
}

public class InaraRequestEvent
{

    [JsonPropertyName("eventName")]
    public string EventName { get; set; }
    [JsonPropertyName("eventTimestamp")]
    public DateTime EventTimestamp { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("eventData")]
    public InaraRequestEventData EventData { get; set; }
}

public class InaraRequestEventData
{
    [JsonPropertyName("searchName")]
    public string SearchName { get; set; }
}
