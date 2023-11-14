using System;
using MultiplayerProtocol;

namespace WebsocketMultiplayer
{
    public class AuthResultValue : INetworkMessage
    {
        public string jwt { get; private set; }
        public Guid userId { get; private set; }

        public AuthResultValue()
        {
        }

        public AuthResultValue(string jwt, Guid userId)
        {
            this.jwt = jwt;
            this.userId = userId;
        }

        public void SerializeInto(SerializedData message)
        {
            message.Write(jwt);
            message.Write(userId);
        }

        public void DeserializeFrom(SerializedData message)
        {
            jwt = message.ReadString();
            userId = message.ReadGuid();
        }
    }
}