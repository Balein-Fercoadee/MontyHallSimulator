namespace SimulatorLogic;

/// <summary>
/// Class that contains global constants.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The default number of thread to use during a simulation.
    /// </summary>
    public const int DEFAULT_THREAD_COUNT = 1;

    /// <summary>
    /// The default number of trials to run in one simulation.
    /// </summary>
    public const long DEFAULT_TRIAL_COUNT = 100000;

    /// <summary>
    /// The default number of trials to run in a given loop within the simulator.
    /// </summary>
    public const int DEFAULT_TRIALS_PER_LOOP = 1000000000;

    /// <summary>
    /// The maximum number of trials that can be run in one simultation.
    /// </summary>
    public const long MAX_TRIAL_COUNT = long.MaxValue - 1;
}
