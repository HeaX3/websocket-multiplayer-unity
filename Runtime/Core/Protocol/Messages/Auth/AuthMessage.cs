using System;
using System.Collections.Generic;
using MultiplayerProtocol;

namespace WebsocketMultiplayer
{
    public class AuthMessage : INetworkMessage
    {
        public GuidValue userId { get; } = new();
        public StringValue jwt { get; } = new();
        
        public AuthMessage(){}

        public AuthMessage(Guid userId, string secret)
        {
            this.userId.value = userId;
            this.jwt.value = secret;
        }

        public IEnumerable<ISerializableValue> values
        {
            get
            {
                yield return userId;
                yield return jwt;
            }
        }
    }
}
