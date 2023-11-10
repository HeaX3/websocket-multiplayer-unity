namespace WebsocketMultiplayer.Server
{
    public class ServerConfiguration : IServerConfiguration
    {
        public string ipAddress { get; }
        public int port { get; }

        public ServerConfiguration(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }
    }
}