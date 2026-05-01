using Mirror;

namespace NetworkMessages.Message
{
    public struct SubscribeNetworkMessage : NetworkMessage
    {
        public ushort MessageTypeId;
    }
}