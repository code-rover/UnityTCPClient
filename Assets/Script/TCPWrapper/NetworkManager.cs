using UnityEngine;
using System;


namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// NetworkManager
    /// 유니티 네트워크 매니저
    /// </summary>
    /// 
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _instance = null;
        public static NetworkManager getInstance()
        {
            if (_instance == null)
            {
                GameObject goNetworkManager = new GameObject();
                _instance = goNetworkManager.AddComponent<NetworkManager>();
            }

            return _instance;
        }

        public delegate void NetworkEventCallback();
        public delegate void ReceiveEventCallback(PacketStream packetStream);

        public NetworkEventCallback connectFailCallback { private get; set; }
        public NetworkEventCallback reconnectFailCallback { private get; set; }
        public NetworkEventCallback connectCompleteCallback {  get; set; }
        public NetworkEventCallback disconnectedCompleteCallback {  get; set; }
        public ReceiveEventCallback receiveEventCallback { private get; set; }

        private INetworkState mNetworkState = null;
        private NetworkSyncQueue mNetworkSyncQueue = null;
        private IClientSession mClientSession = null;
        private IProtocolResolver mProtocolResolver = null;
        private String mIp;
        private int mPort;

        private TCPCommon.NETWORK_STATE mTCPState;
        public TCPCommon.NETWORK_STATE tcpState
        {
            get { return mTCPState; }
        }
        
        /// <summary>
        /// 호스트 세팅
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="protocolResolver"></param>
        public void SettingConnector(string host, int port, IProtocolResolver protocolResolver)
        {
            this.mIp = host;
            this.mPort = port;
            this.mProtocolResolver = protocolResolver;
        }

        /// <summary>
        /// 서버 최초 연결
        /// </summary>
        /// <param name="host">The hosts.</param>
        /// <param name="port">The port.</param>
        public void Connect(string host, int port, IProtocolResolver protocolResolver)
        {
            mNetworkState.Connect(host, port, protocolResolver);
        }

        /// <summary>
        /// 연결 해제
        /// </summary>
        public void Disconnect()
        {
            mNetworkState.Disconnect();
        }

        /// <summary>
        /// 재연결
        /// </summary>
        public void Reconnect()
        {
            mNetworkState.Reconnect();
        }

        /// <summary>
        /// States the handle.
        /// State는 두개뿐이니 일단은 이렇게.
        /// 만약 더 추가 될게 있으면 State패턴으로 변경
        /// </summary>
        /// <param name="state">The state.</param>
        private void SwitchStateHandle(TCPCommon.NETWORK_STATE state)
        {
            if (mTCPState == state)
                return;

            mTCPState = state;
            Debug.Log("Network State Event : " + mTCPState);
            switch (mTCPState)
            {
                case TCPCommon.NETWORK_STATE.CONNECT:
                    {
                        mNetworkState = new NetworkConnectedState(mClientSession);
                    }
                    break;

                case TCPCommon.NETWORK_STATE.DISCONNECT:
                    {
                        mNetworkState = new NetworkDisconnectedState(mIp, mPort, mProtocolResolver, mClientSession);
                    }
                    break;

                case TCPCommon.NETWORK_STATE.NONE:
                    {
                        mNetworkState = new NetworkNoneState();
                    }
                    break;

            }

            mNetworkState.Enter(this);
        }

        /// <summary>
        /// Packet Send
        /// </summary>
        /// <param name="packet"></param>
        public void Send(PacketStream packet)
        {
            mNetworkState.Send(packet);
        }

        #region CALLBACK
        /// <summary>
        /// 연결 완료
        /// </summary>
        /// <param name="tcp_session">The tcp_session.</param>
        public void OnConnnectComplete(TCPSession tcpSession)
        {
            mNetworkSyncQueue = new NetworkSyncQueue();
            mClientSession = new ClientSession(tcpSession, mNetworkSyncQueue, mProtocolResolver);
        }

        /// <summary>
        /// 연결 실패
        /// </summary>
        public void OnConnectFail()
        {
            Debug.LogError("TCP Connect Fail");
            if (connectFailCallback != null)
            {
                connectFailCallback();
            }
        }

        /// <summary>
        /// 재연결 
        /// </summary>
        /// <param name="tcp_session">The tcp_session.</param>
        public void OnReconnectComplete(TCPSession tcpSession)
        {
        }

        /// <summary>
        /// 재연결 실패
        /// </summary>
        public void OnReconnectFail()
        {
            Debug.LogError("TCP Reconnect Fail");
            if (reconnectFailCallback != null)
            {
                reconnectFailCallback();
            }
        }

        /// <summary>
        /// Called when [receive].
        /// </summary>
        /// <param name="bytes_packet">The bytes_packet.</param>
        private void OnReceive(PacketStream packet)
        {
            receiveEventCallback(packet);
        }
        #endregion

        private void Awake()
        {
            SwitchStateHandle(TCPCommon.NETWORK_STATE.NONE);
        }

        private void Update()
        {
            if (mNetworkSyncQueue == null)
            {
                return;
            }

            /***
             * 네트워크 이벤트
             */
            if (this.mNetworkSyncQueue.HasNetworkStateEvent())
            {
                /***
                 * 큐에서 Thread 경쟁이 붙을 경우 Null이 반환 될 수 있다.
                 * Null 체크를 꼭해주자
                 */
                Const<TCPCommon.NETWORK_STATE> constNetworkState = this.mNetworkSyncQueue.PopNetworkStateEvent();
                if (constNetworkState != null)
                {
                    SwitchStateHandle(constNetworkState.Value);
                }
            }

            if (mTCPState != TCPCommon.NETWORK_STATE.CONNECT)
                return;

            /***
             * 패킷 데이터
             */
            if (this.mNetworkSyncQueue.HasReceivePacket())
            {
                /***
                 * 큐에서 Thread 경쟁이 붙을 경우 Null이 반환 될 수 있다.
                 * Null 체크를 꼭해주자
                 */
                PacketStream packet = this.mNetworkSyncQueue.PopReceivePacket();
                if (packet != null)
                {
                    OnReceive(packet);
                }
            }


        }

        private void OnDestroy()
        {
            Disconnect();
        }

    }

}
