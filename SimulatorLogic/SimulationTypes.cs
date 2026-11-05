namespace SimulatorLogic;

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
