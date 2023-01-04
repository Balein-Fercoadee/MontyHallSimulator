using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MontyHallSimulator
{
    class Program
    {
        [ThreadStatic]
        private static Random __random;

        /// <summary>
        /// Gets an instance of <c>Random</c>.
        /// If this is the first call to the property for the thread, a new instance is created and returned. Otherwise, the thread's existing instance is returned.
        /// </summary>
        private static Random Random => __random ?? (__random = new Random((int)((1 + Thread.CurrentThread.ManagedThreadId) * DateTime.UtcNow.Ticks)));

        static void Main(string[] args)
        {
            int numberOfTrials = 100000;
            int numberOfThreads = 1;

            PrintHeader();

            ParseResults results = ArgumentProcessor.Parse(args, numberOfTrials, numberOfThreads);

            if (results.ValidArguments)
            {
                if (results.NumberOfArguments == 0)
                {
                    PrintHelp();
                    Console.WriteLine();
                }
                numberOfTrials = results.NumberOfTrials.Value;
                numberOfThreads = results.NumberOfThreads.Value;
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
            Console.WriteLine("                    The largest value accepted is 2147483647 (max value of int32).");
            Console.WriteLine("                    The default value is 100000.");
            Console.WriteLine("                    WARNING - Running the max, or near max, number of trials may result in an out-of-memory exception.");
            Console.WriteLine("number of threads - The number of threads the simulater will use.");
            Console.WriteLine("                    The largest value accepted is half the available number of cores, rounded down.");
            Console.WriteLine("                    The default value is 1.");
        }

        private static void PrintHeader()
        {
            string version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"Monty Hall Simulator v{version}");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();
        }

        private static void PrintTrailer(DateTime startTime, DateTime endTime, long playerWinsStayingPat, long playerWinsDueToSwitch, int numberOfTrials)
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

        private static void RunSimulation(int numberOfTrials, int numberOfTreads)
        {
            if (numberOfTrials == 0)
                numberOfTrials = 100000;

            long playerWinsSwitching = 0;
            long playerWinsStayingPat = 0;

            ConcurrentBag<int> playerStays = new ConcurrentBag<int>();
            ConcurrentBag<int> playerSwitches = new ConcurrentBag<int>();

            string pluralTrials = numberOfTrials > 1 ? "s" : string.Empty;
            string pluralThreads = numberOfTreads > 1 ? "s" : string.Empty;

            Console.WriteLine();
            Console.Write($"Starting simulation with {numberOfTrials:N0} trial{pluralTrials}, on {numberOfTreads} thread{pluralThreads}... ");

            DateTime startTime = DateTime.UtcNow;

            Parallel.For(0, numberOfTrials, new ParallelOptions() { MaxDegreeOfParallelism = numberOfTreads }, i =>
            {
                // Create the 3 doors and initialize with goats
                List<Prizes> doors = new List<Prizes>() { Prizes.Goat, Prizes.Goat, Prizes.Goat };
                // Pick a door to put the car behind.
                int doorWithCar = Random.Next(3);
                doors[doorWithCar] = Prizes.Car;

                // Have the player chose a door
                int playerDoorChoice = Random.Next(3);

                int shownDoor;
                // The host shows a door which has a goat and is not the player's door.
                do
                {
                    shownDoor = Random.Next(3);
                } while (doors[shownDoor] == Prizes.Car | shownDoor == playerDoorChoice);

                // if the car is behind the player's initial door then they win!
                if (playerDoorChoice == doorWithCar)
                    playerStays.Add(1);

                int newPlayerDoorChoice;
                // Now have player switch their choice
                // Make sure they don't chose the shown door or the door they already picked
                do
                {
                    newPlayerDoorChoice = Random.Next(3);
                } while (newPlayerDoorChoice == playerDoorChoice | newPlayerDoorChoice == shownDoor);

                // if the car is behind the player's second door then they win!
                if (newPlayerDoorChoice == doorWithCar)
                    playerSwitches.Add(1);
            });


            DateTime endTime = DateTime.UtcNow;

            Console.WriteLine("complete.");

            Console.Write("Compiling results... ");
            playerWinsStayingPat = playerStays.Sum();
            playerWinsSwitching = playerSwitches.Sum();
            Console.WriteLine("complete.");

            PrintTrailer(startTime, endTime, playerWinsStayingPat, playerWinsSwitching, numberOfTrials);
        }

        public enum Prizes
        {
            Car = 1,
            Goat = 0
        }
    }
}
