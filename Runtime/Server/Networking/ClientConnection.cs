using System;
using System.Collections.Generic;
using MultiplayerProtocol;

namespace WebsocketMultiplayer.Server
{
    public class ClientConnection : NetworkConnection, IClientConnection
    {
        private IServerNetworkEndpoint endpoint { get; }

        public string id => endpoint.id;
        public Guid userId { get; set; }

        protected override IEnumerable<INetworkMessageListener> handlers
        {
            get
            {
                yield return new AuthMessageHandler(endpoint.server, this);
            }
        }

        public ClientConnection(IServerNetworkEndpoint endpoint) : base(endpoint)
        {
            this.endpoint = endpoint;
        }
    }
}