namespace WebsocketMultiplayer.Server
{
    public interface IServerConfiguration
    {
        string ipAddress { get; }
        int port { get; }
    }
}