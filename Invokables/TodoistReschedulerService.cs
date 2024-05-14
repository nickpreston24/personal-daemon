using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CodeMechanic.Diagnostics;
using CodeMechanic.Todoist;
using CodeMechanic.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace personal_daemon;

public class TodoistSchedulerService : ITodoistSchedulerService
{
    public async Task<bool> Reschedule(params string[] ids)
    {
        string joined_ids = string.Join(",", ids);
        string uri = !(ids.Length > 0)
            ? "https://api.todoist.com/rest/v2/tasks"
            : "https://api.todoist.com/rest/v2/tasks?ids=" + joined_ids;
        string api_key = Environment.GetEnvironmentVariable("TODOIST_API_KEY");

        var now = DateTime.UtcNow;
        var tomorrow = now.AddDays(1);

        // Console.WriteLine("api Key: >> " + api_key);
        Console.WriteLine("uri :>> " + uri);

        var content = await GetContentAsync(uri, api_key, false);
        var todos = JsonConvert.DeserializeObject<List<TodoistTask>>(content);
        var first = todos.FirstOrDefault(x => ids.Contains(x.id)).Dump("firstie!");

        Console.WriteLine("old due date :>> " + first?.due?.date);
        string updated_date = tomorrow.ToString("yyyy-MM-dd");

        string humanized_date = tomorrow.Humanize();
        string friendly_date = tomorrow.ToFriendlyDateString();

        // Console.WriteLine("humanized date :>> " + humanized_date);
        Console.WriteLine("friendly_date :>> " + friendly_date);
        first.due = new Due()
        {
            date = updated_date,
            due_string = friendly_date
        };

        // first.description = "let's get this party started!!!";
        // Console.WriteLine("updated date :>> " + first.due.date);

        var res = await UpdateTask(first, api_key);

        // .Dump(nameof(todos));
        // Console.WriteLine("content :>> " + content);
        return true;
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

    public async Task<List<TodoistTask>> UpdateTask(TodoistTask todo, string api_key)
    {
        bool debug = false;

        // var tomorrow = DateTime.Now.AddDays(1);
        // string updated_date = tomorrow.ToString("yyyy-MM-dd");

        // var updates = new Due()
        // {
        //     date = updated_date,
        //     @string = "tomorrow",
        //     due_string = "tomorrow"
        // };

        var updates = todo.due;

        string json = JsonConvert.SerializeObject(
            updates,
            Formatting.Indented,
            new JsonSerializerSettings
            {
                // ContractResolver = new WritablePropertiesOnlyResolver(),
                ContractResolver = new ExcludeCalculatedResolver(),
                Converters = new List<JsonConverter>
                {
                    new Newtonsoft.Json.Converters.StringEnumConverter()
                },
                NullValueHandling = true
                    ? NullValueHandling.Ignore
                    : NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        );

        string uri = "https://api.todoist.com/rest/v2/tasks/$task_id".Replace("$task_id", todo.id);

        using HttpClient http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", api_key);
        Console.WriteLine("update uri :>> " + uri);

        // json = @"{due :{ 'due_string': 'tomorrow', 'date':'2024-05-15'} }";
        Console.WriteLine("raw json updates :>> " + json);

        /** old
        var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
        // requestContent.Dump(nameof(requestContent));
        requestContent.Headers.Dump("headers");
        var response = await http.PostAsync(uri, requestContent);
        **/


        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(json, Encoding.UTF8);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await http.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        // if (debug)
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
}

public interface ITodoistSchedulerService
{
    Task<bool> Reschedule(params string[] ids);
}