﻿using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    public class ServerNetworkManager : NetworkManager
    {
        public static ServerNetworkManager Ins;

        private void Awake()
        {
            Ins = this;

            Server.Ins.Init();
        }

        private void OnDestroy()
        {
            Ins = null;

            Server.Ins.Dispose();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Server.Ins.OnStartServer();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            Server.Ins.OnStopServer();
        }

        public override void OnServerConnect(NetworkConnection net)
        {
            base.OnServerConnect(net);

            Server.Ins.OnClientConnect(net);
        }

        public override void OnServerDisconnect(NetworkConnection net)
        {
            base.OnServerDisconnect(net);

            Server.Ins.OnClientDisconnect(net);
        }
    }
}