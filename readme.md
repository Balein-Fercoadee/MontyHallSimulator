# Monty Hall Simulator

A simple, console application that simulates the Monty Hall Problem.

Read more about the Monty Hall Problem on [Wikipedia](https://en.wikipedia.org/wiki/Monty_Hall_problem).

## Required Software

* .NET 6.0 SDK (Windows/MacOS/Linux)

### Windows/MacOS
.NET 6.0 SDK can be downloaded from [.NET @ Microsoft](https://dotnet.microsoft.com/en-us/download).

### Linux
.NET 6.0 SDK can be installed using APT:

```bash
sudo apt install dotnet-sdk-6.0
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

This will execute the simulator 100,000 trials on one thread.

To supply the number of trials:

`./MontyHallSimulator <numberOfTrials>`

`./MontyHallSimulator 1000000` will execute 1,000,000 trials on one thread.

To supply the number of trials and threads:

`./MontyHallSimulator <numberOfTrials> <numberOfThreads>`

`./MontyHallSimulator 10000000 6` will execute 10,000,000 trials on six threads.


## More documentation to come.
