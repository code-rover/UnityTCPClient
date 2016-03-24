using System;
using System.IO;
using UNITY_TCPCLIENT;

/// <summary>
/// 2015/12/05 Hyeon
/// TCPClientCommon
/// 어플리케이션 레벨 Common 상수 정의
/// </summary>
public class ClientCommon
{
    /***
     * 헤더 사이즈
     */
    public static readonly int HEADER_SIZE = 4;

    /***
     * 패킷 Body 크기 헤더 사이즈
     */
    public static readonly int BODY_LENGTH_SIZE = 2;

    /***
     * 패킷 아이디 사이즈
     */
    public static readonly int ID_SIZE = 2;

	/**
	 *  p1: msgno
	 *  p2: Protobuf object
	 **/
	public static PacketStream msgpack(int msgno, ProtoBuf.IExtensible obj) {
		MemoryStream stream = new MemoryStream();
		ProtoBuf.Serializer.NonGeneric.Serialize(stream, obj);

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

		return packet;
	}


	public static object msgunpack(PacketStream packet, out int msgno) {
		byte[] recv = packet.Buffer;
		msgno = recv [0] << 24 | recv [1] << 16 | recv [2] << 8 | recv [3];

		MemoryStream stream = new MemoryStream (recv, 4, packet.Size - 4);

		Type type = MsgType.Instance ().getMsgType (msgno);
		if (type != null) {
			object obj = ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
			return obj;
		}
		return null;
	}
}

