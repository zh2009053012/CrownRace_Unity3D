using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

public class MessageEvent {
	public delegate void NetMessageEvent(byte[] data);
	public string methodName;
	public NetMessageEvent method;
	public MessageEvent(){}
	public MessageEvent(NetMessageEvent method, string methodName="")
	{
		this.method = method;
		this.methodName = methodName;
	}
}
public class ServerMessageEvent{
	public delegate void NetMessageEvent (int player_id, byte[] data);
	public string methodName;
	public NetMessageEvent method;
	public ServerMessageEvent(){
	}
	public ServerMessageEvent(NetMessageEvent method, string methodName="")
	{
		this.method = method;
		this.methodName = methodName;
	}
}
