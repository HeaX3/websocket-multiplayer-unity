using System;
using System.Collections.Generic;
using MultiplayerProtocol;

namespace WebsocketMultiplayer.Server
{
    public interface IClientConnection
    {
        Guid userId { get;set; }
    }
}