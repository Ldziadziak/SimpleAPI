using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly AiChatService _aiChatService;
    public HomeController(ILogger<HomeController> logger, IConfiguration configuration, AiChatService aiChatService)
    {
        _logger = logger;
        _configuration = configuration;
        _aiChatService = aiChatService;
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<ActionResult> Home(string? answer, string? question)
    {
        var aiServiceName = _configuration.GetValue<string>("DefaultAIService");

        if (!string.IsNullOrEmpty(question))
        {
            answer = await _aiChatService.RunAiChatDll(aiServiceName!, new object[] { question });
            _logger.LogInformation("Retrieving answer");
            _logger.LogInformation(question);
            _logger.LogInformation(answer);
        }

        return View(new HomeViewModel { Answer = answer, Question = question });
    }
}
