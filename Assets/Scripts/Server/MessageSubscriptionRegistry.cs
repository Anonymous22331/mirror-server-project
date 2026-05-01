using System.Collections.Generic;
using Mirror;

namespace NetworkMessages.Server
{
    public sealed class MessageSubscriptionRegistry : IMessageSubscriptionRegistry
    {
        private readonly Dictionary<int, HashSet<ushort>> _subscriptionsByConnectionId = new();

        public void Subscribe(NetworkConnectionToClient connection, ushort messageTypeId)
        {
            if (connection == null)
                return;

            if (!_subscriptionsByConnectionId.TryGetValue(connection.connectionId, out HashSet<ushort> subscriptions))
            {
                subscriptions = new HashSet<ushort>();
                _subscriptionsByConnectionId.Add(connection.connectionId, subscriptions);
            }

            subscriptions.Add(messageTypeId);
        }

        public void Unsubscribe(NetworkConnectionToClient connection, ushort messageTypeId)
        {
            if (connection == null)
                return;

            if (!_subscriptionsByConnectionId.TryGetValue(connection.connectionId, out HashSet<ushort> subscriptions))
                return;

            subscriptions.Remove(messageTypeId);

            if (subscriptions.Count == 0)
                _subscriptionsByConnectionId.Remove(connection.connectionId);
        }

        public bool IsSubscribed(NetworkConnectionToClient connection, ushort messageTypeId)
        {
            if (connection == null)
                return false;

            return _subscriptionsByConnectionId.TryGetValue(connection.connectionId, out HashSet<ushort> subscriptions)
                   && subscriptions.Contains(messageTypeId);
        }

        public void RemoveConnection(NetworkConnectionToClient connection)
        {
            if (connection == null)
                return;

            _subscriptionsByConnectionId.Remove(connection.connectionId);
        }

        public void Clear()
        {
            _subscriptionsByConnectionId.Clear();
        }
    }
}