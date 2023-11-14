using MultiplayerProtocol;
using WebsocketMultiplayer.Tests.Models;

namespace WebsocketMultiplayer.Tests
{
    public class ResponseTestMessage : INetworkMessage
    {
        public User user { get; private set; }
        public Player player { get; private set; }

        public ResponseTestMessage()
        {
        }

        public ResponseTestMessage(User user, Player player)
        {
            this.user = user;
            this.player = player;
        }

        public void SerializeInto(SerializedData message)
        {
            message.Write(user);
            message.Write(player);
        }

        public void DeserializeFrom(SerializedData message)
        {
            user = message.Read<User>();
            player = message.Read<Player>();
        }
    }
}