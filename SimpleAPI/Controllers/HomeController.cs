using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Interfaces;
using SimpleAPI.Models;

namespace SimpleAPI.Controllers;
public class HomeController(ILogger<HomeController> logger, IConfiguration configuration, IAiChatService aiChatService) : Controller
{
  private readonly ILogger<HomeController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
  private readonly IAiChatService _aiChatService = aiChatService ?? throw new ArgumentNullException(nameof(aiChatService));

  [AcceptVerbs("GET", "POST")]
  public async Task<ActionResult> Home(string? answer, string? question)
  {
    var aiServiceName = _configuration.GetValue<string>("DefaultAIService");

    if (!string.IsNullOrEmpty(question))
    {
      answer = await _aiChatService.RunAiChatDll(aiServiceName!, [question]);
      _logger.LogInformation("Retrieving answer");
      _logger.LogInformation(question);
      _logger.LogInformation(answer);
    }

    return View(new HomeViewModel { Answer = answer, Question = question });
  }
}