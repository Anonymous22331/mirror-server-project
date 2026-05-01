using System;
using Mirror;
using NetworkMessages.Message;
using UnityEngine;
using Zenject;

namespace NetworkMessages.Server
{
    public sealed class HelloSubscriptionScenario : IInitializable, IDisposable
    {
        private readonly IServerNetworkMessageService _serverMessageService;

        private readonly ushort _helloMessageId = NetworkMessageId<HelloMessage>.Id;

        public HelloSubscriptionScenario(IServerNetworkMessageService serverMessageService)
        {
            _serverMessageService = serverMessageService;
        }

        public void Initialize()
        {
            _serverMessageService.ClientSubscribed += OnClientSubscribed;
        }

        public void Dispose()
        {
            _serverMessageService.ClientSubscribed -= OnClientSubscribed;
        }

        private void OnClientSubscribed(NetworkConnectionToClient connection, ushort messageTypeId)
        {
            if (messageTypeId != _helloMessageId)
                return;

            Debug.Log($"Client conn={connection.connectionId} subscribed to HelloMessage.");

            _serverMessageService.TrySend(connection, new HelloMessage
            {
                Text = "Hello Client!"
            });
        }
    }
}