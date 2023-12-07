using MultiplayerProtocol;
using WebsocketMultiplayer.Client.Modules;

namespace WebsocketMultiplayer.Client.Handlers
{
    public class AnalyticsHandler : INetworkMessageHandler<AnalyticsMessage>
    {
        private AnalyticsModule module { get; }

        public AnalyticsHandler(AnalyticsModule module)
        {
            this.module = module;
        }

        public void Handle(AnalyticsMessage message)
        {
            module.HandleFrame(message.frame);
        }
    }
}