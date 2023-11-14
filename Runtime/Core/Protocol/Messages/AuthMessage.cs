using System;
using MultiplayerProtocol;

namespace WebsocketMultiplayer
{
    public class AuthMessage : INetworkMessage
    {
        public Guid userId { get; private set; }
        public string jwt { get; private set; }
        
        public AuthMessage(){}

        public AuthMessage(Guid userId, string secret)
        {
            this.userId = userId;
            jwt = secret;
        }

        public void SerializeInto(SerializedData message)
        {
            message.Write(userId);
            message.Write(jwt);
        }

        public void DeserializeFrom(SerializedData message)
        {
            userId = message.ReadGuid();
            jwt = message.ReadString();
        }
    }
}
