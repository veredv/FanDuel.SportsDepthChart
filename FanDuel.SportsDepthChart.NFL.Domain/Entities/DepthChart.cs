using System.Collections.ObjectModel;
using System.Linq;

using FanDuel.SportsDepthChart.NFL.Domain.Models;

namespace FanDuel.SportsDepthChart.NFL.Domain.Entities;

public class DepthChart
{
    private readonly Dictionary<NflPosition, List<DepthChartListing>> depthChart = [];
    private readonly Dictionary<NflPosition, HashSet<int>> playerNumbersPerPosition = [];

    /// <summary>
    /// Adds a specified player to the depth chart at the given position.
    /// If the position does not exist, it initializes a new entry.
    /// The player can be added at a specified depth; if no depth is provided, the player is added to the end of the list.
    /// Throws an exception if the player is already listed for that position or if the specified depth is invalid.
    /// </summary>
    /// <param name="position">The position to which the player is being added (e.g., quarterback, running back).</param>
    /// <param name="player">The player to add to the depth chart.</param>
    /// <param name="depth">The optional depth at which to insert the player in the depth chart; if null, the player is added to the end.</param>
    /// <exception cref="ArgumentException">Thrown if the player is already listed for that position or if the specified depth is invalid.</exception>
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

    /// <summary>
    /// Removes a specified player from the depth chart at the given position.
    /// </summary>
    /// <param name="position">The position from which the player should be removed (e.g., quarterback, running back).</param>
    /// <param name="player">The player to remove from the depth chart.</param>
    /// <returns>
    /// The player that was removed, or <c>null</c> if the player was not found in the depth chart at the specified position.
    /// </returns>
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
        playerNumbersPerPosition[position].Remove(player.Number);

        if (depthChartEntry.Count == 0)
        {
            depthChart.Remove(position);
            playerNumbersPerPosition.Remove(position);
        }

        return listing.Player;
    }

    /// <summary>
    /// Retrieves all backup players for a given player at a specified position in the depth chart.
    /// A backup is defined as any player listed below the specified player at the same position.
    /// </summary>
    /// <param name="position">The position in the depth chart to search for backups (e.g., quarterback, running back).</param>
    /// <param name="player">The player for whom to find backups.</param>
    /// <returns>
    /// A list of players that are backups to the specified player at the given position, ordered by their depth in descending order.
    /// Returns an empty list if the position does not exist in the depth chart or if there are no backups available.
    /// </returns>
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

    /// <summary>
    /// This method is intended for sanity checks and testing purposes.
    /// </summary>
    public int Count() => depthChart.Count;

    override public string ToString()
    {
        return String.Join(Environment.NewLine, depthChart
            .Select(position => $"{position.Key} – " +
                String.Join(", ", position.Value.Select(listing => listing.ToString()))));
    }
}
