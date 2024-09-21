using System;
using System.Collections.Generic;
using Xunit;
using NSubstitute;
using FanDuel.SportsDepthChart.NFL.Domain.Entities;
using FanDuel.SportsDepthChart.NFL.Domain.Models;
using FanDuel.SportsDepthChart.NFL.Core;
using FluentAssertions;

namespace FanDuel.SportsDepthChart.Tests;

public class DepthChartServiceTests
{
    private const string PlayerName1 = "Player One";
    private const int PlayerNumber1 = 1;
    private const string PlayerName2 = "Player Two";
    private const int PlayerNumber2 = 2;

    private static readonly NflPosition Position = NflPosition.QB;

    private readonly DepthChart depthChart;
    private readonly DepthChartService service;


    public DepthChartServiceTests()
    {
        depthChart = Substitute.For<DepthChart>();
        service = new DepthChartService(depthChart);
    }

    [Theory]
    [InlineData(null)] // No position depth
    [InlineData(1)]    // Specific position depth
    public void AddPlayerToDepthChart_AddsPlayer_DelegatesToDepthChart(int? positionDepth)
    {
        // Arrange
        var player = new Player(PlayerNumber1, PlayerName1);

        // Act
        service.AddPlayerToDepthChart(Position, player, positionDepth);

        // Assert
        depthChart.Received().AddPlayer(Position, player, positionDepth);
    }

    [Fact]
    public void RemovePlayerFromDepthChart_RemovesPlayer_DelegatesToDepthChart()
    {
        // Arrange
        var player = new Player(PlayerNumber1, PlayerName1);
        depthChart.RemovePlayer(Position, player).Returns(player);

        // Act
        var removedPlayer = service.RemovePlayerFromDepthChart(Position, player);

        // Assert
        removedPlayer.Should().Be(player);
    }

    [Fact]
    public void GetBackups_ReturnsBackups()
    {
        // Arrange
        var player1 = new Player(PlayerNumber1, PlayerName1);
        var player2 = new Player(PlayerNumber1, PlayerName1);
        var backups = new List<Player> { player2 };

        depthChart.GetBackups(Position, player1).Returns(backups);

        // Act
        var result = service.GetBackups(Position, player1);

        // Assert
        result.Should().ContainSingle().Which.Should().Be(player2);
    }

    [Fact]
    public void GetFullDepthChart_PrintsDepthChart()
    {
        // Arrange
        var player1 = new Player(PlayerNumber1, PlayerName1);
        var player2 = new Player(PlayerNumber2, PlayerName2);
        var backups = new List<Player> { player2 };
        var service = new DepthChartService(new DepthChart());

        service.AddPlayerToDepthChart(Position, player1);
        service.AddPlayerToDepthChart(Position, player2);

        var expectedOutput = $"{Position} - (#{player1.Number}, {player1.Name}), (#{player2.Number}, {player2.Name}){Environment.NewLine}";

        using (var stringWriter = new StringWriter())
        {
            Console.SetOut(stringWriter);

            // Act
            service.GetFullDepthChart();

            // Assert
            var actualOutput = stringWriter.ToString();
            actualOutput.Should().Be(expectedOutput);
        }
    }
}
