using System;
using Mirror;

namespace NetworkMessages.Handler
{
    public interface IClientNetworkMessageHandler<TMessage>
        where TMessage : struct, NetworkMessage
    {
        void Handle(TMessage message);
    }
}