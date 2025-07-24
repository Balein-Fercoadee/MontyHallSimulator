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
        Assert.AreEqual(0, outcome.TotalWinsWithStay);
        Assert.AreEqual(0, outcome.TotalWinsWithSwitch);
        Assert.AreEqual(0, outcome.PecentWinRateWithSwitch);
        Assert.AreEqual(0, outcome.PercentWinRateWithStay);
        Assert.AreEqual(0, outcome.DurationOfSimulationInSeconds);
    }
}
