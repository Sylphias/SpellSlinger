using System;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;


public class PlayerMessage : MessageBase
{
	private String message;
	private float val;
	private int playerID;

	public String Message{
		get{return this.message;}
		set{this.message = value;}
	}

	public float Val{
		get{return this.val;}
		set{this.val = value;}
	}

	public int PlayerID{
		get{return this.playerID;}
		set{this.playerID = value;}
	}


	public PlayerMessage ()
	{

	}
}

