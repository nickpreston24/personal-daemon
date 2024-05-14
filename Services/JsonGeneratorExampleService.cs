namespace personal_daemon;

public class JsonGeneratorExampleService : IJsonGeneratorExampleService
{
    /// <summary>
    /// src: https://www.youtube.com/watch?v=HhyBaJ7uisU
    /// </summary>
    public Person FasterJsonParsingUsingCodeGenerators()
    {
        var person = new Person()
        {
            Name = "Nick Preston",
            Age = 34
        };

        // var context = PersonJsonContext.Default.Person;
        // string text = JsonSerializer.Serialize(person, context);
        // Console.WriteLine(text);
        // Person obj = JsonSerializer.Deserialize(text, context);
        // obj.Dump("person json");

        return person;
    }
}

public interface IJsonGeneratorExampleService
{
    /// <summary>
    /// src: https://www.youtube.com/watch?v=HhyBaJ7uisU
    /// </summary>
    public Person FasterJsonParsingUsingCodeGenerators();
}