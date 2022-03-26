#pragma warning disable CS8618
namespace UnitedSystemsCooperative.Bot.Models;

public class DatabaseItemBase
{
    public string Key { get; set; }
}

public class DatabaseItemArray : DatabaseItemBase
{
    public List<string> Value { get; init; }
}

public class DatabaseItemArray<T> : DatabaseItemBase
{
    public List<T> Value { get; init; }
}

public class DatabaseItem : DatabaseItemBase
{
    public string Value { get; set; }
}