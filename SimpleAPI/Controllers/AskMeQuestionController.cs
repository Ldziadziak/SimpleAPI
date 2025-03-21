using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Interfaces;

namespace SimpleAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/askme")]
public class AskMeQuestionController : ControllerBase
{
  private readonly ILogger<AskMeQuestionController> _logger;
  private readonly IConfiguration _configuration;
  private readonly IAiChatService _aiChatService;
  public AskMeQuestionController(ILogger<AskMeQuestionController> logger, IConfiguration configuration, IAiChatService aiChatService)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    _aiChatService = aiChatService ?? throw new ArgumentNullException(nameof(aiChatService));
  }

  [HttpGet]
  [Route("question/{question}")]
  public async Task<ActionResult> ReciveAnswerAsync(string question)
  {
    var aiServiceName = _configuration.GetValue<string>("DefaultAIService");
    //verify appsetings
    var answer = await _aiChatService.RunAiChatDll(aiServiceName!, new object[] { question });

    if (answer == null)
    {
      return NotFound($"Service unavailable");
    }
    else
    {
      _logger.LogInformation("Retrieving answer");
      _logger.LogInformation(question);
      _logger.LogInformation(answer);
      return Ok(answer);
    }
  }
}