﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MontyHallProblemCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfTrials = 0;

            if (args.Length == 0)
            {
                PrintHelp();
                Console.WriteLine();
            }
            else
            {
                bool gotIt = int.TryParse(args[0], out numberOfTrials);

                if (!gotIt)
                {
                    PrintHelp();
                    Environment.Exit(1);
                }
            }

            RunSimulation(numberOfTrials);
        }

        /// <summary>
        /// Prints the simulator's usage to the default console.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine("usage: MontyHallSimulator <number of trials>");
            Console.WriteLine();
            Console.WriteLine("  number of trials: The number of trials the simulator will run.");
            Console.WriteLine("                    The largest value accepted is 2147483647 (max value of int32).");
            Console.WriteLine("                    The default value is 100000.");
        }

        private static void RunSimulation(int numberOfTrials)
        {
            if (numberOfTrials == 0)
                numberOfTrials = 100000;

            long playerWinsDueToSwitch = 0;
            long playerWinsStayingPat = 0;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Monty Hall Simulator v1.0");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();
            Console.WriteLine();
            Console.Write($"Starting simulation with {numberOfTrials:N0}...");

            DateTime startTime = DateTime.UtcNow;

            Random random = new Random();
            for (long counter = 0; counter < numberOfTrials; counter++)
            {
                // Create the 3 doors and initialize with goats
                List<Prizes> doors = new List<Prizes>() { Prizes.Goat, Prizes.Goat, Prizes.Goat };
                // Pick a door to put the car behind.
                int doorWithCar = random.Next(3);
                doors[doorWithCar] = Prizes.Car;

                // Have the player chose a door
                int playerDoorChoice = random.Next(3);

                int shownDoor;
                // The host shows a door which has a goat and is not the player's door.
                do
                {
                    shownDoor = random.Next(3);
                } while (doors[shownDoor] == Prizes.Car | shownDoor == playerDoorChoice);

                // if the car is behind the player's initial door then they win!
                playerWinsStayingPat += playerDoorChoice == doorWithCar ? 1 : 0;

                int newPlayerDoorChoice;
                // Now have player switch their choice
                // Make sure they don't chose the shown door or the door they already picked
                do
                {
                    newPlayerDoorChoice = random.Next(3);
                } while (newPlayerDoorChoice == playerDoorChoice | newPlayerDoorChoice == shownDoor);

                // if the car is behind the player's second door then they win!
                playerWinsDueToSwitch += newPlayerDoorChoice == doorWithCar ? 1 : 0;
            }


            DateTime endTime = DateTime.UtcNow;

            Console.WriteLine(" complete.");
            Console.WriteLine($"Simulation runtime: {(endTime - startTime).TotalSeconds}s");
            Console.WriteLine();
            Console.WriteLine("Simulation Results");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Player Stays with First Door Choice");
            Console.WriteLine($"  Wins: {playerWinsStayingPat,14:N0}; Ratio W/T: {(playerWinsStayingPat) / (decimal)numberOfTrials:P5}");
            Console.WriteLine("Player Switches Doors After Host Shows Goat Door");
            Console.WriteLine($"  Wins: {playerWinsDueToSwitch,14:N0}; Ratio W/T: {(playerWinsDueToSwitch) / (decimal)numberOfTrials:P5}");
            Console.WriteLine();
        }

        public enum Prizes
        {
            Car = 1,
            Goat = 0
        }
    }
}