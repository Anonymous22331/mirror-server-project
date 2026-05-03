using Mirror;

namespace NetworkMessages.Message
{
    public struct SubscriptionConfirmedMessage : NetworkMessage
    {
        public ushort MessageTypeId;
    }
}