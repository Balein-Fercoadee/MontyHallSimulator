using System.Collections.Concurrent;
using System.Reflection.Metadata;

namespace SimulatorLogic;

/// <summary>
/// Class that encompasses the logic for the Monty Hall Simulation.
/// </summary>
public class MHSimulator
{
    /// <summary>
    /// Event that periodically updates the caller of simulation progress.
    /// </summary>
    public event EventHandler<SimulationProgressEventArgs>? SimulationProgress;

    [ThreadStatic]
    private static Random? __random;

    /// <summary>
    /// Gets an instance of <c>Random</c>. This property will be unique per thread.
    /// If this is the first call to the property for the thread, a new instance is created and returned. Otherwise, the thread's existing instance is returned.
    /// </summary>
    private static Random Random => __random ??= new Random((int)((1 + Environment.CurrentManagedThreadId) * DateTime.UtcNow.Ticks));

    /// <summary>
    /// Runs Monty Hall game simulations with the application defaults.
    /// </summary>
    public MHSimulationOutcome RunSimulation()
    {
        return RunSimulation(Constants.DEFAULT_TRIAL_COUNT, Constants.DEFAULT_THREAD_COUNT);
    }

    /// <summary>
    /// Runs Monty Hall game simulations.
    /// </summary>
    /// <param name="numberOfGames">The number of Monty Hall games to run.</param>
    /// <param name="numberOfThreads">The number of threads to run the simulation on.</param>
    public MHSimulationOutcome RunSimulation(long numberOfGames, int numberOfThreads)
    {
        MHSimulationOutcome results = new MHSimulationOutcome();

        if (numberOfGames <= 0)
            numberOfGames = Constants.DEFAULT_TRIAL_COUNT;
        if (numberOfThreads <= 0)
            numberOfThreads = Constants.DEFAULT_THREAD_COUNT;

        if (numberOfGames > Constants.MAX_TRIAL_COUNT)
            numberOfGames = Constants.MAX_TRIAL_COUNT;
        if (numberOfThreads > (3 * Environment.ProcessorCount / 4))
            numberOfThreads = 3 * Environment.ProcessorCount / 4;

        long playerWinsSwitching = 0;
        long playerWinsStayingPat = 0;

        ConcurrentBag<int> playerStays = new ConcurrentBag<int>();
        ConcurrentBag<int> playerSwitches = new ConcurrentBag<int>();

        DateTime startTime = DateTime.UtcNow;
        long tempNumberOfGames = numberOfGames;
        long loopsCompleted = 0;

        do
        {
            long gamesToExecute = 0;

            if (tempNumberOfGames >= Constants.DEFAULT_TRIALS_PER_LOOP)
            {
                gamesToExecute = Constants.DEFAULT_TRIALS_PER_LOOP;
                tempNumberOfGames -= Constants.DEFAULT_TRIALS_PER_LOOP;
            }
            else
            {
                gamesToExecute = tempNumberOfGames;
                tempNumberOfGames -= tempNumberOfGames;
            }

            Parallel.For(0, gamesToExecute, new ParallelOptions() { MaxDegreeOfParallelism = numberOfThreads }, i =>
            {
                MHGameOutcome outcome = RunSingleGame(Random);

                if (outcome.WinWithStay)
                    playerStays.Add(1);

                if (outcome.WinWithSwitch)
                    playerSwitches.Add(1);
            });

            playerWinsStayingPat += playerStays.Sum();
            playerWinsSwitching += playerSwitches.Sum();

            loopsCompleted += 1;
            long gamesCompleted = 0;

            if (tempNumberOfGames > 0)
            {
                playerStays.Clear();
                playerSwitches.Clear();
                gamesCompleted = loopsCompleted * Constants.DEFAULT_TRIALS_PER_LOOP;
            }
            else
            {
                gamesCompleted = numberOfGames;
            }

            OnSimulationProgress(new SimulationProgressEventArgs(gamesCompleted, (DateTime.UtcNow - startTime).TotalSeconds));
        } while (tempNumberOfGames > 0);

        DateTime endTime = DateTime.UtcNow;

        results.TotalGamesPlayed = numberOfGames;
        results.TotalThreadsUsed = numberOfThreads;
        results.TotalWinsWithStay = playerWinsStayingPat;
        results.TotalWinsWithSwitch = playerWinsSwitching;
        results.SimulationEndDateTime = endTime;
        results.SimulationStartDateTime = startTime;

        return results;
    }

    private MHGameOutcome RunSingleGame(Random rng)
    {
        MHGameOutcome roundOutcome = new MHGameOutcome();

        // Create the 3 doors and initialize with goats
        List<MHPrizes> doors = new List<MHPrizes>() { MHPrizes.Goat, MHPrizes.Goat, MHPrizes.Goat };
        // Pick a door to put the car behind.
        int doorWithCar = rng.Next(3);
        doors[doorWithCar] = MHPrizes.Car;

        // Have the player chose a door
        int playerDoorChoice = rng.Next(3);

        int shownDoor;
        // The host shows a door which has a goat and is not the player's door.
        do
        {
            shownDoor = rng.Next(3);
        } while (doors[shownDoor] == MHPrizes.Car | shownDoor == playerDoorChoice);

        // if the car is behind the player's initial door then they win!
        if (playerDoorChoice == doorWithCar)
            roundOutcome.WinWithStay = true;

        int newPlayerDoorChoice;
        // Now have player switch their choice
        // Make sure they don't chose the shown door or the door they already picked
        do
        {
            newPlayerDoorChoice = rng.Next(3);
        } while (newPlayerDoorChoice == playerDoorChoice | newPlayerDoorChoice == shownDoor);

        // if the car is behind the player's second door then they win!
        if (newPlayerDoorChoice == doorWithCar)
            roundOutcome.WinWithSwitch = true;

        return roundOutcome;
    }

    /// <summary>
    /// Raises the SimulationProgress event.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnSimulationProgress(SimulationProgressEventArgs e)
    {
        SimulationProgress?.Invoke(this, e);
    }
}


/// <summary>
/// An enum for the prizes available in a Monty Hall game simulation.
/// </summary>
enum MHPrizes
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
/// Stores the results of a Monty Hall game.
/// <para>
/// The possible outcomes:
/// <list type="bullet">
/// <item><c>WinWithStay</c> is true.</item>
/// <item><c>WinWithSwitch</c> is true.</item>
/// <item><c>WinWithStay</c> and <c>WinWithSwitch</c> are both false.</item>
/// </list>
/// </para>
/// </summary>
struct MHGameOutcome
{
    /// <summary>
    /// Player won by staying pat.
    /// </summary>
    public bool WinWithStay;

    /// <summary>
    /// Player won by switching.
    /// </summary>
    public bool WinWithSwitch;

    /// <summary>
    /// Default constructor. Initializes fields to <c>false</c>.
    /// </summary>
    public MHGameOutcome()
    {
        WinWithStay = false;
        WinWithSwitch = false;
    }
}

/// <summary>
/// Stores the results of a Monty Hall game simulation.
/// </summary>
public struct MHSimulationOutcome
{
    /// <summary>
    /// The total number of Monty Hall games that were simulated.
    /// </summary>
    public long TotalGamesPlayed;

    /// <summary>
    /// The total number of threads to run the simulation.
    /// </summary>
    public long TotalThreadsUsed;

    /// <summary>
    /// The total number of Monty Hall games that were won using the 'Stay' strategy.
    /// </summary>
    public long TotalWinsWithStay;

    /// <summary>
    /// The total number of Monty Hall games that were won using the 'Switch' strategy.
    /// </summary>
    public long TotalWinsWithSwitch;

    /// <summary>
    /// The date and time that the Monty Hall simulation started.
    /// </summary>
    public DateTime SimulationStartDateTime;

    /// <summary>
    /// The date and time that the Monty Hall simulation ended.
    /// </summary>
    public DateTime SimulationEndDateTime;

    /// <summary>
    /// The total number of seconds that the Monty Hall simulation ran.
    /// </summary>
    public readonly double SimulationDurationInSeconds
    {
        get { return (SimulationEndDateTime - SimulationStartDateTime).TotalSeconds; }
    }

    /// <summary>
    /// The percentag of games won using the 'Stay' strategy.
    /// </summary>
    public readonly double WinRatioWithStay
    {
        get { return TotalGamesPlayed > 0 ? ((double)TotalWinsWithStay / TotalGamesPlayed) : 0; }
    }

    /// <summary>
    /// The percentag of games won using the 'Switch' strategy.
    /// </summary>
    public readonly double WinRatioWithSwitch
    {
        get { return TotalGamesPlayed > 0 ? ((double)TotalWinsWithSwitch / TotalGamesPlayed) : 0; }
    }
}

/// <summary>
/// Custom event agrs for the SimulationProgress event within MHSimulator.
/// </summary>
public class SimulationProgressEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the number of trials completed at the time the event was raised.
    /// </summary>
    public long CurrentNumberOfCompletedTrials { get; set; }

    /// <summary>
    /// Gets or sets the seconds that have passed since the simulation started.
    /// </summary>
    public double CurrentSimulationDurationInSeconds { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public SimulationProgressEventArgs()
    {

    }

    /// <summary>
    /// Constructor that allows the setting of the completed trials and current duration.
    /// </summary>
    /// <param name="currentNumberOfCompletedTrials"></param>
    /// <param name="currentSimulationDurationInSeconds"></param>
    public SimulationProgressEventArgs(long currentNumberOfCompletedTrials, double currentSimulationDurationInSeconds)
    {
        CurrentNumberOfCompletedTrials = currentNumberOfCompletedTrials;
        CurrentSimulationDurationInSeconds = currentSimulationDurationInSeconds;
    }
}
