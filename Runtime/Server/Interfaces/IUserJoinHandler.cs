using System;
using JetBrains.Annotations;
using MultiplayerProtocol;
using Newtonsoft.Json.Linq;
using RSG;

namespace WebsocketMultiplayer.Server
{
    public interface IUserJoinHandler
    {
        /// <summary>
        /// Perform operations that should happen after authenticating a client and before the client can start playing.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="json">User JSON provided by the login API</param>
        /// <returns>Serializable data which will be sent to the client. Use this to initialize the client application state.</returns>
        IPromise<ISerializableValue> HandleUserJoin(Guid userId, [CanBeNull] JObject json);
    }
}