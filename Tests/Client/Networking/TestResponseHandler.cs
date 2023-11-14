using MultiplayerProtocol;
using UnityEngine;

namespace WebsocketMultiplayer.Tests.Client
{
    public class TestResponseHandler : INetworkMessageHandler<ResponseTestMessage>
    {
        public void Handle(ResponseTestMessage message)
        {
            Debug.Log((message.user.name ?? "<null>") + " (" + message.user.id + ")");
        }
    }
}