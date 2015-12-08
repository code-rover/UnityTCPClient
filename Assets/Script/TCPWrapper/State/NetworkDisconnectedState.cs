using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/08 Hyeon
    /// NetworkConnectedState
    /// 네트워크 연결 해제 상태
    /// </summary>
    public class NetworkDisconnectedState : INetworkState
    {
        private NetworkManager mNetworkManager = null;
        private String mHost;
        private int mPort;
        private IProtocolResolver mProcotolResolver = null;
        private IClientSession mClientSession = null;

        public NetworkDisconnectedState(String host, int port, IProtocolResolver protocolResolver, IClientSession clientSession)
        {
            this.mHost = host;
            this.mPort = port;
            this.mProcotolResolver = protocolResolver;
            this.mClientSession = clientSession;
        }

        public void Enter(NetworkManager networkManager)
        {
            mNetworkManager = networkManager;

            if( mNetworkManager.disconnectedCompleteCallback != null )
                mNetworkManager.disconnectedCompleteCallback();
        }

        public void Connect(string host, int port, IProtocolResolver protocolResolver)
        {
        }

        public void Disconnect()
        {
            
        }

        public void Reconnect()
        {
            TCPConnector connector = new TCPConnector();
            connector.connectHandler = mNetworkManager.OnReconnectComplete;
            connector.failHandler = mNetworkManager.OnReconnectFail;
            connector.Connect(mHost, mPort, mProcotolResolver, mClientSession.TcpSession);
        }

        public void Send(PacketStream packet)
        {
        }
    }
}
