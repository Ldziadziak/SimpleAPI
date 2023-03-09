using SimpleAPI.Interfaces;
using System.Reflection;

namespace SimpleAPI.Services;

public class AiChatService : IAiChatService
{
    private readonly IConfiguration _configuration;
    public AiChatService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    public async Task<string> RunAiChatDll(string service, object[] parameters)
    {
        var libraryFullName = _configuration.GetValue<string>($"ChatAILibraries:{service}:LibraryFullName");
        var dllName = _configuration.GetValue<string>($"ChatAILibraries:{service}:DllName")!;
        var type = _configuration.GetValue<string>($"ChatAILibraries:{service}:Type")!;
        var method = _configuration.GetValue<string>($"ChatAILibraries:{service}:Method")!;
        var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        try
        {
#pragma warning disable S3885
            var assembly = Assembly.LoadFile(Path.Combine(rootDir, dllName));
#pragma warning restore S3885

            if (assembly.FullName != libraryFullName)
            {
                return "The required library is not in the appropriate version: \n" +
                    $"Required: {libraryFullName} \n" +
                    $"Avalible: {assembly.FullName}";
            }

            var assemblyType = assembly.GetType(type)!;
            var assemlbyInstance = Activator.CreateInstance(assemblyType);
            var AsyncMethod = assemblyType.GetMethod(method)!;
            var resultTask = (Task<string>)AsyncMethod.Invoke(assemlbyInstance, parameters)!;

            return await resultTask;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
