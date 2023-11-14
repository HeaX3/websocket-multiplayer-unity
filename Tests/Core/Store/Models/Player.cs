using System;
using MultiplayerProtocol;

namespace WebsocketMultiplayer.Tests.Models
{
    public class Player : ISerializableValue
    {
        public Guid id { get; private set; }
        public string name { get; set; }
        public Guid userId { get; set; }
        public Guid areaId { get; set; }

        public Player()
        {
        }

        public Player(Guid id)
        {
            this.id = id;
        }

        public void SerializeInto(SerializedData message)
        {
            message.Write(id);
            message.Write(name);
            message.Write(userId);
            message.Write(areaId);
        }

        public void DeserializeFrom(SerializedData message)
        {
            id = message.ReadGuid();
            name = message.ReadString();
            userId = message.ReadGuid();
            areaId = message.ReadGuid();
        }

        public override string ToString()
        {
            return $"{GetType().Name}({nameof(id)}: {id}, {nameof(name)}: {name})";
        }
    }
}