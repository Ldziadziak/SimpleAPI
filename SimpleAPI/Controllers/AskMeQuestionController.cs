using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Interfaces;

namespace SimpleAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/askme")]
public class AskMeQuestionController(ILogger<AskMeQuestionController> logger, IConfiguration configuration, IAiChatService aiChatService) : ControllerBase
{
  private readonly ILogger<AskMeQuestionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
  private readonly IAiChatService _aiChatService = aiChatService ?? throw new ArgumentNullException(nameof(aiChatService));

  [HttpGet]
  [Route("question/{question}")]
  public async Task<ActionResult> ReciveAnswerAsync(string question)
  {
    var aiServiceName = _configuration.GetValue<string>("DefaultAIService");
    //verify appsetings
    var answer = await _aiChatService.RunAiChatDll(aiServiceName!, [question]);

    if (answer == null)
    {
      _logger.LogWarning("AI service returned null answer.");
      return NotFound("Service unavailable");
    }
    else
    {
      _logger.LogInformation("Retrieved answer successfully.");
      _logger.LogInformation("Question: {Question}", question);
      _logger.LogInformation("Answer: {Answer}", answer);
      return Ok(answer);
    }
  }
}