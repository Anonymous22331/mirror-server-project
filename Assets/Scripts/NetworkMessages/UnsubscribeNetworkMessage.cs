using Mirror;

namespace NetworkMessages.Message
{
    public struct UnsubscribeNetworkMessage : NetworkMessage
    {
        public ushort MessageTypeId;
    }
}