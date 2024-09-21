using System;
using System.Collections.Generic;
using Xunit;
using NSubstitute;
using FanDuel.SportsDepthChart.NFL.Domain.Entities;
using FanDuel.SportsDepthChart.NFL.Domain.Models;
using FanDuel.SportsDepthChart.NFL.Core;
using FluentAssertions;
using System.Numerics;
using System.IO;

namespace FanDuel.SportsDepthChart.Tests;

public class DepthChartServiceTests
{
    private const string PlayerName1 = "Player One";
    private const int PlayerNumber1 = 1;
    private const string PlayerName2 = "Player Two";
    private const int PlayerNumber2 = 2;

    private static readonly NflPosition Quarterback = NflPosition.QB;
    private static readonly NflPosition RunningBack = NflPosition.RB;
    private static readonly NflPosition Linebacker = NflPosition.LB;

    private readonly DepthChart depthChart;
    private readonly DepthChartService service;


    public DepthChartServiceTests()
    {
        depthChart = Substitute.For<DepthChart>();
        service = new DepthChartService(depthChart);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenDepthChartIsNull()
    {
        // Arrange
        DepthChart? nullDepthChart = null;

        // Act & Assert
        #pragma warning disable CS8604 // Suppressing possible null reference argument - this is what we're testing
        var exception = Assert.Throws<ArgumentNullException>(() => new DepthChartService(nullDepthChart));
        #pragma warning restore CS8604 // Possible null reference argument.
        exception.ParamName.Should().Be("depthChart");
    }

    [Theory]
    [InlineData(null)] // No position depth
    [InlineData(1)]    // Specific position depth
    public void AddPlayerToDepthChart_DelegatesToDepthChart_WithDepth(int? positionDepth)
    {
        // Arrange
        var player = new Player(PlayerNumber1, PlayerName1);

        // Act
        service.AddPlayerToDepthChart(Quarterback, player, positionDepth);

        // Assert
        depthChart.Received().AddPlayer(Quarterback, player, positionDepth);
    }

    [Fact]
    public void AddPlayerToDepthChart_DelegatesToDepthChart_WithoutDepth()
    {
        // Arrange
        var player = new Player(PlayerNumber1, PlayerName1);

        // Act
        service.AddPlayerToDepthChart(Quarterback, player);

        // Assert
        depthChart.Received().AddPlayer(Quarterback, player);
    }

    [Fact]
    public void RemovePlayerFromDepthChart_DelegatesToDepthChart_AndPropagatesValue()
    {
        // Arrange
        var player1 = new Player(PlayerNumber1, PlayerName1);
        depthChart.RemovePlayer(Quarterback, player1).Returns(player1);

        // Act
        var removedPlayer = service.RemovePlayerFromDepthChart(Quarterback, player1);

        // Assert
        removedPlayer.Should().Be(player1);
    }

    [Fact]
    public void RemovePlayerFromDepthChart_DelegatesToDepthChart_AndPropagatesNull()
    {
        // Arrange
        var player1 = new Player(PlayerNumber1, PlayerName1);
        depthChart.RemovePlayer(Quarterback, player1).Returns((Player?)null);

        // Act
        var removedPlayer = service.RemovePlayerFromDepthChart(Quarterback, player1);

        // Assert
        depthChart.Received().RemovePlayer(Quarterback, player1);
        removedPlayer.Should().BeNull();
    }

    [Fact]
    public void GetBackups_ReturnsBackups()
    {
        // Arrange
        var player1 = new Player(PlayerNumber1, PlayerName1);
        var player2 = new Player(PlayerNumber2, PlayerName2);
        var backups = new List<Player> { player2 };

        depthChart.GetBackups(Quarterback, player1).Returns(backups);

        // Act
        var result = service.GetBackups(Quarterback, player1);

        // Assert
        result.Should().ContainSingle().Which.Should().Be(player2);
    }

    [Fact]
    public void GetFullDepthChart_PrintsDepthChart()
    {
        // Arrange
        var player1 = new Player(PlayerNumber1, PlayerName1);
        var player2 = new Player(PlayerNumber2, PlayerName2);
        var service = new DepthChartService(new DepthChart());

        service.AddPlayerToDepthChart(Quarterback, player1);
        service.AddPlayerToDepthChart(Quarterback, player2);

        var expectedOutput = $"{Quarterback} - (#{player1.Number}, {player1.Name}), (#{player2.Number}, {player2.Name}){Environment.NewLine}";

        using var stringWriter = new StringWriter();
        var previousConsoleTextWriter = Console.Out;
        Console.SetOut(stringWriter);

        // Act
        service.GetFullDepthChart();

        // Assert
        var actualOutput = stringWriter.ToString();
        actualOutput.Should().Be(expectedOutput);

        //Cleanup
        Console.SetOut(previousConsoleTextWriter);

    }

    [Fact]
    public void GetFullDepthChart_AfterRemovals_PrintsDepthChart()
    {
        // Arrange
        string PlayerName3 = "Player Three";
        string PlayerName4 = "Player Four";
        string PlayerName5 = "Player Five";
        string PlayerName6 = "Player Six";
        int PlayerNumber3 = 3;
        int PlayerNumber4 = 4;
        int PlayerNumber5 = 5;
        int PlayerNumber6 = 6;
        var player1 = new Player(PlayerNumber1, PlayerName1);
        var player2 = new Player(PlayerNumber2, PlayerName2);
        var player3 = new Player(PlayerNumber3, PlayerName3);
        var player4 = new Player(PlayerNumber4, PlayerName4);
        var player5 = new Player(PlayerNumber5, PlayerName5);
        var player6 = new Player(PlayerNumber6, PlayerName6);

        var service = new DepthChartService(new DepthChart());

        service.AddPlayerToDepthChart(Quarterback, player1);
        service.AddPlayerToDepthChart(Quarterback, player2);
        service.AddPlayerToDepthChart(Quarterback, player3);
        service.RemovePlayerFromDepthChart(Quarterback, player2);
        service.AddPlayerToDepthChart(RunningBack, player3);
        service.AddPlayerToDepthChart(RunningBack, player4);
        service.AddPlayerToDepthChart(RunningBack, player6);
        service.AddPlayerToDepthChart(RunningBack, player5);
        service.AddPlayerToDepthChart(Linebacker, player2);
        service.RemovePlayerFromDepthChart(Linebacker, player2);
        service.RemovePlayerFromDepthChart(RunningBack, player3);

        var expectedOutput = $"{Quarterback} - (#{player1.Number}, {player1.Name}), (#{player3.Number}, {player3.Name}){Environment.NewLine}" +
        $"{RunningBack} - (#{player4.Number}, {player4.Name}), (#{player6.Number}, {player6.Name}), (#{player5.Number}, {player5.Name}){Environment.NewLine}";

        using var stringWriter = new StringWriter();
        var previousConsoleTextWriter = Console.Out;
        Console.SetOut(stringWriter);

        // Act
        service.GetFullDepthChart();
       
        // Assert
        var actualOutput = stringWriter.ToString();
        actualOutput.Should().Be(expectedOutput);
        
        //Cleanup
        Console.SetOut(previousConsoleTextWriter);
    }
}
