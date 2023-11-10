using FileStore;
using JetBrains.Annotations;

namespace WebsocketMultiplayer.Client
{
    public interface IMultiplayerClient
    {
        string serverAddress { get; }
        
        WebsocketClient client { get; }
        IMultiplayerStore store { get; }
        IFileStorage files { get; }
        IMultiplayerConnection connection { get; }
        ILoginClient login { get; }

        void SetLoadingStatus([CanBeNull] string status);
    }
}