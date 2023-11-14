using System;
using MultiplayerProtocol;
using WebsocketMultiplayer.Tests.Models;

namespace WebsocketMultiplayer.Tests.Server.Handlers
{
    public class TestRequestHandler : INetworkRequestHandler<RequestTestMessage>
    {
        private NetworkConnection connection { get; }

        public TestRequestHandler(NetworkConnection connection)
        {
            this.connection = connection;
        }

        public IRequestResponse Handle(RequestTestMessage message)
        {
            var user = new User(Guid.NewGuid())
            {
                name = "HeaX"
            };
            return new RequestResponse
            {
                postResponse = connection.protocol.Serialize(new INetworkMessage[]
                {
                    new ResponseTestMessage(user, new Player(Guid.NewGuid())
                    {
                        name = user.name,
                        areaId = Guid.NewGuid(),
                        userId = user.id
                    }),
                })
            };
        }
    }
}