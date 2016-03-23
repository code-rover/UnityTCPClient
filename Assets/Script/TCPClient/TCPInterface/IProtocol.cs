using System.IO;
using System;
using System.Collections.Generic;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// IProtocol
    /// 베이스 패킷
    /// </summary>
	[Serializable]
    public class IProtocol
    {
        //ushort Id { get; }
		public int msgid;
		public MemoryStream stream;
    }
}

