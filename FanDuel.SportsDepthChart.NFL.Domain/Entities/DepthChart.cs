using System.Collections.ObjectModel;
using System.Linq;

using FanDuel.SportsDepthChart.NFL.Domain.Models;

namespace FanDuel.SportsDepthChart.NFL.Domain.Entities;

public class DepthChart
{
    private readonly Dictionary<NflPosition, List<DepthChartListing>> depthCharts = [];

    public virtual void AddPlayer(NflPosition position, Player player, int? depth = null)
    {
        if (!_depthCharts.ContainsKey(position))
        {
            _depthCharts[position] = new List<DepthChartListing>();
        }

        var depthChart = _depthCharts[position];

        if (depth is null)
        {
            depthChart.Add(new DepthChartListing(player, position, depthChart.Count));
        }
        else
        {
            depthChart.Insert(depth.Value, new DepthChartListing(player, position, depth.Value));

        depthCharts[NflPosition.QB] = [new (player1, NflPosition.QB, 1), new (player2, NflPosition.QB, 2)];
    }

    public virtual Player? RemovePlayer(NflPosition position, Player player)
    {
        //Todo
        return null;
    }

    // Get backups for a player at a given position
    public virtual List<Player> GetBackups(NflPosition position, Player player)
    {
        if (!depthChart.ContainsKey(position))
        {
            return [];
        }

        var depthChartForPosition = depthChart[position];
        return depthChartForPosition
        .SkipWhile(listing => listing.Player != player) // Skip until we find the player
        .Skip(1) // Skip the player itself
        .Select(listing => listing.Player) // Select remaining players as backups
        .ToList();
    }


    public IReadOnlyDictionary<NflPosition, ReadOnlyCollection<DepthChartListing>> AsDictionary() => depthChart.ToDictionary(
        entry => entry.Key,
        entry => entry.Value.AsReadOnly());

    public int Count() => depthChart.Count;


    // Get the full depth chart
    override public string ToString()
    {
        var d = depthChart.AsReadOnly();
        return String.Join(Environment.NewLine, depthChart
            .Select(position => $"{position.Key} – " +
                String.Join(", ", position.Value.Select(listing => listing.ToString()))));
    }

    // Return a copy of the list
    private List<DepthChartListing> GetDepthChartListings(NflPosition position)
    {
        return depthChart.ContainsKey(position) ? new List<DepthChartListing>(depthChart[position]) : new List<DepthChartListing>();
    }

}
