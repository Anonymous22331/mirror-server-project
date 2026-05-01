using System;
using Mirror;

namespace NetworkMessages.Client
{
    public interface IClientNetworkMessageService
    {
        void Subscribe<TMessage>(Action<TMessage> handler)
            where TMessage : struct, NetworkMessage;

        void Unsubscribe<TMessage>()
            where TMessage : struct, NetworkMessage;

        void ClearLocalHandlers();
    }
}