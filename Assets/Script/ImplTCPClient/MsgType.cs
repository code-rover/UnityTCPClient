using System;
using System.Collections.Generic;



public class MsgType
{
	private Dictionary<int,Type> msg_dic = new Dictionary<int, Type>();

	private static MsgType instance;
	public static MsgType Instance() {
		if (instance == null)
			instance = new MsgType ();
		
		return instance;
	}

	private MsgType ()
	{
		msg_dic.Add (10002, typeof(login_message.CMsgAccountLoginResponse));
		msg_dic.Add (10004, typeof(login_message.CMsgAccountRegistResponse));
	}

	public Type getMsgType(int msgno) {
		Type msgType = null;
		msg_dic.TryGetValue (msgno, out msgType);

		return msgType;
	}
}


