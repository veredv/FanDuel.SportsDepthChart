using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.NFL.Domain.Models;
public record Player(int Number, string Name)
{
    public override string ToString() => $"#{Number} - {Name}";
}
