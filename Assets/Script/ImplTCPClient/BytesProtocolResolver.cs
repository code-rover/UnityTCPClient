using System;
using UNITY_TCPCLIENT;

/// <summary>
/// 2015/12/05 Hyeon
/// BytesProtocolResolver
/// 바이트 프로토콜
/// </summary>
public class BytesProtocolResolver : IProtocolResolver
{
    PacketStream IProtocolResolver.PacketProtocolResolve(ArraySegment<byte> segmentBytes, out int packetSize)
    {
        packetSize = 0;

        // 헤더 사이즈 체크
		if (segmentBytes.Count < 2 + 4)  //pack size + msgno size
        {
            return null;
        }

		//byte[] recv = new byte[segmentBytes.Count];
		byte[] recv = segmentBytes.Array;

		UInt16 bodyLen = (ushort)(recv[0] << 8 | recv[1]);


        // Body 사이즈  Body 大小
        //UInt16 bodyLen = BitConverter.ToUInt16(segmentBytes.Array, segmentBytes.Offset);
		packetSize = 2 + bodyLen;

        // 패킷 사이즈  (信息包尺寸)
        //packetSize = bodyLen + ClientCommon.HEADER_SIZE;
        // Body 사이즈 체크
        if (segmentBytes.Count < packetSize)
        {
            return null;
        }
        // 헤더의 Len을 제외한 만큼 패킷을 복사해서 넘겨주자.
		//byte[] packetData = new byte[packetSize - ClientCommon.BODY_LENGTH_SIZE];
		byte[] packetData = new byte[bodyLen];
        int copyOffset = segmentBytes.Offset + 2;
        //int copyLength = packetSize - ClientCommon.BODY_LENGTH_SIZE;

		//Array.Copy(segmentBytes.Array, copyOffset, packetData, 0, copyLength);
		Array.Copy(segmentBytes.Array, copyOffset, packetData, 0, bodyLen);
        PacketStream packet = new PacketStream(packetData, packetData.Length);

        return packet;
    }
}

