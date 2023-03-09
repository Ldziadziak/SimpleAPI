using System.Runtime.Serialization;

namespace SimpleAPI.Interfaces;

[Serializable]
public class EntityNotFoundException : Exception, ISerializable
{
    public EntityNotFoundException(int id, string message) : base(message)
    {
        Id = id;
    }

    public int Id { get; private set; }

    protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Id = info.GetInt32(nameof(Id));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Id), Id);
    }
}