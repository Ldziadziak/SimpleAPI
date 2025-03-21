using System.Runtime.Serialization;

namespace SimpleAPI.Interfaces
{
  [Serializable]
  public class EntityNotFoundException : Exception, ISerializable
  {
    public EntityNotFoundException(int id, string message)
        : base(message)
    {
      Id = id;
    }

    public EntityNotFoundException() { }

    public int Id { get; private set; }

    protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
#pragma warning disable SYSLIB0051
        : base(info, context)
#pragma warning restore SYSLIB0051
    {
      Id = info.GetInt32(nameof(Id));
    }
#pragma warning disable CS0672, SYSLIB0051
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue(nameof(Id), Id);
    }
#pragma warning restore CS0672, SYSLIB0051
  }
}