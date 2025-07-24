using System.CommandLine;
using System.Reflection;
using SimulatorLogic;

namespace SimulatorConsole;

class Program
{
    static void Main(string[] args)
    {
        int numberOfTrials = Constants.DEFAULT_TRIAL_COUNT;
        int numberOfThreads = Constants.DEFAULT_THREAD_COUNT;

        PrintHeader();

        ParseResults results = ArgumentProcessor.Parse(args, numberOfTrials, numberOfThreads);

        if (results.ValidArguments)
        {
            if (results.NumberOfArguments == 0)
            {
                PrintHelp();
                Console.WriteLine();
            }
            numberOfTrials = results.NumberOfTrials != null ? results.NumberOfTrials.Value : Constants.DEFAULT_TRIAL_COUNT;
            numberOfThreads = results.NumberOfThreads!= null ? results.NumberOfThreads.Value : Constants.DEFAULT_THREAD_COUNT;
        }
        else
        {
            PrintError("Invalid arguments.");
            PrintHelp();
            Environment.Exit(1);
        }

        RunSimulation(numberOfTrials, numberOfThreads);
    }

    /// <summary>
    /// Prints the simulator's usage to the default console.
    /// </summary>
    private static void PrintHelp()
    {
        Console.WriteLine();
        Console.WriteLine("usage: MontyHallSimulator <number of trials> <number of threads>");
        Console.WriteLine();
        Console.WriteLine("number of trials -  The number of trials the simulator will run.");
        Console.WriteLine($"                    The largest value accepted is {Constants.MAX_TRIAL_COUNT}.");
        Console.WriteLine($"                    The default value is {Constants.DEFAULT_TRIAL_COUNT}.");
        Console.WriteLine("                    WARNING - Running the max, or near max, number of trials may result in an out-of-memory exception.");
        Console.WriteLine("number of threads - The number of threads the simulater will use.");
        Console.WriteLine("                    The largest value accepted is half the available number of cores, rounded down.");
        Console.WriteLine($"                    The default value is {Constants.DEFAULT_THREAD_COUNT}.");
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

    private static void PrintTrailer(DateTime startTime, DateTime endTime, long playerWinsStayingPat, long playerWinsDueToSwitch, long numberOfTrials)
    {
        Console.WriteLine();
        Console.WriteLine("Simulation Results");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Simulation runtime: {(endTime - startTime).TotalSeconds:F3}s");
        Console.WriteLine();
        Console.WriteLine("Stay with First Door Choice");
        Console.WriteLine($"  Wins: {playerWinsStayingPat,14:N0}; Ratio W/T: {(playerWinsStayingPat) / (decimal)numberOfTrials:P5}");
        Console.WriteLine("Switch Doors After Shown Goat Door");
        Console.WriteLine($"  Wins: {playerWinsDueToSwitch,14:N0}; Ratio W/T: {(playerWinsDueToSwitch) / (decimal)numberOfTrials:P5}");
        Console.WriteLine();
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR - {message}");
        Console.ResetColor();
    }

    private static void RunSimulation(int numberOfTrials, int numberOfThreads)
    {

        Console.WriteLine();
        Console.Write($"Starting simulation ... ");

        MHSimulator simulator = new MHSimulator();
        MHSimulationOutcome results = simulator.RunSimulation(numberOfTrials, numberOfThreads);

        Console.WriteLine("complete.");

        PrintTrailer(results.SimulationStartDateTime, results.SimulationEndDateTime, results.TotalWinsWithStay, results.TotalWinsWithSwitch, results.TotalGamesPlayed);
    }
}
