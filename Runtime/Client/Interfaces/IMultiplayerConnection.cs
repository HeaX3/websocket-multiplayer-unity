using RSG;
using WebsocketMultiplayer.Client.Senders;

namespace WebsocketMultiplayer.Client
{
    public interface IMultiplayerConnection
    {
        IPromise PerformProtocolHandshake();
        AuthSender auth { get; }
    }
}