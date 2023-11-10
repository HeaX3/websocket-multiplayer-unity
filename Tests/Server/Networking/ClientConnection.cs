using System;
using System.Collections.Generic;
using MultiplayerProtocol;
using WebsocketMultiplayer.Server;

namespace WebsocketMultiplayer.Tests.Server
{
    public class ClientConnection : NetworkConnection, IClientConnection
    {
        public Guid userId { get; set; }
        private IMultiplayerServer server { get; }

        protected override IEnumerable<INetworkMessageListener> handlers => ((IClientConnection)this).GetMultiplayerListeners(server);

        public ClientConnection(IMultiplayerServer server, INetworkEndpoint endpoint) : base(endpoint)
        {
            this.server = server;
        }
    }
}