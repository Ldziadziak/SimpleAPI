namespace SimpleAPI.Interfaces;
public interface IAiChatService
{
  Task<string> RunAiChatDll(string service, object[] parameters);
}