using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;
using FanDuel.SportsDepthChart.NFL.Domain.Entities;
using FanDuel.SportsDepthChart.NFL.Domain.Models;
using System.Numerics;
using FluentAssertions;

namespace FanDuel.SportsDepthChart.Tests;


public class DepthChartTests
{
    private const string PlayerName1 = "Player One";
    private const string PlayerName2 = "Player Two";
    private const string PlayerName3 = "Player Three";
    private const int PlayerNumber1 = 1;
    private const int PlayerNumber2 = 2;
    private const int PlayerNumber3 = 3;

    private static readonly NflPosition PositionQB = NflPosition.QB;
    private static readonly NflPosition PositionRB = NflPosition.RB;

    private readonly DepthChart depthChart;

    private static readonly Player player1 = new (PlayerNumber1, PlayerName1);
    private static readonly Player player2 = new (PlayerNumber2, PlayerName2);
    private static readonly Player player3 = new (PlayerNumber3, PlayerName3);


    public DepthChartTests()
    {
        depthChart = new DepthChart();
    }

    [Fact]
    public void AddPlayer_ShouldAddPlayerAtSpecifiedDepth_WhenPositionDepthSpecified()
    {
        depthChart.AddPlayer(PositionQB, player1);
        depthChart.AddPlayer(PositionQB, player2, 0);

        var player1Backups = depthChart.GetBackups(PositionQB, player1);
        var player2Backups = depthChart.GetBackups(PositionQB, player2);

        player1Backups.Should().ContainSingle().Which.Should().Be(player1);
        player2Backups.Should().BeEmpty();
    }

    [Fact]
    public void AddPlayer_ShouldAddPlayerAtSpecifiedDepth_WhenPlayerPlayesInMultiplePositions()
    {
        depthChart.AddPlayer(PositionQB, player1, 0);
        depthChart.AddPlayer(PositionQB, player2, 1);
        depthChart.AddPlayer(PositionQB, player3, 2);

        depthChart.AddPlayer(PositionRB, player3, 0);
        depthChart.AddPlayer(PositionRB, player2, 1);

        var qBPlayer1backups = depthChart.GetBackups(PositionQB, player1);
        var qBPlayer2backups = depthChart.GetBackups(PositionQB, player2);
        var qBPlayer3backups = depthChart.GetBackups(PositionQB, player3);
        var rBPlayer1backups = depthChart.GetBackups(PositionQB, player1);
        var rBPlayer2backups = depthChart.GetBackups(PositionQB, player2);
        var rBPlayer3backups = depthChart.GetBackups(PositionQB, player3);

        depthChart.Count().Should().Be(2); //Sanity

        //Todo - Choose only one from 2 bellow
        qBPlayer1backups.Should().HaveCount(2).And.ContainInOrder([player2, player3]);
        qBPlayer1backups.Should().BeEquivalentTo([player2, player3]);
        qBPlayer2backups.Should().BeEquivalentTo([player3]);
        qBPlayer3backups.Should().BeEmpty();

        rBPlayer1backups.Should().BeEmpty();
        rBPlayer2backups.Should().BeEmpty();
        rBPlayer3backups.Should().BeEquivalentTo([player2]);
    }

    [Fact]
    public void AddPlayer_ShouldAddPlayerToEndOfDepthChart_WhenPositionDepthNotSpecified()
    {
        depthChart.AddPlayer(PositionQB, player1);
        depthChart.AddPlayer(PositionQB, player2);
        depthChart.AddPlayer(PositionQB, player3);

        var qBPlayer1backups = depthChart.GetBackups(PositionQB, player1);
        var qBPlayer2backups = depthChart.GetBackups(PositionQB, player2);
        var qBPlayer3backups = depthChart.GetBackups(PositionQB, player3);

        depthChart.Count().Should().Be(1); //Sanity

        //Todo - Choose only one from  3 bellow
        qBPlayer1backups.Should().HaveCount(2).And.ContainInOrder([player2, player3]);
        qBPlayer1backups.Should().BeEquivalentTo([player2, player3]);
        qBPlayer1backups.Should().BeEquivalentTo([player3, player2]);
        qBPlayer2backups.Should().BeEquivalentTo([player3]);
        qBPlayer3backups.Should().BeEmpty();
    }

    [Fact]
    public void AddPlayer_ShouldThrowException_WhenPositionIsUndefined()
    {
        Action act = () => depthChart.AddPlayer((NflPosition)600, player1);

        act.Should().Throw<ArgumentException>();
    }
}
