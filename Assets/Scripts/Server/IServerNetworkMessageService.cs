using System;
using Mirror;

namespace NetworkMessages.Server
{
    public interface IServerNetworkMessageService : IDisposable
    {
        event Action<NetworkConnectionToClient, ushort> ClientSubscribed;

        void Initialize();

        bool TrySend<TMessage>(
            NetworkConnectionToClient connection,
            TMessage message,
            int channelId = Channels.Reliable)
            where TMessage : struct, NetworkMessage;

        void SendToSubscribers<TMessage>(
            TMessage message,
            int channelId = Channels.Reliable)
            where TMessage : struct, NetworkMessage;
    }
}