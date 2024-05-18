using System.Diagnostics;

namespace CodeMechanic.Diagnostics;

/// <summary>
/// This makes timing a method as easy as:
/// double time = new Stopwatch().Time(MyMethod).TimeInFractionalSeconds();
/// or
/// 
/// double time = new Stopwatch().Time(()=> Math.Sqrt(int.MaxValue)).TimeInFractionalSeconds();
/// </summary>
public static class StopwatchExtensions
{
    /// <summary>
    /// Extends the Stopwatch class with a method to time an Action delegate over a specified number of iterations
    /// </summary>
    public static Stopwatch Time(this Stopwatch stopwatch, Action action, long numberOfIterations)
    {
        stopwatch.Reset();
        stopwatch.Start();

        for (int i = 0; i < numberOfIterations; i++)
        {
            action();
        }

        stopwatch.Stop();
        return stopwatch;
    }

    /// <summary>
    /// Extends the Stopwatch class with a method to time an Action delegate
    /// </summary>
    /// <param name=\"stopwatch\"></param>
    /// <param name=\"action\"></param>
    public static Stopwatch Time(this Stopwatch stopwatch, Action action)
    {
        return stopwatch.Time(action, 1);
    }

    public static double TimeInFractionalSeconds(this Stopwatch stopwatch)
    {
        // figure out how much of a second a Stopwatch tick represents
        double secondsPerTick = (double)1 / Stopwatch.Frequency;

        return stopwatch.ElapsedTicks * secondsPerTick;
    }
}