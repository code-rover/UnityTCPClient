using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/08 Hyeon
    /// NetworkConnectedState
    /// 네트워크 연결 상태
    /// </summary>
    public class NetworkConnectedState : INetworkState
    {
        private NetworkManager mNetworkManager = null;
        private IClientSession mClientSession = null;

        public NetworkConnectedState(IClientSession clientSession)
        {
            this.mClientSession = clientSession;
        }

        public void Enter(NetworkManager networkManager)
        {            
            mNetworkManager = networkManager;

            if( mNetworkManager.connectCompleteCallback != null )
                mNetworkManager.connectCompleteCallback();
        }

        public void Connect(string host, int port, IProtocolResolver protocolResolver)
        {
            
        }

        public void Disconnect()
        {
            mClientSession.Disconnect();
        }

        public void Reconnect()
        {
        }

        public void Send(PacketStream packet)
        {
            if (mClientSession == null)
                return;

            mClientSession.Send(packet);
        }
    }
}
