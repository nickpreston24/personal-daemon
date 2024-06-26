using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CodeMechanic.Todoist;
using CodeMechanic.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace personal_daemon;

public class TodoistSchedulerService : ITodoistSchedulerService
{
    private readonly string api_key = string.Empty;

    public TodoistSchedulerService()
    {
        api_key = Environment.GetEnvironmentVariable("TODOIST_API_KEY");
    }

    public async Task<List<TodoistTask>> Reschedule(params TodoistTask[] tasks)
    {
        var now = DateTime.UtcNow;
        var tomorrow = now.AddDays(1);
        // Console.WriteLine("old due date :>> " + first?.due?.date);
        string updated_date = tomorrow.ToString("yyyy-MM-dd");
        // string humanized_date = tomorrow.Humanize();
        string friendly_date = tomorrow.ToFriendlyDateString();

        // Console.WriteLine("humanized date :>> " + humanized_date);
        Console.WriteLine("friendly_date :>> " + friendly_date);

        // first.due = new Due()
        // {
        //     date = updated_date,
        //     due_string = friendly_date
        // };

        // first.description = "let's get this party started!!!";
        // Console.WriteLine("updated date :>> " + first.due.date);

        var updated_tasks = await UpdateTodos(tasks.ToList())
            // new List<TodoistTask>()
            ;


        // .Dump(nameof(todos));
        // Console.WriteLine("content :>> " + content);
        return updated_tasks;
    }

    public async Task<List<TodoistTask>> SearchTodos(TodoistTaskSearch search)
    {
        // var ids = todos.SelectMany(t => t.id);
        // string joined_ids = string.Join(",", ids);
        //    string uri = !(todos.Length > 0)
        //        ? "https://api.todoist.com/rest/v2/tasks": 
        //        $"https://api.todoist.com/rest/v2/tasks?todos={joined_ids}&label={label}";
        //    Console.WriteLine("uri :>> " + uri);

        string joined_ids = string.Join(",", search.ids);
        string label = search.label;

        string uri = $"https://api.todoist.com/rest/v2/tasks?todos={joined_ids}&label={label}";

        var content = await GetContentAsync(uri, api_key, false);
        var todos = JsonConvert.DeserializeObject<List<TodoistTask>>(content);
        Console.WriteLine("total todos:>> " + todos.Count);
        return todos;
    }

    private async Task<string> GetContentAsync(string uri, string bearer_token, bool debug = false)
    {
        using HttpClient http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer_token);
        var response = await http.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        // if (debug)
        //     Console.WriteLine("content :>> " + content);
        return content;
    }

    public async Task<List<TodoistTask>> UpdateTodos(List<TodoistTask> todos)
    {
        var updated_todos = todos.Select(todo => new TodoistUpdates()
        {
            due_string = todo.due.due_string,
            description = todo.description
        });

        return await PerformUpdate(updated_todos.FirstOrDefault());
    }

    private async Task<List<TodoistTask>> PerformUpdate(TodoistUpdates todo)
    {
        bool debug = false;
        if (todo.id.IsEmpty())
            throw new ArgumentNullException(nameof(todo.id) + " must not be empty!");

        string json = JsonConvert.SerializeObject(todo);
        Console.WriteLine("raw json updates :>> " + json);

        string uri = "https://api.todoist.com/rest/v2/tasks/$task_id".Replace("$task_id", todo.id);
        Console.WriteLine("update uri :>> " + uri);

        using HttpClient http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", api_key);

        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(json, Encoding.UTF8);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await http.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        if (debug)
            Console.WriteLine("content :>> " + content);
        // response.Dump(nameof(response));
        return JsonConvert.DeserializeObject<List<TodoistTask>>(content);
    }

    sealed class ExcludeCalculatedResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = _ => ShouldSerialize(member);
            return property;
        }

        internal static bool ShouldSerialize(MemberInfo memberInfo)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo == null)
            {
                return false;
            }

            if (propertyInfo.SetMethod != null)
            {
                return true;
            }

            var getMethod = propertyInfo.GetMethod;
            return Attribute.GetCustomAttribute(getMethod, typeof(CompilerGeneratedAttribute)) != null;
        }
    }

    sealed class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.Writable).ToList();
        }
    }
}

public class TodoistTaskSearch
{
    public string[] ids { get; set; } = Array.Empty<string>();
    public string label { get; set; } = string.Empty;
}

public class TodoistUpdates
{
    public string id { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public string due_string { get; set; } = string.Empty;
}

public interface ITodoistSchedulerService
{
    Task<List<TodoistTask>> Reschedule(params TodoistTask[] todos);
    Task<List<TodoistTask>> UpdateTodos(List<TodoistTask> todos);
    Task<List<TodoistTask>> SearchTodos(TodoistTaskSearch search);
}


// var tomorrow = DateTime.Now.AddDays(1);
// string updated_date = tomorrow.ToString("yyyy-MM-dd");

// var updates = new Due()
// {
//     date = updated_date,
//     @string = "tomorrow",
//     due_string = "tomorrow"
// };
// var updates = new
// {
//     due_string = todo.due.due_string
// };


// var updates = todo.due;
// var updates = new TodoistTask()
// {
//     due = todo.due
// };

// string json = JsonConvert.SerializeObject(
//     updates,
//     Formatting.Indented,
//     new JsonSerializerSettings
//     {
//         ContractResolver = new WritablePropertiesOnlyResolver(),
//         // ContractResolver = new ExcludeCalculatedResolver(),
//         Converters = new List<JsonConverter>
//         {
//             new Newtonsoft.Json.Converters.StringEnumConverter()
//         },
//         NullValueHandling = true
//             ? NullValueHandling.Ignore
//             : NullValueHandling.Include,
//         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
//     }
// );


// json = @"{due :{ 'due_string': 'tomorrow', 'date':'2024-05-15'} }";

/** old
var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
// requestContent.Dump(nameof(requestContent));
requestContent.Headers.Dump("headers");
var response = await http.PostAsync(uri, requestContent);
**/

//  public async Task<List<TodoistTask>> Search()
// {
//     // var ids = tasks.SelectMany(t => t.id);
//     // string joined_ids = string.Join(",", ids);
//     // string[] labels = new string[] { "bump" };
//     // string joined_labels = string.Join(",", labels);
//     // string label = "bump";
//     
//     return new List<TodoistTask>();
// }