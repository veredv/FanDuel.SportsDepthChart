namespace FanDuel.SportsDepthChart.NFL.Domain.Models;

public record Player(int Number, string Name)
{
    public override string ToString() => $"#{Number} - {Name}";
}
