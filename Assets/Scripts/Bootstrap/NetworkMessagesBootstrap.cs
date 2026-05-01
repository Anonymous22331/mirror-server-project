using Mirror;
using NetworkMessages.Client;
using NetworkMessages.Handler;
using NetworkMessages.Message;
using NetworkMessages.Server;
using UnityEngine;
using Zenject;

namespace NetworkMessages.Bootstrap
{
    public sealed class NetworkMessagesBootstrap : MonoBehaviour
    {
        private IServerNetworkMessageService _serverMessageService;
        private IClientNetworkMessageService _clientMessageService;
        private IClientNetworkMessageHandler<HelloMessage> _helloMessageHandler;

        private bool _serverServiceInitialized;
        private bool _clientWasConnected;
        private bool _clientSubscriptionSent;

        [Inject]
        public void Construct(
            IServerNetworkMessageService serverMessageService,
            IClientNetworkMessageService clientMessageService,
            IClientNetworkMessageHandler<HelloMessage> helloMessageHandler)
        {
            _serverMessageService = serverMessageService;
            _clientMessageService = clientMessageService;
            _helloMessageHandler = helloMessageHandler;
        }

        private void Update()
        {
            InitializeServerServiceIfNeeded();
            TrackClientConnectionState();
        }

        private void OnDestroy()
        {
            if (_serverServiceInitialized)
                _serverMessageService.Dispose();
        }

        private void InitializeServerServiceIfNeeded()
        {
            if (_serverServiceInitialized)
                return;
            if (!NetworkServer.active)
                return;

            _serverServiceInitialized = true;

            _serverMessageService.Initialize();

            Debug.Log("Server initialized.");
        }

        private void TrackClientConnectionState()
        {
            bool isClientConnected = NetworkClient.active && NetworkClient.isConnected;

            if (isClientConnected && !_clientWasConnected)
            {
                _clientWasConnected = true;
                OnClientConnected();
            }

            if (!isClientConnected && _clientWasConnected)
            {
                _clientWasConnected = false;
                OnClientDisconnected();
            }
        }

        private void OnClientConnected()
        {
            Debug.Log("Client connected.");

            SubscribeClientToHelloMessage();
        }

        private void OnClientDisconnected()
        {
            Debug.Log("Client disconnected.");

            _clientSubscriptionSent = false;
            _clientMessageService.ClearLocalHandlers();
        }

        private void SubscribeClientToHelloMessage()
        {
            if (_clientSubscriptionSent)
                return;

            _clientSubscriptionSent = true;

            Debug.Log("Subscribe client to HelloMessage.");

            _clientMessageService.Subscribe<HelloMessage>(_helloMessageHandler.Handle);
        }
    }
}