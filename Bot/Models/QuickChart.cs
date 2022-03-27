using System.Text;

namespace UnitedSystemsCooperative.Bot.Models;

public class QuickChart
{
    public QuickChart(IEnumerable<string> labels, IEnumerable<int> counts)
    {
        Data = new QuickChartData(labels, counts);
    }

    public string Type { get; set; } = "bar";
    public QuickChartData Data { get; set; }

    public override string ToString()
    {
        return Uri.EscapeDataString($"{{ type: '{Type}', data: {Data} }}");
    }

    public string GetChartUrl()
    {
        return $"https://quickchart.io/chart?c={ToString()}";
    }
}

public class QuickChartData
{
    public QuickChartData(IEnumerable<string> labels, IEnumerable<int> counts)
    {
        Labels = labels;
        DataSets = new List<QuickChartDataSet> {new(counts)};
    }

    public IEnumerable<string> Labels { get; init; }
    public IEnumerable<QuickChartDataSet> DataSets { get; init; }

    public override string ToString()
    {
        var labelsString = Labels.Aggregate(new StringBuilder(), (acc, val) => acc.Append($"'{val}',")).ToString();
        return $"{{ labels: [{labelsString}], datasets: [{DataSets.First()}] }}";
    }
}

public class QuickChartDataSet
{
    public QuickChartDataSet(IEnumerable<int> counts)
    {
        data = $"[{string.Join(',', counts)}]";
    }

    public string label { get; set; } = "votes";
    public string data { get; init; }

    public override string ToString()
    {
        return $"{{ label: '{label}', data: {data} }}";
    }
}