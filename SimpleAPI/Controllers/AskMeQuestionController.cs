using ChatGPTLibrary;
using Microsoft.AspNetCore.Mvc;

namespace SimpleAPI.Controllers;

[ApiController]
[Route("api/askme")]
public class AskMeQuestionController : ControllerBase
{

    private readonly ILogger<AskMeQuestionController> _logger;
    private readonly IChatGPT _chatGPT;

    public AskMeQuestionController(ILogger<AskMeQuestionController> logger, IChatGPT chatGPT)
    {
        _logger = logger;
        _chatGPT = chatGPT;
    }

    [HttpGet]
    [Route("/question/{question}")]
    public async Task<ActionResult> ReciveAnswerAsync(string question)
    {

        var answer = await _chatGPT.AskMeQuestion(question);
        if (answer == null)
        {
            return NotFound($"Service unavailable");
        }
        else
        {
            _logger.LogInformation("Retrieving answer");
            return Ok(answer);
        }
    }
}