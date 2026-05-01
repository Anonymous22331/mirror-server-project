using Mirror;

namespace NetworkMessages.Server
{
    public interface IMessageSubscriptionRegistry
    {
        void Subscribe(NetworkConnectionToClient connection, ushort messageTypeId);
        void Unsubscribe(NetworkConnectionToClient connection, ushort messageTypeId);
        bool IsSubscribed(NetworkConnectionToClient connection, ushort messageTypeId);
        void RemoveConnection(NetworkConnectionToClient connection);
        void Clear();
    }
}