using System.Collections.Generic;
using DataStores;
using WebsocketMultiplayer.Client;
using WebsocketMultiplayer.Client.Modules;

namespace WebsocketMultiplayer.Tests.Client
{
    public class MultiplayerStore : DataStore, IMultiplayerStore
    {
        public AuthModule auth { get; }
        public ConnectionModule connection { get; }

        public MultiplayerStore(TestMultiplayerClient client)
        {
            auth = new AuthModule(client);
            connection = new ConnectionModule(client);
        }
        
        protected override IEnumerable<IDataModule> modules
        {
            get
            {
                yield return auth;
                yield return connection;
            }
        }

        public static MultiplayerStore Create(TestMultiplayerClient client)
        {
            var result = new MultiplayerStore(client);
            result.Initialize();
            return result;
        }
    }
}