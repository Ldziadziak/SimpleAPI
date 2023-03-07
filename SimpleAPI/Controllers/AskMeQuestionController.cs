using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Services;

namespace SimpleAPI.Controllers;

[ApiController]
[Route("api/askme")]
public class AskMeQuestionController : ControllerBase
{
    private readonly ILogger<AskMeQuestionController> _logger;
    private readonly IConfiguration _configuration;
    private readonly AiChatService _aiChatService;
    public AskMeQuestionController(ILogger<AskMeQuestionController> logger, IConfiguration configuration, AiChatService aiChatService)
    {
        _logger = logger;
        _configuration = configuration;
        _aiChatService = aiChatService;
    }

    [HttpGet]
    [Route("/question/{question}")]
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