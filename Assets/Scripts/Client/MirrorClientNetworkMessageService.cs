using System;
using System.Collections.Generic;
using Mirror;
using NetworkMessages.Message;
using UnityEngine;

namespace NetworkMessages.Client
{
    public sealed class MirrorClientNetworkMessageService : IClientNetworkMessageService
    {
        private readonly Dictionary<ushort, Action> _localHandlerCleanups = new();

        public void Subscribe<TMessage>(Action<TMessage> handler)
            where TMessage : struct, NetworkMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            ushort messageTypeId = NetworkMessageId<TMessage>.Id;

            NetworkClient.ReplaceHandler(handler);

            _localHandlerCleanups[messageTypeId] = () => { NetworkClient.UnregisterHandler<TMessage>(); };

            if (NetworkClient.isConnected)
            {
                NetworkClient.Send(new SubscribeNetworkMessage
                {
                    MessageTypeId = messageTypeId
                });

                Debug.Log($"Subscribed to {typeof(TMessage).Name}, id={messageTypeId}");
            }
            else
            {
                Debug.LogWarning($"Cannot subscribe to {typeof(TMessage).Name}: client is not connected.");
            }
        }

        public void Unsubscribe<TMessage>()
            where TMessage : struct, NetworkMessage
        {
            ushort messageTypeId = NetworkMessageId<TMessage>.Id;

            NetworkClient.UnregisterHandler<TMessage>();
            _localHandlerCleanups.Remove(messageTypeId);

            if (NetworkClient.isConnected)
            {
                NetworkClient.Send(new UnsubscribeNetworkMessage
                {
                    MessageTypeId = messageTypeId
                });
            }

            Debug.Log($"Unsubscribed from {typeof(TMessage).Name}, id={messageTypeId}");
        }

        public void ClearLocalHandlers()
        {
            foreach (Action cleanup in _localHandlerCleanups.Values)
            {
                cleanup.Invoke();
            }

            _localHandlerCleanups.Clear();

            Debug.Log("Client local network message handlers cleared.");
        }
    }
}