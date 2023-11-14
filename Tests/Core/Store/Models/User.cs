using System;
using MultiplayerProtocol;

namespace WebsocketMultiplayer.Tests.Models
{
    public class User : ISerializableValue
    {
        public Guid id;
        public string name;
        public bool admin;
        public bool demo;
        public string language;

        public User()
        {
        }

        public User(Guid id)
        {
            this.id = id;
        }

        public virtual void SerializeInto(SerializedData message)
        {
            message.Write(id);
            message.Write(name);
            message.Write(admin);
            message.Write(demo);
            message.Write(language);
        }

        public virtual void DeserializeFrom(SerializedData message)
        {
            id = message.ReadGuid();
            name = message.ReadString();
            admin = message.ReadBool();
            demo = message.ReadBool();
            language = message.ReadString();
        }

        public override string ToString()
        {
            return $"{GetType().Name}({nameof(id)}: {id}, {nameof(name)}: {name})";
        }
    }
}