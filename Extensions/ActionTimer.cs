using System.Diagnostics;

namespace CodeMechanic.Diagnostics;

/// <summary>
/// Times how long a given delegate takes to run
/// </summary>
public static class ActionTimer
{
    /// <summary>
    /// Times the given delegate
    /// </summary>
    ///
    /// <usage>
    /// This is how you might use it, using lambda syntax:
    ///
    ///    double time = ActionTimer.Time(
    ///        () => Enumerable.Range(1, 999)
    ///            .Where(x => x % 5 == 0  x % 3 == 0)
    ///            .Sum()
    ///    )
    /// </usage>
    /// <param name=\"action\">The delegate to time</param>
    /// <param name=\"targetRunTime\">The target amount of time to average over (in seconds)</param>
    /// <returns>The running time, in fractional seconds</returns>
    public static double AverageTime(this Action action, double targetRunTime)
    {
        if (targetRunTime <= 0)
        {
            throw new ArgumentException(@"targetRunTime must be greater than 0.", "targetRunTime");
        }

        Stopwatch timer = new Stopwatch();


        // time the action once to see how fast it is
        timer.Time(action);

        // if the action took more than half of the targetRunTime time, we\'re not going to
        // fit in a second iteration
        double firstIterationTime = timer.TimeInFractionalSeconds();
        if (firstIterationTime > targetRunTime / 2)
        {
            return firstIterationTime;
        }

        // otherwise, repeat the action to get an accurate timing
        // aim for targetRunTime seconds of total running time
        long numberOfIterations = (long)(targetRunTime / firstIterationTime);

        // the number of iterations should be at least 1 because firstIterationTime is less than half of
        // targetRunTime
        Debug.Assert(numberOfIterations > 0);

        timer.Time(action, numberOfIterations);

        // calculate the length of time per iteration
        return timer.TimeInFractionalSeconds() / numberOfIterations;
    }

    /// <summary>
    /// Times how long the given delegate takes to complete, averaging over up to 5 seconds of running time
    /// </summary>
    /// <param name=\"action\"></param>
    /// <returns></returns>
    public static double AverageTime(this Action action)
    {
        return AverageTime(action, 5);
    }
}