using MultiplayerProtocol;
using RSG;
using WebsocketMultiplayer.Client.Senders;

namespace WebsocketMultiplayer.Client
{
    public interface IMultiplayerConnection
    {
        Protocol protocol { get; }
        IPromise PerformProtocolHandshake();
        AuthSender auth { get; }
    }
}