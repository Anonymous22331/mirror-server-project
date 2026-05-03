using System;
using System.Collections.Generic;
using Mirror;
using NetworkMessages.Message;
using UnityEngine;

namespace NetworkMessages.Client
{
    public sealed class MirrorClientNetworkMessageService : IClientNetworkMessageService
    {
        private readonly Dictionary<ushort, Action> _handlerCleanups = new();

        private readonly HashSet<ushort> _activeSubscriptions = new();

        private readonly HashSet<ushort> _confirmedSubscriptions = new();

        private bool _ackHandlerRegistered;
        public void Subscribe<TMessage>(Action<TMessage> handler)
            where TMessage : struct, NetworkMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            RegisterSubscriptionConfirmedHandler();

            ushort messageTypeId = NetworkMessageId<TMessage>.Id;

            if (!_handlerCleanups.ContainsKey(messageTypeId))
            {
                NetworkClient.RegisterHandler<TMessage>(msg =>
                {
                    if (_activeSubscriptions.Contains(messageTypeId) &&
                        _confirmedSubscriptions.Contains(messageTypeId))
                    {
                        handler(msg);
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Ignored {typeof(TMessage).Name} (not confirmed yet), id={messageTypeId}");
                    }
                });

                _handlerCleanups[messageTypeId] = () =>
                {
                    NetworkClient.UnregisterHandler<TMessage>();
                };
            }

            _activeSubscriptions.Add(messageTypeId);

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
                Debug.LogWarning($"Cannot subscribe to {typeof(TMessage).Name}: client not connected.");
            }
        }

        public void Unsubscribe<TMessage>()
            where TMessage : struct, NetworkMessage
        {
            ushort messageTypeId = NetworkMessageId<TMessage>.Id;

            _activeSubscriptions.Remove(messageTypeId);
            _confirmedSubscriptions.Remove(messageTypeId);

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
            foreach (var cleanup in _handlerCleanups.Values)
            {
                cleanup.Invoke();
            }

            _handlerCleanups.Clear();
            _activeSubscriptions.Clear();
            _confirmedSubscriptions.Clear();

            Debug.Log("Client local network message handlers cleared.");
        }
        private void RegisterSubscriptionConfirmedHandler()
        {
            if (_ackHandlerRegistered)
                return;

            _ackHandlerRegistered = true;

            NetworkClient.RegisterHandler<SubscriptionConfirmedMessage>(msg =>
            {
                _confirmedSubscriptions.Add(msg.MessageTypeId);

                Debug.Log($"Subscription confirmed for id={msg.MessageTypeId}");
            });
        }
    }
}