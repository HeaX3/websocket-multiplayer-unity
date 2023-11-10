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
        /// <param name="connection">Connection</param>
        /// <param name="json">User JSON provided by the login API</param>
        /// <returns>Serializable data which will be sent to the client. Use this to initialize the client application state.</returns>
        IPromise<ISerializableValue> HandleUserJoin(IClientConnection connection, [CanBeNull] JObject json);
    }

    public interface IUserJoinHandler<in T> : IUserJoinHandler where T : IClientConnection
    {
        IPromise<ISerializableValue> IUserJoinHandler.HandleUserJoin(IClientConnection connection, JObject json)
        {
            if (connection is not T t)
            {
                throw new InvalidCastException("Connection is not a " + typeof(T).Name);
            }

            return HandleUserJoin(t, json);
        }

        /// <summary>
        /// Perform operations that should happen after authenticating a client and before the client can start playing.
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="json">User JSON provided by the login API</param>
        /// <returns>Serializable data which will be sent to the client. Use this to initialize the client application state.</returns>
        IPromise<ISerializableValue> HandleUserJoin(T connection, [CanBeNull] JObject json);
    }
}