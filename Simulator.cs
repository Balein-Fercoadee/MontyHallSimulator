using System;
using System.Collections.Generic;

namespace MontyHallSimulator;

public class Simulator
{
    private Random _random;

    public Simulator()
    {
        _random = new Random((int)((1 + Environment.CurrentManagedThreadId) * DateTime.UtcNow.Ticks));
    }

    /// <summary>
    /// Runs a single Monty Hall simulation. It will run both 'Stay Pat' and 'Switch Door' strategies.
    /// </summary>
    /// <returns></returns>
    public SimulationOutcome RunSingleSimulation()
    {
        Random rng = _random;
        SimulationOutcome outcome = new SimulationOutcome();

        // Create the 3 doors and initialize with all goats.
        List<Prizes> doors = [Prizes.Goat, Prizes.Goat, Prizes.Goat];
        // Pick a door to put the car behind.
        int doorWithCar = rng.Next(3);
        doors[doorWithCar] = Prizes.Car;

        // Have the player chose a door
        int playerDoorChoice = rng.Next(3);

        // The host shows a door which has a goat and is not the player's picked door.
        int shownDoor;
        do
        {
            shownDoor = rng.Next(3);
        } while (doors[shownDoor] == Prizes.Car | shownDoor == playerDoorChoice);

        // if the car is behind the player's initial door then they win!
        if (playerDoorChoice == doorWithCar)
            outcome.WinWithStay = true;

        // Now have player switch their choice
        // Make sure they don't chose the shown door or the door they already picked
        int newPlayerDoorChoice;
        do
        {
            newPlayerDoorChoice = rng.Next(3);
        } while (newPlayerDoorChoice == playerDoorChoice | newPlayerDoorChoice == shownDoor);

        // if the car is behind the player's second door then they win!
        if (newPlayerDoorChoice == doorWithCar)
            outcome.WinWithSwitch = true;

        return outcome;
    }
}

/// <summary>
/// An enum for the prizes available in a Monty Hall simulation.
/// </summary>
public enum Prizes
{
    /// <summary>
    /// Car is an actual prize. Player is considered a winner if they win a car.
    /// </summary>
    Car = 1,

    /// <summary>
    /// Goat is a gag prize. Player is considered a loser if they win a goat.
    /// </summary>
    Goat = 0
}

/// <summary>
/// Stores the results of a Monty Hall simulation.
/// <para>
/// The possible outcomes:
/// <list type="bullet">
/// <item><c>WinWithStay</c> is true.</item>
/// <item><c>WinWithSwitch</c> is true.</item>
/// <item><c>WinWithStay</c> and <c>WinWithSwitch</c> are both false.</item>
/// </list>
/// </para>
/// </summary>
public struct SimulationOutcome
{
    /// <summary>
    /// Player won by staying pat.
    /// </summary>
    public bool WinWithStay;

    /// <summary>
    /// Player won by switching.
    /// </summary>
    public bool WinWithSwitch;

    public SimulationOutcome()
    {
        WinWithStay = false;
        WinWithSwitch = false;
    }
}
