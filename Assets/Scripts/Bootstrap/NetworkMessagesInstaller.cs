using NetworkMessages.Client;
using NetworkMessages.Handler;
using NetworkMessages.Message;
using NetworkMessages.Server;
using UnityEngine;
using Zenject;

namespace NetworkMessages.Bootstrap
{
    public sealed class NetworkMessagesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindClientServices();
            BindServerServices();
            BindScenarios();
            BindSceneComponents();
        }

        private void BindClientServices()
        {
            Container
                .Bind<IClientNetworkMessageService>()
                .To<MirrorClientNetworkMessageService>()
                .AsSingle();

            Container
                .Bind<IClientNetworkMessageHandler<HelloMessage>>()
                .To<HelloMessageClientHandler>()
                .AsSingle();
        }

        private void BindServerServices()
        {
            Container
                .Bind<IMessageSubscriptionRegistry>()
                .To<MessageSubscriptionRegistry>()
                .AsSingle();

            Container
                .Bind<IServerNetworkMessageService>()
                .To<MirrorServerNetworkMessageService>()
                .AsSingle();
        }

        private void BindScenarios()
        {
            Container
                .BindInterfacesAndSelfTo<HelloSubscriptionScenario>()
                .AsSingle()
                .NonLazy();
        }

        private void BindSceneComponents()
        {
            Container
                .Bind<NetworkMessagesBootstrap>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}