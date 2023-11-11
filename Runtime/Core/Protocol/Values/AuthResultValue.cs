using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MultiplayerProtocol;

namespace WebsocketMultiplayer
{
    public class AuthResultValue : INetworkMessage
    {
        public StringValue jwt { get; } = new();
        public GuidValue userId { get; } = new();

        public AuthResultValue()
        {
        }

        public AuthResultValue(string jwt, Guid userId)
        {
            this.jwt.value = jwt;
            this.userId.value = userId;
        }

        public IEnumerable<ISerializableValue> values
        {
            get
            {
                yield return jwt;
                yield return userId;
            }
        }
    }
}