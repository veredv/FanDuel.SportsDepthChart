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

    private const NflPosition Quarterback = NflPosition.QB;
    private const NflPosition RunningBack = NflPosition.RB;

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
        depthChart.AddPlayer(Quarterback, player1);
        depthChart.AddPlayer(Quarterback, player2, 0);

        var player1Backups = depthChart.GetBackups(Quarterback, player1);
        var player2Backups = depthChart.GetBackups(Quarterback, player2);

        player1Backups.Should().BeEmpty(); 
        player2Backups.Should().ContainSingle().Which.Should().Be(player1);
    }

    [Fact]
    public void AddPlayer_ShouldAddPlayerAtSpecifiedDepth_WhenPlayerPlayesInMultiplePositions()
    {
        depthChart.AddPlayer(Quarterback, player1, 0);
        depthChart.AddPlayer(Quarterback, player2, 1);
        depthChart.AddPlayer(Quarterback, player3, 2);

        depthChart.AddPlayer(RunningBack, player3, 0);
        depthChart.AddPlayer(RunningBack, player2, 1);

        var qBPlayer1backups = depthChart.GetBackups(Quarterback, player1);
        var qBPlayer2backups = depthChart.GetBackups(Quarterback, player2);
        var qBPlayer3backups = depthChart.GetBackups(Quarterback, player3);
        var rBPlayer1backups = depthChart.GetBackups(RunningBack, player1);
        var rBPlayer2backups = depthChart.GetBackups(RunningBack, player2);
        var rBPlayer3backups = depthChart.GetBackups(RunningBack, player3);

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
        depthChart.AddPlayer(Quarterback, player1);
        depthChart.AddPlayer(Quarterback, player2);
        depthChart.AddPlayer(Quarterback, player3);

        var qBPlayer1backups = depthChart.GetBackups(Quarterback, player1);
        var qBPlayer2backups = depthChart.GetBackups(Quarterback, player2);
        var qBPlayer3backups = depthChart.GetBackups(Quarterback, player3);

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

        act.Should().Throw<ArgumentException>()
            .And.Message.Should().Be($"Position value 600 is not defined in {nameof(NflPosition)}");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    public void AddPlayer_ShouldThrowException_WhenPlayerAddedInSamePosition(int initalDepth, int updatedDepth)
    {
        depthChart.AddPlayer(Quarterback, player1, initalDepth);

        Action act = () => depthChart.AddPlayer(Quarterback, player1, updatedDepth);

        act.Should().Throw<ArgumentException>()
            .And.Message.Should().Be($"Player {player1} already listed for position {Quarterback}.");
    }

    [Fact]
    public void AddPlayer_ShouldThrowException_WhenPlayerWithSameNumberExists()
    {
        Assert.True(false);
       //Todo
    }
}
