using MultiplayerProtocol;

namespace WebsocketMultiplayer.Server
{
    public interface IServerNetworkEndpoint : INetworkEndpoint
    {
        IMultiplayerServer server { get; }
        string id { get; }
    }
}