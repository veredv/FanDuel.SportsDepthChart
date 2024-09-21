using FanDuel.SportsDepthChart.NFL.Domain.Entities;
using FanDuel.SportsDepthChart.NFL.Domain.Models;

namespace FanDuel.SportsDepthChart.NFL.Core;

public class DepthChartService(DepthChart depthChart)
{
    private readonly DepthChart depthChart = depthChart ?? throw new ArgumentNullException(nameof(depthChart));

    /// <summary>
    /// Adds a specified player to the depth chart at the given position by delegating to the underlying depth chart.
    /// This method allows for the addition of a player at a specified depth; if no depth is provided, the player is added to the end of the list.
    /// </summary>
    /// <param name="position">The position to which the player is being added (e.g., quarterback, running back).</param>
    /// <param name="player">The player to add to the depth chart.</param>
    /// <param name="depth">The optional depth at which to insert the player in the depth chart; if null, the player is added to the end.</param>
    /// <exception cref="ArgumentException">Thrown if the player is already listed for that position or if the specified depth is invalid.</exception>
    public void AddPlayerToDepthChart(NflPosition position, Player player, int? positionDepth = null) =>
        depthChart.AddPlayer(position, player, positionDepth);

    /// <summary>
    /// Removes a specified player from the depth chart at the given position by delegating to the underlying depth chart method.
    /// </summary>
    /// <param name="position">The position from which the player should be removed (e.g., quarterback, running back).</param>
    /// <param name="player">The player to remove from the depth chart.</param>
    /// <returns>
    /// The player that was removed, or <c>null</c> if the player was not found in the depth chart at the specified position.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the position is not defined.</exception>
    public Player? RemovePlayerFromDepthChart(NflPosition position, Player player) => depthChart.RemovePlayer(position, player);

    /// <summary>
    /// Retrieves all backup players for a specified player at a given position in the depth chart.
    /// This method delegates the retrieval to the underlying depth chart implementation.
    /// A backup is defined as any player listed below the specified player at the same position.
    /// </summary>
    /// <param name="position">The position in the depth chart to search for backups (e.g., quarterback, running back).</param>
    /// <param name="player">The player for whom to find backups.</param>
    /// <returns>
    /// A list of players that are backups to the specified player at the given position, ordered by their depth in descending order.
    /// Returns an empty list if the position does not exist in the depth chart or if there are no backups available.
    /// </returns>
    public List<Player> GetBackups(NflPosition position, Player player) => 
        depthChart.GetBackups(position, player);

    /// <summary>
    /// Displays the full depth chart, printing each position and its associated players to the console.
    /// Each player is represented by their number and name in the format (#{PlayerNumber}, {PlayerName}).
    /// This method iterates through all entries in the depth chart and formats them for output.
    /// </summary>
    /// <remarks>
    /// The method does not return any value but outputs the depth chart to the console for visibility.
    /// It assumes that the depth chart has been populated with players and positions.
    /// </remarks>
    public void GetFullDepthChart()
    {
        foreach (var entry in depthChart.AsDictionary())
        {
            Console.WriteLine(
                $"{entry.Key} - {string.Join(", ", entry.Value.Select(listing => $"(#{listing.Player.Number}, {listing.Player.Name})"))}");
        }
    }
}
