using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MultiplayerProtocol;

namespace WebsocketMultiplayer
{
    public class AuthResultMessage : INetworkMessage
    {
        public StringValue jwt { get; } = new();
        public GuidValue userId { get; } = new();
        public ByteArrayValue extra { get; } = new();

        public AuthResultMessage()
        {
        }

        public AuthResultMessage(string jwt, Guid userId, [CanBeNull] ISerializableValue extra = null)
        {
            this.jwt.value = jwt;
            this.userId.value = userId;
            this.extra.value = extra?.Serialize().ToArray();
        }

        public IEnumerable<ISerializableValue> values
        {
            get
            {
                yield return jwt;
                yield return userId;
                yield return extra;
            }
        }
    }
}