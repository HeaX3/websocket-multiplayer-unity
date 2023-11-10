namespace WebsocketMultiplayer.Server
{
    public interface IMultiplayerServer
    {
        IServerConfiguration configuration { get; }
        IMultiplayerStore store { get; }
        ILoginServer login { get; }
        IUserJoinHandler joinHandler { get; }
    }
}