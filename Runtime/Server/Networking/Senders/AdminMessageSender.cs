using System.Collections.Generic;
using System.Linq;
using MultiplayerProtocol;

namespace WebsocketMultiplayer.Server.Senders
{
    public class AdminMessageSender : IScopedMessageSender
    {
        private IMultiplayerServer server { get; }

        public AdminMessageSender(IMultiplayerServer server)
        {
            this.server = server;
        }

        protected void SendToAll(INetworkMessage message) => ((IScopedMessageSender)this).Send(message);

        public IEnumerable<NetworkConnection> GetConnections()
        {
            return server.store.connection.GetConnections().OfType<ClientConnection>().Where(c => c.isAdmin);
        }
    }
}