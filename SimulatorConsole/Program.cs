using System.CommandLine;
using System.Reflection;
using SimulatorLogic;

namespace SimulatorConsole;

class Program
{
    static void Main(string[] args)
    {
        // Setup the command line parser.
        RootCommand rootCommand = new RootCommand("A simulator for the two main strategies in the Monty Hall problem.");

        Option<long> trialOption = new Option<long>("--trials")
        {
            Description = $"The number of trials (aka games) to execute in the simulation. The largest value accepted is {Constants.MAX_TRIAL_COUNT}.",
            DefaultValueFactory = parseResult => Constants.DEFAULT_TRIAL_COUNT,
            HelpName = "numberOfTrials"
        };
        Option<int> threadOption = new Option<int>("--threads")
        {
            Description = $"The number of threads the simulater will use. The largest value accepted is three-fourths the available number of cores, rounded down.",
            DefaultValueFactory = parseResult => Constants.DEFAULT_THREAD_COUNT,
            HelpName = "numberOfThreads"
        };

        rootCommand.Add(trialOption);
        rootCommand.Add(threadOption);

        long numberOfTrials;
        int numberOfThreads;

        rootCommand.SetAction(parseResult =>
        {
            numberOfThreads = parseResult.GetValue(threadOption);
            numberOfTrials = parseResult.GetValue(trialOption);
            RunSimulation(numberOfTrials, numberOfThreads);

            return 0;
        });

        ParseResult parseResult = rootCommand.Parse(args);

        Environment.Exit(parseResult.Invoke());
    }

    private static void PrintHeader()
    {
        string version = "v" + Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? string.Empty;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Monty Hall Simulator {version}");
        Console.WriteLine(new string('=', 50));
        Console.ResetColor();
    }

    private static void PrintTrailer(DateTime startTime, DateTime endTime, long playerWinsStayingPat, long playerWinsDueToSwitch, long numberOfTrials, long numberOfThreads)
    {
        Console.WriteLine();
        Console.WriteLine("Simulation Results");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Simulation runtime: {(endTime - startTime).TotalSeconds:F3} seconds");
        Console.WriteLine("Total number of threads used:");
        Console.WriteLine($"       {numberOfThreads,14:N0}");
        Console.WriteLine("Total number of game simulated:");
        Console.WriteLine($"        {numberOfTrials,14:N0}");
        Console.WriteLine("Stay with First Door Choice");
        Console.WriteLine($"  Wins: {playerWinsStayingPat,14:N0}; Ratio W/T: {(playerWinsStayingPat) / (decimal)numberOfTrials:P5}");
        Console.WriteLine("Switch Doors After Shown Goat Door");
        Console.WriteLine($"  Wins: {playerWinsDueToSwitch,14:N0}; Ratio W/T: {(playerWinsDueToSwitch) / (decimal)numberOfTrials:P5}");
        Console.WriteLine();
    }

    private static void RunSimulation(long numberOfTrials, int numberOfThreads)
    {

        Console.WriteLine();
        Console.WriteLine($"Running simulation for {numberOfTrials,14:N0} trials...");

        MHSimulator simulator = new MHSimulator();
        simulator.SimulationProgress += Simulator_SimulationProgress;
        MHSimulationOutcome results = simulator.RunSimulation(numberOfTrials, numberOfThreads);

        Console.WriteLine("Simulation complete.");

        PrintTrailer(results.SimulationStartDateTime, results.SimulationEndDateTime, results.TotalWinsWithStay, results.TotalWinsWithSwitch, results.TotalGamesPlayed, results.TotalThreadsUsed);
    }

    private static void Simulator_SimulationProgress(object? sender, SimulationProgressEventArgs e)
    {
        Console.WriteLine($"{e.CurrentNumberOfCompletedTrials,14:N0} completed...");
    }
}
