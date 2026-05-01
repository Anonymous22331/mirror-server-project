using NetworkMessages.Handler;
using NetworkMessages.Message;
using UnityEngine;

namespace NetworkMessages.Client
{
    public sealed class HelloMessageClientHandler : IClientNetworkMessageHandler<HelloMessage>
    {
        public void Handle(HelloMessage message)
        {
            Debug.Log($"Received HelloMessage: {message.Text}");
        }
    }
}