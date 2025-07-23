namespace SimulatorConsole;

internal class ArgumentProcessor
{
    /// <summary>
    /// Takes command line parameters and converts them usable settings.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="defaultTrials"></param>
    /// <param name="defaultThreads"></param>
    /// <returns></returns>
    public static ParseResults Parse(string[] args, int defaultTrials, int defaultThreads)
    {
        int numberOfTrials = defaultTrials;
        int numberOfTreads = defaultThreads;

        ParseResults results = new ParseResults();

        bool gotIt = false;

        switch (args.Length)
        {
            case 0: // nothing was provided, use the default values
                results = new ParseResults(0, numberOfTrials, numberOfTreads);
                break;

            case 1: // one value was provided, assume it's number of trials
                gotIt = int.TryParse(args[0], out numberOfTrials);
                gotIt &= (numberOfTrials > 0);

                if (gotIt)
                {
                    results = new ParseResults(1, numberOfTrials, numberOfTreads);
                }

                break;

            case 2: // both number of trials and threads was provided
                gotIt = int.TryParse(args[0], out numberOfTrials);
                gotIt &= numberOfTrials > 0;

                gotIt &= int.TryParse(args[1], out numberOfTreads);
                gotIt &= numberOfTreads > 0;

                if (gotIt)
                {
                    // Simulations will use up to half of available cores.
                    numberOfTreads = Math.Min(Environment.ProcessorCount / 2, numberOfTreads);
                    results = new ParseResults(2, numberOfTrials, numberOfTreads);
                }

                break;

            default: // too many arguments sent
                results = new ParseResults();
                break;
        }

        return results;
    }
}

/// <summary>
/// Class that store the results of <c>ArgumentProcessor.Parse</c>.
/// </summary>
internal class ParseResults
{
    /// <summary>
    /// Gets whether the incoming arugments where successfully parsed.
    /// </summary>
    public bool ValidArguments
    {
        get
        {
            return NumberOfThreads.HasValue & NumberOfTrials.HasValue;
        }
    }

    public bool InvalidNumberOfTrials
    {
        get { return !NumberOfTrials.HasValue; }
    }

    public bool InvalidNumberOfThreads
    {
        get { return !NumberOfThreads.HasValue; }
    }

    public int? NumberOfTrials { get; private set; }

    public int? NumberOfThreads { get; private set; }

    public int NumberOfArguments { get; private set; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public ParseResults()
    {
    }

    /// <summary>
    /// Contructor that fully initializes <c>ParseResults</c>
    /// </summary>
    /// <param name="numberOfArguments">The number of arguements being stored.</param>
    /// <param name="numberOfTrials">The number of trials to run.</param>
    /// <param name="numberOfThreads">The number of threads that will run the trials.</param>
    public ParseResults(int numberOfArguments, int? numberOfTrials, int? numberOfThreads) : this()
    {
        NumberOfArguments = numberOfArguments;
        NumberOfTrials = numberOfTrials;
        NumberOfThreads = numberOfThreads;
    }
}
