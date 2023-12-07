using DataStores;
using UnityEngine;
using WebsocketMultiplayer.Server.Analytics;
using WebsocketMultiplayer.Server.Senders;

namespace WebsocketMultiplayer.Server
{
    public class AnalyticsModule : IDataModule
    {
        private IMultiplayerServer server { get; }
        private AnalyticsSender sender { get; }
        private AnalyticsController controller { get; set; }

        public AnalyticsModule(IMultiplayerServer server)
        {
            this.server = server;
            sender = new AnalyticsSender(server);
        }

        public void Initialize()
        {
            controller = AnalyticsController.Create(this);
        }

        public void Publish(float assumedFramerate)
        {
            sender.SendAnalytics(new AnalyticsFrame(
                (int)Time.realtimeSinceStartup,
                server.store.connection.GetConnections().Count,
                assumedFramerate
            ));
        }

        public void Unload()
        {
            Object.Destroy(controller);
        }
    }
}