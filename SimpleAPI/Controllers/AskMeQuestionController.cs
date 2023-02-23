using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Services;

namespace SimpleAPI.Controllers;

[ApiController]
[Route("api/askme")]
public class AskMeQuestionController : ControllerBase
{
    private readonly ILogger<AskMeQuestionController> _logger;
    private readonly ChatGptService _chatGptService;
    public AskMeQuestionController(ILogger<AskMeQuestionController> logger, ChatGptService chatGptService)
    {
        _logger = logger;
        _chatGptService = chatGptService;
    }

    [HttpGet]
    [Route("/question/{question}")]
    public async Task<ActionResult> ReciveAnswerAsync(string question)
    {
        var answer = await _chatGptService.AskMeQuestionAsync(question);

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