## Invokables

### What are they?

Task schedulers written by [James MH](https://github.com/jamesmh/coravel)

Usage

```csharp
public class TestInvocable : IInvocable
{
  private ApplicationDbContext _context;
  private IDistributedLockProvider _distributedlock;

  public TestInvocable(ApplicationDbContext context, IDistributedLockProvider distributedlock)
  {
    this._context = context;
    this._distributedlock = distributedlock;
  }

  public async Task Invoke()
  {
    await using (await this._distributedlock.AcquireAsync())
    {
      await this._context.Test.AddAsync(new TestModel() { Name = "test name" });
      await this._context.SaveChangesAsync();
    }
  }
}
```
