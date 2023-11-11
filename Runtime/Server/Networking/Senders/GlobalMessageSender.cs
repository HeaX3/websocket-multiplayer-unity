using System.Collections.Generic;
using MultiplayerProtocol;
using MultiplayerProtocol.Senders;

namespace WebsocketMultiplayer.Server.Senders
{
    public abstract class GlobalMessageSender : ScopedMessageSender
    {
        protected IMultiplayerServer server { get; }

        protected GlobalMessageSender(IMultiplayerServer server)
        {
            this.server = server;
        }

        public override IEnumerable<NetworkConnection> GetConnections()
        {
            return server.store.connection.GetConnections();
        }
    }
}