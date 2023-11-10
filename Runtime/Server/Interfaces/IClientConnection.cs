using System;
using System.Collections.Generic;
using MultiplayerProtocol;

namespace WebsocketMultiplayer.Server
{
    public interface IClientConnection
    {
        Guid userId { get;set; }

        IEnumerable<INetworkMessageListener> GetMultiplayerListeners(IMultiplayerServer server)
        {
            yield return new AuthMessageHandler(server, this);
        }
    }
}