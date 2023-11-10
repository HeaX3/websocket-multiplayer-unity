using WebsocketMultiplayer.Client.Modules;

namespace WebsocketMultiplayer.Client
{
    public interface IMultiplayerStore
    {
        AuthModule auth { get; }
        ConnectionModule connection { get; }
    }
}