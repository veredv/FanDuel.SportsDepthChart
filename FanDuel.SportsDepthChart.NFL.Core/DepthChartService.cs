using FanDuel.SportsDepthChart.NFL.Domain.Entities;
using FanDuel.SportsDepthChart.NFL.Domain.Models;

namespace FanDuel.SportsDepthChart.NFL.Core;

public class DepthChartService(DepthChart depthChart)
{
    private readonly DepthChart depthChart = depthChart;

    public void AddPlayerToDepthChart(NflPosition position, Player player, int? positionDepth = null) =>
        depthChart.AddPlayer(position, player, positionDepth);

    // Remove player from depth chart
    public Player? RemovePlayerFromDepthChart(NflPosition position, Player player) => depthChart.RemovePlayer(position, player);

    // Get backups for a player at a given position
    public List<Player> GetBackups(NflPosition position, Player player) => 
        depthChart.GetBackups(position, player);

    public void GetFullDepthChart()
    {
        foreach (var entry in depthChart.AsDictionary())
        {
            Console.WriteLine(
                $"{entry.Key} - {string.Join(", ", entry.Value.Select(listing => $"(#{listing.Player.Number}, {listing.Player.Name})"))}");
        }
    }
}
