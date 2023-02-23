using Newtonsoft.Json;
using System.Text;

namespace ChatGPTLibrary;

#pragma warning disable S101 // Types should be named in PascalCase
internal class ChatGPT : IChatGPT
#pragma warning restore S101 // Types should be named in PascalCase
{

    private static HttpClient Http = new HttpClient();
    public async Task<string> AskMeQuestionAsync(string question)
    {
        var apiKey = Environment.GetEnvironmentVariable("GPT_API_KEY");
        if (String.IsNullOrEmpty(apiKey))
        {
            return "provide GPT API key in environment variable GPT_API_KEY";
        }

        if (!Http.DefaultRequestHeaders.Contains("Authorization"))
        {
            Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        var jsonContent = new
        {
            prompt = String.IsNullOrEmpty(question) ? "What can you say about empty string ?" : question,
            model = "text-davinci-003",
            max_tokens = 1000
        };

#pragma warning disable S1075
        var uri = "https://api.openai.com/v1/completions";
#pragma warning restore S1075

        dynamic data;

        try
        {
            var responseContent = await Http.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));
            var resContext = await responseContent.Content.ReadAsStringAsync();
            data = JsonConvert.DeserializeObject<dynamic>(resContext)!;

            return data.choices[0].text;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}