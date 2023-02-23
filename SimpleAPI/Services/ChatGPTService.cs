using System.Reflection;

namespace SimpleAPI.Services;

public class ChatGptService
{
    private readonly IConfiguration _configuration;
    public ChatGptService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<string> AskMeQuestionAsync(string question)
    {
        var libraryFullName = _configuration.GetValue<string>("ChatGPT:LibraryFullName");
        var dllName = _configuration.GetValue<string>("ChatGPT:DllName");
        var type = _configuration.GetValue<string>("ChatGPT:Type");
        var method = _configuration.GetValue<string>("ChatGPT:Method");
        var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        try
        {
#pragma warning disable S3885
            var chatGpt = Assembly.LoadFile(Path.Combine(rootDir!, dllName));
#pragma warning restore S3885

            if (chatGpt.FullName != libraryFullName)
            {
                return "The required library is not in the appropriate version: \n" +
                    $"Required: {libraryFullName} \n" +
                    $"Avalible: {chatGpt.FullName}";
            }

            var chatGptType = chatGpt.GetType(type)!;
            var chatGptInstance = Activator.CreateInstance(chatGptType);
            var askMeQuestionAsyncMethod = chatGptType.GetMethod(method)!;
            var resultTask = (Task<string>)askMeQuestionAsyncMethod.Invoke(chatGptInstance, new object[] { question })!;

            return await resultTask;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
