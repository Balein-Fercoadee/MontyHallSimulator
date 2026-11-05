using SimulatorConsole;

namespace SimulatorConsoleTests;

[TestClass]
[DoNotParallelize]
public class ProgramTests
{
    [TestMethod]
    [DataRow("0")]
    [DataRow("-1")]
    [DataRow("9223372036854775807")]
    public void InvalidTrialCountIsRejected(string trialCount)
    {
        InvocationResult result = Invoke("--trials", trialCount);

        Assert.AreEqual(1, result.ExitCode);
        StringAssert.Contains(result.Output, "The number of trials must be between 1 and");
        Assert.IsFalse(result.Output.Contains("Running simulation"));
    }

    [TestMethod]
    [DataRow("0")]
    [DataRow("-1")]
    public void InvalidThreadCountIsRejected(string threadCount)
    {
        InvocationResult result = Invoke("--threads", threadCount);

        Assert.AreEqual(1, result.ExitCode);
        StringAssert.Contains(result.Output, "The number of threads must be between 1 and");
        Assert.IsFalse(result.Output.Contains("Running simulation"));
    }

    [TestMethod]
    public void ThreadCountAboveSupportedMaximumIsRejected()
    {
        int maxThreadCount = 3 * Environment.ProcessorCount / 4;
        InvocationResult result = Invoke("--threads", (maxThreadCount + 1).ToString());

        Assert.AreEqual(1, result.ExitCode);
        StringAssert.Contains(result.Output, $"The number of threads must be between 1 and {maxThreadCount}.");
        Assert.IsFalse(result.Output.Contains("Running simulation"));
    }

    [TestMethod]
    public void ValidOptionsRunRequestedSimulation()
    {
        InvocationResult result = Invoke("--trials", "1", "--threads", "1");

        Assert.AreEqual(0, result.ExitCode);
        StringAssert.Contains(result.Output, "Running simulation for              1 trials...");
        StringAssert.Contains(result.Output, "Simulation complete.");
        StringAssert.Contains(result.Output, "Total number of threads used:");
        StringAssert.Contains(result.Output, "Total number of game simulated:");
    }

    private static InvocationResult Invoke(params string[] args)
    {
        TextWriter originalOut = Console.Out;
        TextWriter originalError = Console.Error;
        using StringWriter output = new();

        Console.SetOut(output);
        Console.SetError(output);

        try
        {
            int exitCode = Program.Main(args);
            return new InvocationResult(exitCode, output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    private sealed record InvocationResult(int ExitCode, string Output);
}
