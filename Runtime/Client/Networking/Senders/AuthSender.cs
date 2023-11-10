using System;
using MultiplayerProtocol;
using MultiplayerProtocol.Senders;
using RSG;

namespace WebsocketMultiplayer.Client.Senders
{
    public class AuthSender : MessageSender
    {
        public AuthSender(NetworkConnection connection) : base(connection)
        {
        }

        public IPromise<AuthResultMessage> Authenticate(Guid userId, string secret, uint timeoutMs = 5000)
        {
            return connection.SendRequest<AuthMessage, AuthResultMessage>(new AuthMessage(userId, secret), timeoutMs);
        }
    }
}