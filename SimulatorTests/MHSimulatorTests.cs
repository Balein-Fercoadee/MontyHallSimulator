using SimulatorLogic;

namespace SimulatorTests;

[TestClass]
public class MHSimulatorTests
{
    [TestMethod]
    public void MHSimulationOutcomeIntialized()
    {
        MHSimulationOutcome outcome = new MHSimulationOutcome();

        Assert.AreEqual(0, outcome.TotalGamesPlayed);
        Assert.AreEqual(0, outcome.TotalThreadsUsed);
        Assert.AreEqual(0, outcome.TotalWinsWithStay);
        Assert.AreEqual(0, outcome.TotalWinsWithSwitch);
        Assert.AreEqual(0, outcome.WinRatioWithStay);
        Assert.AreEqual(0, outcome.WinRatioWithSwitch);
        Assert.AreEqual(0, outcome.SimulationDurationInSeconds);

        // Set games played to check the actual percentage calculation.
        outcome.TotalGamesPlayed = 1;
        Assert.AreEqual(0, outcome.WinRatioWithStay);
        Assert.AreEqual(0, outcome.WinRatioWithSwitch);

        outcome.SimulationStartDateTime = DateTime.Now;
        outcome.SimulationEndDateTime = outcome.SimulationStartDateTime.AddHours(1);
        Assert.AreEqual(3600, outcome.SimulationDurationInSeconds);
    }

    /// <summary>
    /// If the simulator is passed zeros for both parameters, it should use the default values.
    /// </summary>
    [TestMethod]
    public void MHSimulatorPassedZeros()
    {
        MHSimulator sim = new MHSimulator();
        var outcome = sim.RunSimulation(0, 0);

        Assert.AreEqual(Constants.DEFAULT_TRIAL_COUNT, outcome.TotalGamesPlayed);
        Assert.AreEqual(Constants.DEFAULT_THREAD_COUNT, outcome.TotalThreadsUsed);
        Assert.IsTrue(outcome.SimulationDurationInSeconds > 0);
    }

    /// <summary>
    /// If the simulator is passed ones for both parameters, it should only use 1 thread and execute 1 game.
    /// </summary>
    [TestMethod]
    public void MHSimulatorPassedOnes()
    {
        MHSimulator sim = new MHSimulator();
        var outcome = sim.RunSimulation(1, 1);

        Assert.AreEqual(1, outcome.TotalGamesPlayed);
        Assert.AreEqual(1, outcome.TotalThreadsUsed);
        Assert.IsTrue(outcome.SimulationDurationInSeconds > 0);
    }

    [TestMethod]
    public void MHSimulatorPassedMax()
    {
        MHSimulator sim = new MHSimulator();
        var outcome = sim.RunSimulation(Constants.MAX_TRIAL_COUNT,10);

        Assert.AreEqual(Constants.MAX_TRIAL_COUNT, outcome.TotalGamesPlayed);
        Assert.AreEqual(10, outcome.TotalThreadsUsed);
        Assert.AreEqual(0.333, outcome.WinRatioWithStay, 2);
        Assert.AreEqual(0.663, outcome.WinRatioWithSwitch, 2);
    }

    [TestMethod]
    public void MHSimulatorUsingDefaults()
    {
        MHSimulator sim = new MHSimulator();
        var outcome = sim.RunSimulation();

        Assert.AreEqual(Constants.DEFAULT_TRIAL_COUNT, outcome.TotalGamesPlayed);
        Assert.AreEqual(Constants.DEFAULT_THREAD_COUNT, outcome.TotalThreadsUsed);
        Assert.AreEqual(0.33, outcome.WinRatioWithStay, 2);
        Assert.AreEqual(0.66, outcome.WinRatioWithSwitch, 2);
        Assert.IsTrue(outcome.SimulationDurationInSeconds > 0);
    }
}
