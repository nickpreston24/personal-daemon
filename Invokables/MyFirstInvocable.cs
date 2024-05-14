using Coravel.Invocable;

namespace personal_daemon;

public class MyFirstInvocable : IInvocable
{
    public Task Invoke()
    {
        Console.WriteLine("This is my first invocable! (updated@7:55)");
        return Task.CompletedTask;
    }
}