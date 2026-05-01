using System;
using Mirror;
using NetworkMessages.Message;
using UnityEngine;

namespace NetworkMessages.Server
{
    public sealed class MirrorServerNetworkMessageService : IServerNetworkMessageService
    {
        private readonly IMessageSubscriptionRegistry _subscriptionRegistry;

        private bool _initialized;

        public event Action<NetworkConnectionToClient, ushort> ClientSubscribed;

        public MirrorServerNetworkMessageService(IMessageSubscriptionRegistry subscriptionRegistry)
        {
            _subscriptionRegistry = subscriptionRegistry;
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            NetworkServer.ReplaceHandler<SubscribeNetworkMessage>(OnSubscribeMessage);
            NetworkServer.ReplaceHandler<UnsubscribeNetworkMessage>(OnUnsubscribeMessage);

            NetworkServer.OnDisconnectedEvent += OnClientDisconnected;

            Debug.Log("Network message service initialized.");
        }

        public bool TrySend<TMessage>(
            NetworkConnectionToClient connection,
            TMessage message,
            int channelId = Channels.Reliable)
            where TMessage : struct, NetworkMessage
        {
            if (connection == null)
                return false;

            ushort messageTypeId = NetworkMessageId<TMessage>.Id;

            if (!_subscriptionRegistry.IsSubscribed(connection, messageTypeId))
            {
                Debug.Log($"Skip {typeof(TMessage).Name} for conn={connection.connectionId}: not subscribed.");
                return false;
            }

            connection.Send(message, channelId);

            Debug.Log($"Sent {typeof(TMessage).Name} to conn={connection.connectionId}");
            return true;
        }

        public void SendToSubscribers<TMessage>(
            TMessage message,
            int channelId = Channels.Reliable)
            where TMessage : struct, NetworkMessage
        {
            foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
            {
                TrySend(connection, message, channelId);
            }
        }

        public void Dispose()
        {
            if (!_initialized)
                return;

            _initialized = false;

            NetworkServer.UnregisterHandler<SubscribeNetworkMessage>();
            NetworkServer.UnregisterHandler<UnsubscribeNetworkMessage>();

            NetworkServer.OnDisconnectedEvent -= OnClientDisconnected;

            _subscriptionRegistry.Clear();

            Debug.Log("Network message service disposed.");
        }

        private void OnSubscribeMessage(NetworkConnectionToClient connection, SubscribeNetworkMessage message)
        {
            _subscriptionRegistry.Subscribe(connection, message.MessageTypeId);

            Debug.Log($"Conn={connection.connectionId} subscribed to messageId={message.MessageTypeId}");

            ClientSubscribed?.Invoke(connection, message.MessageTypeId);
        }

        private void OnUnsubscribeMessage(NetworkConnectionToClient connection, UnsubscribeNetworkMessage message)
        {
            _subscriptionRegistry.Unsubscribe(connection, message.MessageTypeId);

            Debug.Log($"Conn={connection.connectionId} unsubscribed from messageId={message.MessageTypeId}");
        }

        private void OnClientDisconnected(NetworkConnectionToClient connection)
        {
            _subscriptionRegistry.RemoveConnection(connection);

            Debug.Log($"Removed subscriptions for conn={connection.connectionId}");
        }
    }
}