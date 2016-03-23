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

		mNetworkManager.SwitchStateHandle(TCPCommon.NETWORK_STATE.NONE);

        mNetworkManager.mReceiveEventCallback = TestRecv;
        mNetworkManager.mConnectCompleteEvent += () => { text = "Connect";  Debug.Log("Connect!"); };
        mNetworkManager.mDisconnectedCompleteEvent += () => { text = "Disconnect"; Debug.Log("Disconnect!"); };
		mNetworkManager.mConnectFailEvent += () => { text = "Connect Failed"; Debug.Log("Connect Fail!"); };
        mNetworkManager.mReconnectFailEvent += () => { text = "Reconnect Fail"; Debug.Log("Reconnect Fail!"); };
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

	/*
    public void TestSend()
    {
		CMsgAccountLoginRequest obj = new CMsgAccountLoginRequest();
		obj.account = "a";
		obj.password = "a";

		MemoryStream s = new MemoryStream();
		//Serializer.NonGeneric.Serialize(s, obj);
		ProtoBuf.Serializer.Serialize<CMsgAccountLoginRequest>(s, obj);
		s.Position = 0;

		Debug.Log ("send size: " + s.Length);

		IProtocol pack = new Packet();
		pack.msgid = 10001;
		pack.stream = s;

        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                var formatter = new BinaryFormatter();
				formatter.Serialize(stream, pack);

                //UInt16 size = (ushort)((ushort)stream.Length - (ushort)ClientCommon.BODY_LENGTH_SIZE);
				UInt16 size = (ushort)stream.Length;
				PacketStream packet = new PacketStream((int)(ClientCommon.BODY_LENGTH_SIZE + (int)stream.Length));
                packet.Push(size);
                packet.Push(stream.GetBuffer(), (int)stream.Length);

                mNetworkManager.Send(packet);
            }
        }
        
    }
*/
	private void SendMessageToServer(int msgno, ProtoBuf.IExtensible obj)
    {
		MemoryStream stream = new MemoryStream();
		ProtoBuf.Serializer.NonGeneric.Serialize(stream, obj);
		//ProtoBuf.Serializer.Serialize<CMsgAccountLoginRequest>(stream, obj);

        //总长度=数据包长度所占字节+消息号长度+消息体长度
        byte[] msgBody = stream.ToArray();
        short msgBodySize = Convert.ToInt16(msgBody.Length);

        int pkgSize = 2 + 4 + msgBodySize;
        byte[] msgByte = new byte[pkgSize];

        int avaliableSize = pkgSize - 2;
        msgByte[0] = (byte)(avaliableSize >> 8);
        msgByte[1] = (byte)(avaliableSize);

        msgByte[2] = (byte)(msgno >> 24);
        msgByte[3] = (byte)(msgno >> 16);
        msgByte[4] = (byte)(msgno >> 8);
        msgByte[5] = (byte)(msgno);

		//Array.Copy (msgBody, 0, msgByte, 6, msgBody.Length - 6);

        int index = 6;
        for (int i = 0; i < msgBody.Length; i++)
        {
            msgByte[index] = msgBody[i];
            index++;
        }

		PacketStream packet = new PacketStream(msgByte.Length);
		packet.Push(msgByte, msgByte.Length);
		mNetworkManager.Send(packet);
    }
	
	public void TestSend()
	{
		CMsgAccountLoginRequest obj = new CMsgAccountLoginRequest();
		obj.account = "a";
		obj.password = "a";

		int msgno = 10001;
		SendMessageToServer (msgno, obj);
	}

    private void TestRecv(PacketStream packet)
    {
		byte[] recv = packet.Buffer;
		int msgno = recv [0] << 24 | recv [1] << 16 | recv [2] << 8 | recv [3];

		MemoryStream stream = new MemoryStream (recv, 4, packet.Size - 4);

		print ("msgno is: " + msgno);
       
		CMsgAccountLoginResponse res = ProtoBuf.Serializer.Deserialize<CMsgAccountLoginResponse> (stream);

		print (res);
		print (res.accountid);
    }


}
