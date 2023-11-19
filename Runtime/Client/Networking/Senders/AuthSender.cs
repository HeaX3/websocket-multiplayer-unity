using System;
using MultiplayerProtocol;
using MultiplayerProtocol.Senders;

namespace WebsocketMultiplayer.Client.Senders
{
    public class AuthSender : MessageSender
    {
        public AuthSender(NetworkConnection connection) : base(connection)
        {
        }

        public RequestPromise<AuthResultValue> Authenticate(Guid userId, string secret, uint timeoutMs = 5000)
        {
            return connection.SendRequest<AuthResultValue>(new AuthMessage(userId, secret), timeoutMs);
        }
    }
}