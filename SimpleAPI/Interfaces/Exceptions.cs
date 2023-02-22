namespace SimpleAPI.Interfaces;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(int id, string message) : base(message)
    {
        Id = id;
    }
    public int Id { get; private set; }
}
