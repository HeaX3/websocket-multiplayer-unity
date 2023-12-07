using MultiplayerProtocol;

namespace WebsocketMultiplayer
{
    public class AnalyticsMessage : INetworkMessage
    {
        public AnalyticsFrame frame { get; private set; }
        
        public AnalyticsMessage(){}

        public AnalyticsMessage(AnalyticsFrame frame)
        {
            this.frame = frame;
        }

        public void SerializeInto(SerializedData message)
        {
            message.Write(frame);
        }

        public void DeserializeFrom(SerializedData message)
        {
            frame = message.Read<AnalyticsFrame>();
        }
    }
}