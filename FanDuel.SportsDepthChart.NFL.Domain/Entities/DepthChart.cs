using System.Collections.ObjectModel;
using System.Linq;

using FanDuel.SportsDepthChart.NFL.Domain.Models;

namespace FanDuel.SportsDepthChart.NFL.Domain.Entities;

public class DepthChart
{
    private readonly Dictionary<NflPosition, List<DepthChartListing>> depthChart = [];
    private readonly Dictionary<NflPosition, HashSet<int>> playerNumbersPerPosition = [];

    public virtual void AddPlayer(NflPosition position, Player player, int? depth = null)
    {
        List<DepthChartListing> depthChartEntry;
        HashSet<int> playerExistenceEntry;
        if (!depthChart.TryGetValue(position, out List<DepthChartListing>? value))
        {
            depthChartEntry = depthChart[position] = [];
            playerExistenceEntry = playerNumbersPerPosition[position] = [];
        } 
        else 
        {
            depthChartEntry = value;
            playerExistenceEntry = playerNumbersPerPosition[position];
        }
        
        if (playerExistenceEntry.Contains(player.Number))
        {
            throw new ArgumentException($"Player {player} already listed for position {position}.");
        }

        var playerListing = new DepthChartListing(player, position);
        if (depth is null)
        {
            depthChartEntry.Add(playerListing);
        }
        else
        {
            if (depth > depthChartEntry?.Count)
            {
                throw new ArgumentException($"Depth provided is {depth}, but should be between 0 and {depthChart.Count}");
            }

            depthChartEntry!.Insert(depth.Value, playerListing);
        }

        playerExistenceEntry.Add(player.Number);
    }

    // Remove player from depth chart
    public virtual Player? RemovePlayer(NflPosition position, Player player)
    {
        if (!depthChart.TryGetValue(position, out List<DepthChartListing>? depthChartEntry))
        {
            return null;
        }

        var listing = depthChartEntry.FirstOrDefault(l => l.Player.Number == player.Number);

        if (listing == null)
        {
            return null;
        }

        depthChartEntry.Remove(listing);

        return listing.Player;
    }

    // Get backups for a player at a given position
    public virtual List<Player> GetBackups(NflPosition position, Player player)
    {
        if (!depthChart.TryGetValue(position, out List<DepthChartListing>? depthChartEntry))
        {
            return [];
        }

        return depthChartEntry
        .SkipWhile(listing => listing.Player.Number != player.Number) // Skip until we find the player
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

    //// Return a copy of the list
    //private List<DepthChartListing> GetDepthChartListings(NflPosition position)
    //{
    //    return depthChart.TryGetValue(position, out List<DepthChartListing>? value) ? new List<DepthChartListing>(value) : [];
    //}

//// Get the full depth chart
//public string GetFullDepthChart()
//{
//    return string.Join(Environment.NewLine, _depthCharts
//        .Select(position => $"{position.Key} – " +
//            string.Join(", ", position.Value.Select(listing => listing.ToString()))));
//}

//// Safely return depth chart listings for a given position
//public List<DepthChartListing> GetDepthChartListings(NflPosition position)
//{
//    if (!_depthCharts.ContainsKey(position))
//    {
//        return new List<DepthChartListing>();
//    }

//    // Return a copy of the list to prevent external modification
//    return new List<DepthChartListing>(_depthCharts[position]);
//}

}
