using System.Collections.Generic;
using DataStores;
using WebsocketMultiplayer.Server;

namespace WebsocketMultiplayer.Tests.Server
{
    public class TestServerStore : DataStore, IMultiplayerStore
    {
        public ConnectionModule connection { get; } = new ConnectionModule<TestServerConnection>();

        protected override IEnumerable<IDataModule> modules
        {
            get { yield return connection; }
        }
    }
}