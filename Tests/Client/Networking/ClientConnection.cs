using System.Collections.Generic;
using MultiplayerProtocol;
using WebsocketMultiplayer.Client;
using WebsocketMultiplayer.Client.Senders;

namespace WebsocketMultiplayer.Tests.Client
{
    public class ClientConnection : NetworkConnection, IMultiplayerConnection
    {
        public AuthSender auth { get; }
        
        public ClientConnection(INetworkEndpoint endpoint) : base(endpoint)
        {
            auth = new AuthSender(this);
        }

        protected override IEnumerable<INetworkMessageListener> handlers
        {
            get
            {
                yield break;
            }
        }
    }
}