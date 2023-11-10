namespace WebsocketMultiplayer.Server
{
    public interface IMultiplayerStore
    {
        ConnectionModule connection { get; }
    }
}