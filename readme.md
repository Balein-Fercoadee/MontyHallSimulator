# Monty Hall Simulator

A simple, console application that simulates the Monty Hall Problem.

Read more about the Monty Hall Problem on [Wikipedia](https://en.wikipedia.org/wiki/Monty_Hall_problem).

A few years back, my college roommate told me about the Monty Hall Problem. I won't describe it here since Wikipedia does an excellent job. I was convinced I knew the answer. Afterall, I was a Physics major! Turns out, I didn't know squat!

Run the program, take a look at the code and be prepared to have your intuition blown apart.

## Required Software

* .NET 9.0 SDK (Windows/MacOS/Linux)

### Windows/MacOS
.NET 9.0 SDK can be downloaded from [.NET @ Microsoft](https://dotnet.microsoft.com/en-us/download).

### Linux
.NET 9.0 SDK can be installed using APT:

```bash
sudo apt install dotnet-sdk-9.0
```

## Building

### Using Visual Studio (Windows)

Open the solution in Visual Studio and select `Build -> Build Solution`.

### Using the command line interface (Windows/MacOS/Linux)

Run the following command within the solution directory:

```bash
dotnet build MontyHallSimulator.sln
```

Note: Using either build method, NuGet packages will be downloaded and installed for the project.

## Running

To use the default number of trials and threads:

```bash
./MontyHallSimulator
```

This will execute the simulator with the default 100,000 trials on one thread.

It will also print out the help for calling the simulator.

To supply the number of trials:

`./MontyHallSimulator <numberOfTrials>`

`./MontyHallSimulator 1000000` will execute 1,000,000 trials on one thread.

To supply the number of trials and threads:

`./MontyHallSimulator <numberOfTrials> <numberOfThreads>`

`./MontyHallSimulator 10000000 6` will execute 10,000,000 trials on six threads.
