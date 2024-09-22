namespace FanDuel.SportsDepthChart.NFL.Domain.Models;

public record DepthChartListing(Player Player, NflPosition Position)
{
    public override string ToString() => $"{Position} - {Player}";
}
