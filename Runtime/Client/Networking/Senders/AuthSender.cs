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

        public IPromise Authenticate(Guid userId, string secret, Action<AuthResultMessage> resultHandler,
            uint timeoutMs = 5000)
        {
            return connection.SendRequest(new AuthMessage(userId, secret), resultHandler, timeoutMs);
        }
    }
}