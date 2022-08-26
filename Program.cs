using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MontyHallProblemCoreConsole
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
            int numberOfTrials = 0;
            int numberOfTreads = 1;

            PrintHeader();

            if (args.Length == 0)
            {
                PrintHelp();
                Console.WriteLine();
            }
            else // User has supplied the number of trials.
            {
                bool gotIt = int.TryParse(args[0], out numberOfTrials);
                gotIt &= (numberOfTrials > 0) ? true : false;

                if (!gotIt)
                {
                    PrintError("Invalid number of trials.");
                    PrintHelp();
                    Environment.Exit(1);
                }
            }

            if (args.Length == 2)
            {
                bool gotIt = int.TryParse(args[1], out numberOfTreads);
                gotIt &= (numberOfTreads > 0) ? true : false;

                if (gotIt)
                {
                    numberOfTreads = Math.Min(Environment.ProcessorCount / 2, numberOfTreads);
                }
                else
                {
                    PrintError("Invalid number of threads.");
                    PrintHelp();
                    Environment.Exit(1);
                }
            }

            RunSimulation(numberOfTrials, numberOfTreads);
        }

        /// <summary>
        /// Prints the simulator's usage to the default console.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("usage: MontyHallSimulator <number of trials> <number of threads>");
            Console.WriteLine();
            Console.WriteLine("  number of trials  The number of trials the simulator will run.");
            Console.WriteLine("                    The largest value accepted is 2147483647 (max value of int32).");
            Console.WriteLine("                    The default value is 100000.");
            Console.WriteLine("  number of threads The number of threads the simulater will use.");
            Console.WriteLine("                    The largest value accepted is the half current number of cores, rounded down.");
            Console.WriteLine("                    The default value is 1.");
        }

        private static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Monty Hall Simulator v1.0");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();
        }

        private static void PrintTrailer(DateTime startTime, DateTime endTime, long playerWinsStayingPat, long playerWinsDueToSwitch, int numberOfTrials)
        {
            Console.WriteLine();
            Console.WriteLine("Simulation Results");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"Simulation runtime: {(endTime - startTime).TotalSeconds}s");
            Console.WriteLine();
            Console.WriteLine("Player Stays with First Door Choice");
            Console.WriteLine($"  Wins: {playerWinsStayingPat,14:N0}; Ratio W/T: {(playerWinsStayingPat) / (decimal)numberOfTrials:P5}");
            Console.WriteLine("Player Switches Doors After Host Shows Goat Door");
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

            long playerWinsDueToSwitch = 0;
            long playerWinsStayingPat = 0;

            ConcurrentBag<int> playerStays = new ConcurrentBag<int>();
            ConcurrentBag<int> playerSwitches = new ConcurrentBag<int>();

            string plural = numberOfTreads>1?"s" : string.Empty;

            Console.WriteLine();
            Console.Write($"Starting simulation with {numberOfTrials:N0} trials, on {numberOfTreads} thread{plural}... ");

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
                playerStays.Add(playerDoorChoice == doorWithCar ? 1 : 0);

                int newPlayerDoorChoice;
                // Now have player switch their choice
                // Make sure they don't chose the shown door or the door they already picked
                do
                {
                    newPlayerDoorChoice = Random.Next(3);
                } while (newPlayerDoorChoice == playerDoorChoice | newPlayerDoorChoice == shownDoor);

                // if the car is behind the player's second door then they win!
                playerSwitches.Add(newPlayerDoorChoice == doorWithCar ? 1 : 0);
            });


            DateTime endTime = DateTime.UtcNow;

            Console.WriteLine("complete.");

            Console.Write("Compiling results... ");
            playerWinsStayingPat = playerStays.Sum();
            playerWinsDueToSwitch = playerSwitches.Sum();
            Console.WriteLine("complete.");

            PrintTrailer(startTime, endTime, playerWinsStayingPat, playerWinsDueToSwitch, numberOfTrials);
        }

        public enum Prizes
        {
            Car = 1,
            Goat = 0
        }
    }
}
