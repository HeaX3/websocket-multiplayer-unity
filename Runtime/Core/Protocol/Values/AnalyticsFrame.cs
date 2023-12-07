using MultiplayerProtocol;

namespace WebsocketMultiplayer
{
    public struct AnalyticsFrame : ISerializableValue
    {
        public int secondsSinceStartup;
        public int connections;
        public float assumedFrameRate;

        public AnalyticsFrame(int secondsSinceStartup, int connections, float assumedFrameRate)
        {
            this.secondsSinceStartup = secondsSinceStartup;
            this.connections = connections;
            this.assumedFrameRate = assumedFrameRate;
        }
        
        public void SerializeInto(SerializedData message)
        {
            message.Write(secondsSinceStartup);
            message.Write(connections);
            message.Write(assumedFrameRate);
        }

        public void DeserializeFrom(SerializedData message)
        {
            secondsSinceStartup = message.ReadInt();
            connections = message.ReadInt();
            assumedFrameRate = message.ReadFloat();
        }
    }
}