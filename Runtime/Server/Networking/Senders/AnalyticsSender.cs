namespace WebsocketMultiplayer.Server.Senders
{
    public class AnalyticsSender : AdminMessageSender
    {
        public AnalyticsSender(IMultiplayerServer server) : base(server)
        {
        }

        public void SendAnalytics(AnalyticsFrame frame)
        {
            SendToAll(new AnalyticsMessage(frame));
        }
    }
}