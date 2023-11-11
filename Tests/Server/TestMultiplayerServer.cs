using MultiplayerProtocol;
using Newtonsoft.Json.Linq;
using RSG;
using WebsocketMultiplayer.Server;

namespace WebsocketMultiplayer.Tests.Server
{
    public class TestMultiplayerServer : IMultiplayerServer, ILoginServer, IUserJoinHandler
    {
        public IServerConfiguration configuration { get; }
        public IMultiplayerStore store { get; }
        public ILoginApi api { get; }

        public ILoginServer login => this;
        public IUserJoinHandler joinHandler => this;

        private WebsocketServer server { get; set; }

        public TestMultiplayerServer(ILoginApi api, string address, int port)
        {
            configuration = new ServerConfiguration(address, port);
            this.api = api;
            store = new TestServerStore();
        }

        public void Start()
        {
            server = WebsocketServer.CreateInstance();
            server.Initialize(this, endpoint => new ClientConnection(endpoint));
        }

        public void Stop()
        {
            if (server) server.Stop();
        }

        public IPromise<ResponseContextMessages> HandleUserJoin(IClientConnection connection, JObject json)
        {
            return Promise<ResponseContextMessages>.Resolved(null);
        }
    }
}