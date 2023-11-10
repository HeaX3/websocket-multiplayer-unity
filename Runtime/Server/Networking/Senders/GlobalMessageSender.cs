using System.Collections.Generic;
using MultiplayerProtocol;
using MultiplayerProtocol.Senders;

namespace WebsocketMultiplayer.Server.Senders
{
    public abstract class GlobalMessageSender : IScopedMessageSender
    {
        protected IMultiplayerServer server { get; }
        
        protected GlobalMessageSender(IMultiplayerServer server)
        {
            this.server = server;
        }

        public IEnumerable<NetworkConnection> GetConnections()
        {
            return server.store.connection.GetConnections();
        }
    }
}