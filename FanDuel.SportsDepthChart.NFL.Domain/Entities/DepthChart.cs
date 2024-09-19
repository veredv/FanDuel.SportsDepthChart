using System.Collections.ObjectModel;
using System.Linq;

using FanDuel.SportsDepthChart.NFL.Domain.Models;

namespace FanDuel.SportsDepthChart.NFL.Domain.Entities;

public class DepthChart
{
    private readonly Dictionary<NflPosition, List<DepthChartListing>> depthCharts = [];

    public void AddPlayer(NflPosition position, Player player, int? depth = null)
    {
        //Todo
    }

    public Player? RemovePlayer(NflPosition position, Player player)
    {
        //Todo
        return null;
    }

    // Get backups for a player at a given position
    public List<Player> GetBackups(NflPosition position, Player player)
    {
        //Todo
        return [];
    }

    // Return a copy of the list
    private List<DepthChartListing> GetDepthChartListings(NflPosition position)
    {
        return depthCharts.ContainsKey(position) ? new List<DepthChartListing>(depthCharts[position]) : new List<DepthChartListing>();
    }

    public IReadOnlyDictionary<NflPosition, ReadOnlyCollection<DepthChartListing>> AsDictionary() => depthCharts.ToDictionary(
        entry => entry.Key,
        entry => entry.Value.AsReadOnly());


    // Get the full depth chart
    override public string ToString()
    {
        var d = depthCharts.AsReadOnly();
        return String.Join(Environment.NewLine, depthCharts
            .Select(position => $"{position.Key} – " +
                String.Join(", ", position.Value.Select(listing => listing.ToString()))));
    }
}
