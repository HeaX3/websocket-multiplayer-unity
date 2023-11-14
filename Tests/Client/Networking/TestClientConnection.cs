using System.Collections.Generic;
using MultiplayerProtocol;
using RSG;

namespace WebsocketMultiplayer.Tests.Client
{
    public class TestClientConnection : ClientConnection
    {
        public TestClientConnection(INetworkEndpoint endpoint) : base(endpoint)
        {
        }

        protected override IEnumerable<INetworkMessageListener> handlers
        {
            get
            {
                foreach (var h in base.handlers) yield return h;
                yield return new TestResponseHandler();
            }
        }

        public IPromise SendTestRequest()
        {
            return new Promise((resolve, reject) =>
            {
                SendRequest(new RequestTestMessage()).Then(resolve).Catch(reject);
            });
        }
    }
}