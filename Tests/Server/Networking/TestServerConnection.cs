using System.Collections.Generic;
using MultiplayerProtocol;
using WebsocketMultiplayer.Server;
using WebsocketMultiplayer.Tests.Server.Handlers;

namespace WebsocketMultiplayer.Tests.Server
{
    public class TestServerConnection : ClientConnection
    {
        public TestServerConnection(IServerNetworkEndpoint endpoint, bool debug = false) : base(endpoint, debug)
        {
        }

        protected override IEnumerable<INetworkMessageListener> handlers
        {
            get
            {
                foreach (var h in base.handlers) yield return h;
                yield return new TestRequestHandler(this);
            }
        }
    }
}