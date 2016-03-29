using System;
using System.Collections.Generic;


public class EventMap
{
	private Dictionary<int, int> eventMap = new Dictionary<int, int>();

	private EventMap ()
	{
		eventMap.Add (10001, 10002);
		eventMap.Add (10003, 10004);
	}

	private static EventMap instance;
	public static EventMap Instance() {
		if (instance == null)
			instance = new EventMap ();

		return instance;
	}

	public int getRespMsg(int k) {
		return eventMap[k];	
	}
}


