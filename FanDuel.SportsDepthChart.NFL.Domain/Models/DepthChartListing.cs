using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.NFL.Domain.Models;

public record DepthChartListing(Player Player, NflPosition Position, int Depth)
{
    public override string ToString() => $"{Position} - {Player} (Depth: {Depth})";
}
