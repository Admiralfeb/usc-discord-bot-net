namespace UnitedSystemsCooperative.Bot.Models;

public class DatabaseItemBase
{
    public string Key { get; set; }
}

public class DatabaseItemArray : DatabaseItemBase
{
    public List<string> Value { get; set; }
}

public class DatabaseItem : DatabaseItemBase
{
    public string Value { get; set; }
}
