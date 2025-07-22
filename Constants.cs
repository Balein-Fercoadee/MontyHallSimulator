using System;

namespace MontyHallSimulator;

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
    public const int DEFAULT_TRIAL_COUNT = 100000;

    /// <summary>
    /// The maximum number of trials that can be run in one simultation.
    /// </summary>
    public const int MAX_TRIAL_COUNT = Int32.MaxValue;
}
