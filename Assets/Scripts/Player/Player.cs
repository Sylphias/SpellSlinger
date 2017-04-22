using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using Spells;
[RequireComponent(typeof(Player))]
public class Player : NetworkBehaviour,IPlayer {
	private Transform myTransform;
	private Animation playerAnimations;

	[SyncVar] public string playerName;

	[SyncVar] public string playerUniqueIdentity;
    GameObject playerCam;
//	[SyncVar]
    public string state{get;set;}
	public int Kills{get;set;}
	public int Deaths{get;set;}
	public List<IBuffable> BuffList{get;set;}

	private	void Awake()
	{
		myTransform = transform;
	}       

	// Generate Player ID only once player has joined the server

	public override void OnStartClient(){
		base.OnStartClient();
		string _netID =  GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();
		GameManager.RegisterPlayer(_netID,_player);

	}

    [Command]
    void CmdUpdateAllClients()
    {
        RpcUpdateNonLocalClient();
    }

	// Initialize the client on all previously joined clients
    [ClientRpc]
    void RpcUpdateNonLocalClient()
    {
        if (!isLocalPlayer)
        {
			foreach (IPlayer p in GetComponents<IPlayer>())
			{
				p.Init();
			}       
        }
    }

	// Initialize all Non local players who have joined prior to this client on this client
	void InitializeAllNonLocalJoinedPlayers(){
		foreach(Player p in GameManager.GetAllPlayers()){
			if(!p.isLocalPlayer)
				foreach(IPlayer playerComponent in p.GetComponents<IPlayer>())
					p.Init();
		}
	}

	public override void OnStartLocalPlayer(){
        if (!isLocalPlayer) return;
        Init();
        InitializeCamera();
        CmdUpdateAllClients();
        playerAnimations = GetComponent<Animation>();
        base.OnStartLocalPlayer();
	}

    public void Init()
    {
        state = "alive";
        Deaths = 0;
        BuffList = new List<IBuffable>();
    }

    void InitializeCamera(){
		GameObject camera = Resources.Load ("PlayerCamera" , typeof(GameObject)) as GameObject;
		playerCam  = Instantiate(camera, transform.position, Quaternion.Euler(0, 0, 0));
		playerCam.GetComponent<CameraFollow>().player = gameObject;
		GameObject.FindWithTag("MainCamera").GetComponent<Camera>().enabled = false;
	}

    public void SpectatorMode()
    {
        playerCam.GetComponent<CameraFollow>().player = null;

    }


	void OnDisable()
	{
        Destroy(playerCam);
	}

    // Fixed Update ticks every 0.02
    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
		checkBuffs();
	}

	// If I have a fixed number of buffs, then I can just use a hashmap and check if the buff timer is active. This will speed things up.
	// Checks and updates buffs on the user. 
	// To prevent
	public void checkBuffs(){
		// Make a copy of the buff list to iterate through
		IBuffable [] _buffListCopy = new IBuffable[BuffList.Count];
		BuffList.CopyTo(_buffListCopy,0);
		foreach (IBuffable b in _buffListCopy){
			if ((b.TimeElapsed) >= b.TickTime) {
				b.Apply (gameObject);
				b.TimeElapsed = 0;
			} else {
				b.TimeElapsed += Time.deltaTime;
			}
			if(b.Finished){
				b.Reset (gameObject);
				BuffList.Remove (b);
			}
		}
	}


}
