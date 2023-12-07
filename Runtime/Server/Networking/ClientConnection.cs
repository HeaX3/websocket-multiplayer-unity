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
        public virtual bool isAdmin { get; set; }

        private bool debug { get; }

        protected override IEnumerable<INetworkMessageListener> handlers
        {
            get { yield return new AuthMessageHandler(endpoint.server, this, debug); }
        }

        public ClientConnection(IServerNetworkEndpoint endpoint, bool debug = false) : base(endpoint)
        {
            this.endpoint = endpoint;
            this.debug = debug;
        }
    }
}