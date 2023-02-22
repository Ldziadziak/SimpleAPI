namespace ChatGPTLibrary;

#pragma warning disable S101 // Types should be named in PascalCase
public interface IChatGPT
#pragma warning restore S101 // Types should be named in PascalCase
{
    Task<string> AskMeQuestion(string question);
}
