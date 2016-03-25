using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using UNITY_TCPCLIENT;
using ProtoBuf;
using login_message;

/// <summary>
/// Echo 패킷 테스트 컴포넌트
/// </summary>
public class EchoNetworkTest : MonoBehaviour {

	//public String IP = "127.0.0.1";
    public String IP = "192.168.1.222";
	//public int PORT = 9001;
    public int PORT = 8888;
    public Text StateText;

	private String text = "";

    private NetworkManager mNetworkManager;
    
    void Start()
    {
        mNetworkManager = NetworkManager.getInstance();

		Debug.Log ("main thread: " + System.Threading.Thread.CurrentThread.GetHashCode());

        mNetworkManager.mReceiveEventCallback = TestRecv;
		mNetworkManager.mConnectCompleteEvent += () => { text = "Connect";  Debug.Log("Connect! " + System.Threading.Thread.CurrentThread.GetHashCode()); };
		mNetworkManager.mDisconnectedCompleteEvent += () => { text = "Disconnect"; Debug.Log("Disconnect! " + System.Threading.Thread.CurrentThread.GetHashCode()); };
		mNetworkManager.mConnectFailEvent += () => { text = "Connect Failed"; Debug.Log("Connect Fail!" + System.Threading.Thread.CurrentThread.GetHashCode()); };
		mNetworkManager.mReconnectFailEvent += () => { text = "Reconnect Fail"; Debug.Log("Reconnect Fail! " + System.Threading.Thread.CurrentThread.GetHashCode()); };
    
	}

	void Update() {
		StateText.text = text;				
	}

    public void TestConnect()
    {
        mNetworkManager.Connect(IP, PORT, new BytesProtocolResolver());
    }

    public void TestReconnect()
    {
        mNetworkManager.Reconnect();
    }

    public void TestDisconnect()
    {
        mNetworkManager.Disconnect();
    }

	public void TestSend()
	{
		CMsgAccountLoginRequest obj = new CMsgAccountLoginRequest();
		obj.account = "b";
		obj.password = "bb";

		int msgno = 10001;

		PacketStream ps = ClientCommon.msgpack(msgno, obj);
		mNetworkManager.Send(ps);
	}

    private void TestRecv(PacketStream packet)
    {
		Debug.Log ("TestRecv thread: " + System.Threading.Thread.CurrentThread.GetHashCode());

		int msgno;
		object obj = ClientCommon.msgunpack (packet, out msgno);

		Type t = MsgType.Instance ().getMsgType (msgno);

		print(msgno);
		print (obj);
		//print (s);
    }


}
