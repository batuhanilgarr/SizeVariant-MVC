namespace TireSearchMVC.Models;

public class DailyStatsModel
{
    public DateTime Date { get; set; }
    public int TotalCount { get; set; }
    public int BridgestoneCount { get; set; }
    public int LassaCount { get; set; }
    public List<string> NewProductCodes { get; set; } = new();
}

public class StatsComparisonModel
{
    public DailyStatsModel? PreviousDay { get; set; }
    public DailyStatsModel CurrentDay { get; set; } = new();
    public int NewProductsCount => CurrentDay.NewProductCodes.Count;
    public int TotalDifference => CurrentDay.TotalCount - (PreviousDay?.TotalCount ?? 0);
}
